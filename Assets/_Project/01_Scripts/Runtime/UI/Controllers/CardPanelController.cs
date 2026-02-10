using System;
using System.Collections.Generic;
using FourMelds.Cards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class CardPanelController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private Button buttonPrefab;

    [Header("Card Look")]
    [SerializeField] private float cardWidth = 170f;
    [SerializeField] private float cardHeight = 220f;
    [SerializeField] private float cardSpacing = 18f;
    [SerializeField] private float hoverScale = 1.12f;

    [Header("Panel Placement")]
    [SerializeField] private bool forceRuntimePanelPlacement = false;
    [SerializeField] private Vector2 panelSize = new Vector2(980f, 280f);
    [SerializeField] private Vector2 panelAnchoredPosition = new Vector2(420f, 18f);

    public event Action<int> OnHandCardClicked;

    private readonly List<Button> _activeButtons = new();
    private readonly List<Button> _buttonPool = new();
    private bool _layoutReady;
    private bool _isInteractable = true;

    private RectTransform _tooltipRoot;
    private Text _tooltipText;

    private void Start()
    {
        // Keep scene-authored placement unless user explicitly re-enables it in code.
        forceRuntimePanelPlacement = false;
        EnsurePanelPlacement();
        EnsureContentLayout();
        EnsureTooltip();
    }

    public void RenderHand(IReadOnlyList<int> handCardIndices)
    {
        ReleaseAllActiveButtons();

        if (panelRoot == null || contentRoot == null || buttonPrefab == null)
        {
            Debug.LogError("[CardPanel] UI refs are null (Inspector?)");
            return;
        }

        EnsurePanelPlacement();
        EnsureContentLayout();
        EnsureTooltip();

        if (handCardIndices == null)
            handCardIndices = Array.Empty<int>();

        var cards = CardRegistry.DefaultCards;
        for (int i = 0; i < handCardIndices.Count; i++)
        {
            int handIndex = i;
            int registryIndex = handCardIndices[i];

            var btn = AcquireButton();
            if (btn == null)
                continue;

            string name = registryIndex >= 0 && registryIndex < cards.Count
                ? cards[registryIndex]?.Name ?? $"Card {registryIndex}"
                : $"Card {registryIndex}";

            string desc = registryIndex >= 0 && registryIndex < cards.Count
                ? GetCardDescription(cards[registryIndex])
                : "";

            var text = btn.GetComponentInChildren<Text>(true);
            if (text != null)
            {
                text.supportRichText = true;
                text.alignment = TextAnchor.UpperLeft;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Overflow;
                text.text = $"<b>{name}</b>\n<size=14>{desc}</size>";
            }

            btn.interactable = _isInteractable;
            ConfigureCardStyle(btn);
            ConfigureHover(btn, desc);

            btn.onClick.AddListener(() =>
            {
                if (!_isInteractable)
                    return;

                HideTooltip();
                OnHandCardClicked?.Invoke(handIndex);
            });
        }

        if (contentRoot is RectTransform rt)
            LayoutRebuilder.MarkLayoutForRebuild(rt);
    }

    private void EnsurePanelPlacement()
    {
        if (!forceRuntimePanelPlacement || panelRoot == null)
            return;

        if (panelRoot.transform is not RectTransform rt)
            return;

        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.sizeDelta = panelSize;
        rt.anchoredPosition = panelAnchoredPosition;
    }

    private void EnsureContentLayout()
    {
        if (_layoutReady)
            return;

        if (contentRoot is not RectTransform rt)
            return;

        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = new Vector2(14f, 10f);
        rt.offsetMax = new Vector2(-14f, -10f);

        var hlg = contentRoot.GetComponent<HorizontalLayoutGroup>();
        if (hlg == null)
            hlg = contentRoot.gameObject.AddComponent<HorizontalLayoutGroup>();

        hlg.padding = new RectOffset(10, 10, 8, 8);
        hlg.spacing = cardSpacing;
        hlg.childAlignment = TextAnchor.LowerCenter;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.childScaleWidth = false;
        hlg.childScaleHeight = false;

        var vlg = contentRoot.GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
            Destroy(vlg);

        var fitter = contentRoot.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        _layoutReady = true;
    }

    private void EnsureButtonLayout(Button btn)
    {
        if (btn == null)
            return;

        var rt = btn.transform as RectTransform;
        if (rt != null)
            rt.sizeDelta = new Vector2(cardWidth, cardHeight);

        var le = btn.GetComponent<LayoutElement>();
        if (le == null)
            le = btn.gameObject.AddComponent<LayoutElement>();
        le.minHeight = cardHeight;
        le.preferredHeight = cardHeight;
        le.flexibleHeight = 0f;
        le.minWidth = cardWidth;
        le.preferredWidth = cardWidth;
        le.flexibleWidth = 0f;

        var text = btn.GetComponentInChildren<Text>(true);
        if (text != null)
        {
            text.resizeTextForBestFit = false;
            text.fontSize = 18;
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
        }
    }

    private void ConfigureCardStyle(Button btn)
    {
        var img = btn.GetComponent<Image>();
        if (img != null)
        {
            img.color = new Color(0.12f, 0.16f, 0.24f, 0.94f);
            img.raycastTarget = true;
        }

        var outline = btn.GetComponent<Outline>();
        if (outline == null)
            outline = btn.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.72f, 0.78f, 0.93f, 0.72f);
        outline.effectDistance = new Vector2(1f, -1f);

        var shadow = btn.GetComponent<Shadow>();
        if (shadow == null)
            shadow = btn.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.38f);
        shadow.effectDistance = new Vector2(2f, -2f);
    }

    private void ConfigureHover(Button btn, string description)
    {
        if (btn == null)
            return;

        var trigger = btn.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = btn.gameObject.AddComponent<EventTrigger>();
        trigger.triggers ??= new List<EventTrigger.Entry>();
        trigger.triggers.Clear();

        var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener(_ =>
        {
            if (!_isInteractable)
                return;

            ApplyHoverVisual(btn, hovered: true);
            ShowTooltip(description);
        });

        var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener(_ =>
        {
            ApplyHoverVisual(btn, hovered: false);
            HideTooltip();
        });

        trigger.triggers.Add(enter);
        trigger.triggers.Add(exit);
    }

    private Button AcquireButton()
    {
        Button btn;
        var last = _buttonPool.Count - 1;
        if (last >= 0)
        {
            btn = _buttonPool[last];
            _buttonPool.RemoveAt(last);
            if (btn != null)
            {
                btn.transform.SetParent(contentRoot, false);
                btn.gameObject.SetActive(true);
                btn.transform.localScale = Vector3.one;
            }
        }
        else
        {
            btn = Instantiate(buttonPrefab, contentRoot);
            EnsureButtonLayout(btn);
        }

        if (btn == null)
            return null;

        EnsureButtonLayout(btn);
        btn.onClick.RemoveAllListeners();
        _activeButtons.Add(btn);
        return btn;
    }

    private void ReleaseAllActiveButtons()
    {
        HideTooltip();

        for (int i = 0; i < _activeButtons.Count; i++)
        {
            var btn = _activeButtons[i];
            if (btn == null)
                continue;

        btn.onClick.RemoveAllListeners();
        btn.transform.localScale = Vector3.one;
        ApplyHoverVisual(btn, hovered: false);
        btn.gameObject.SetActive(false);
        _buttonPool.Add(btn);
        }

        _activeButtons.Clear();
    }

    public void SetVisible(bool visible)
    {
        if (panelRoot != null)
            panelRoot.SetActive(visible);

        if (!visible)
            HideTooltip();
    }

    public void SetInteractable(bool interactable)
    {
        _isInteractable = interactable;
        for (int i = 0; i < _activeButtons.Count; i++)
        {
            var btn = _activeButtons[i];
            if (btn != null)
                btn.interactable = interactable;
        }

        if (!interactable)
            HideTooltip();
    }

    private void EnsureTooltip()
    {
        if (panelRoot == null || _tooltipRoot != null)
            return;

        var tipGO = new GameObject("CardTooltip", typeof(RectTransform), typeof(Image));
        _tooltipRoot = tipGO.GetComponent<RectTransform>();
        _tooltipRoot.SetParent(panelRoot.transform, false);
        _tooltipRoot.anchorMin = new Vector2(0.5f, 1f);
        _tooltipRoot.anchorMax = new Vector2(0.5f, 1f);
        _tooltipRoot.pivot = new Vector2(0.5f, 1f);
        _tooltipRoot.sizeDelta = new Vector2(560f, 76f);
        _tooltipRoot.anchoredPosition = new Vector2(0f, -8f);

        var bg = tipGO.GetComponent<Image>();
        bg.color = new Color(0.05f, 0.08f, 0.14f, 0.92f);
        bg.raycastTarget = false;

        var txtGO = new GameObject("Text", typeof(RectTransform), typeof(Text));
        var txtRt = txtGO.GetComponent<RectTransform>();
        txtRt.SetParent(_tooltipRoot, false);
        txtRt.anchorMin = new Vector2(0f, 0f);
        txtRt.anchorMax = new Vector2(1f, 1f);
        txtRt.offsetMin = new Vector2(12f, 8f);
        txtRt.offsetMax = new Vector2(-12f, -8f);

        _tooltipText = txtGO.GetComponent<Text>();
        var builtInFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (builtInFont != null)
            _tooltipText.font = builtInFont;
        _tooltipText.fontSize = 18;
        _tooltipText.alignment = TextAnchor.MiddleLeft;
        _tooltipText.color = new Color(0.92f, 0.95f, 1f, 1f);
        _tooltipText.horizontalOverflow = HorizontalWrapMode.Wrap;
        _tooltipText.verticalOverflow = VerticalWrapMode.Truncate;
        _tooltipText.raycastTarget = false;

        _tooltipRoot.gameObject.SetActive(false);
    }

    private void ShowTooltip(string text)
    {
        if (_tooltipRoot == null || _tooltipText == null)
            return;

        _tooltipText.text = string.IsNullOrWhiteSpace(text) ? "효과 설명 없음" : text;
        _tooltipRoot.gameObject.SetActive(true);
    }

    private void HideTooltip()
    {
        if (_tooltipRoot != null)
            _tooltipRoot.gameObject.SetActive(false);
    }

    private static string GetCardDescription(ICardEffect card)
    {
        if (card == null)
            return "효과 없음";

        return card.Id switch
        {
            "draw.random5" => "랜덤 패 5장 드로우(자패 우선 1장)",
            "draw.manzu3" => "만수 패 3장 드로우",
            "draw.honor1" => "자패 1장 드로우",
            "exchange.2" => "손패 2장 버리고 2장 다시 드로우",
            "draw.tanyao4" => "2~8 수패 4장 드로우",
            _ => card.Name,
        };
    }

    private static void ApplyHoverVisual(Button btn, bool hovered)
    {
        if (btn == null)
            return;

        var outline = btn.GetComponent<Outline>();
        if (outline != null)
        {
            outline.effectColor = hovered
                ? new Color(0.92f, 0.96f, 1f, 0.98f)
                : new Color(0.72f, 0.78f, 0.93f, 0.72f);
            outline.effectDistance = hovered ? new Vector2(2f, -2f) : new Vector2(1f, -1f);
        }

        var img = btn.GetComponent<Image>();
        if (img != null)
        {
            img.color = hovered
                ? new Color(0.18f, 0.24f, 0.34f, 0.98f)
                : new Color(0.12f, 0.16f, 0.24f, 0.94f);
        }
    }
}
