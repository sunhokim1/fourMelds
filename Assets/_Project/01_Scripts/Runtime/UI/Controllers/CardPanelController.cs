using System;
using System.Collections.Generic;
using FourMelds.Cards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class CardPanelController : MonoBehaviour
{
    private const float MinLargeDragPlayRadius = 660f;
#if UNITY_EDITOR
    private const string UiArtDir = "Assets/_Project/04_Art/UI";
#endif

    [Header("UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private Button buttonPrefab;

    [Header("Card Look")]
    [SerializeField] private bool usePrefabCardSize = true;
    [SerializeField] private float cardWidth = 180f;
    [SerializeField] private float cardHeight = 240f;
    [SerializeField] private float cardSpacing = 18f;
    [SerializeField] private float hoverScale = 1.12f;
    [SerializeField] private Color cardNormalColor = new Color(0.12f, 0.16f, 0.24f, 0.94f);
    [SerializeField] private Color cardHoverColor = new Color(0.18f, 0.24f, 0.34f, 0.98f);
    [SerializeField] private Color cardPressedColor = new Color(0.24f, 0.30f, 0.42f, 1f);
    [SerializeField] private Color cardDisabledColor = new Color(0.10f, 0.12f, 0.18f, 0.65f);

    [Header("Play Input")]
    [SerializeField] private bool requireDragToPlay = true;
    [SerializeField] private Vector2 dragPlayViewportTarget = new Vector2(0.5f, 0.5f);
    [SerializeField] private float dragPlayRadius = 660f;
    [SerializeField] private float dragLiftScale = 1.18f;
    [SerializeField] private bool requireDropOutsidePanel = true;
    [SerializeField] private float outsidePanelPadding = 18f;
    [SerializeField] private bool showDragPlayGuide = false;
    [SerializeField] private string dragPlayGuideText = "카드를 여기로 드래그";
    [SerializeField] private Color dragGuideFillColor = new Color(0.22f, 0.72f, 1f, 0.12f);
    [SerializeField] private Color dragGuideRingColor = new Color(0.62f, 0.86f, 1f, 0.72f);
    [SerializeField] private Color dragGuideTextColor = new Color(0.90f, 0.96f, 1f, 0.92f);
    [SerializeField] private Color dragValidColor = new Color(0.14f, 0.50f, 0.30f, 0.98f);
    [SerializeField] private Color dragValidOutlineColor = new Color(0.66f, 1f, 0.78f, 0.98f);
    [SerializeField] private float dragValidScaleMultiplier = 1.30f;
    [Header("Card Skin Sprites")]
    [SerializeField] private Sprite cardFrameSprite;
    [SerializeField] private Sprite nameBannerSprite;
    [SerializeField] private Sprite artFrameSprite;
    [SerializeField] private Sprite descriptionPanelSprite;
    [SerializeField] private Sprite rarityGemCommonSprite;
    [SerializeField] private Sprite rarityGemRareSprite;
    [SerializeField] private Sprite rarityGemEpicSprite;
    [SerializeField] private Sprite rarityGemLegendarySprite;
    [SerializeField] private Sprite rarityGemDreamSprite;
    [Header("Rarity Frame Overrides (Optional)")]
    [SerializeField] private Sprite rarityFrameCommonSprite;
    [SerializeField] private Sprite rarityFrameRareSprite;
    [SerializeField] private Sprite rarityFrameEpicSprite;
    [SerializeField] private Sprite rarityFrameLegendarySprite;
    [SerializeField] private Sprite rarityFrameDreamSprite;

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
    private Canvas _rootCanvas;
    private RectTransform _rootCanvasRect;
    private Button _draggingButton;
    private Transform _dragOriginalParent;
    private int _dragOriginalSiblingIndex = -1;
    private Vector2 _dragPointerOffset;
    private RectTransform _dragPlayGuideRoot;
    private Image _dragPlayGuideFill;
    private Image _dragPlayGuideRing;
    private Text _dragPlayGuideLabel;
    private int _lastGuideRadiusPx = -1;
    private bool _dragOverValidZone;

    private static Sprite _sharedCircleFillSprite;
    private static Sprite _sharedCircleRingSprite;

    private void Start()
    {
        EnforceRuntimeDragDefaults();

        // Keep scene-authored placement unless user explicitly re-enables it in code.
        forceRuntimePanelPlacement = false;
        EnsurePanelPlacement();
        EnsureContentLayout();
        EnsureTooltip();
        EnsureDragPlayGuide();
        UpdateDragPlayGuideVisibility();
    }

    private void Update()
    {
        UpdateDragPlayGuideLayout();
    }

    private void OnValidate()
    {
        if (requireDragToPlay && dragPlayRadius < MinLargeDragPlayRadius)
            dragPlayRadius = MinLargeDragPlayRadius;

#if UNITY_EDITOR
        AutoAssignRarityFrameSpritesInEditor();
#endif
    }

#if UNITY_EDITOR
    private void AutoAssignRarityFrameSpritesInEditor()
    {
        bool changed = false;
        changed |= TryAssignIfNull(ref rarityFrameCommonSprite, $"{UiArtDir}/card_common.png");
        changed |= TryAssignIfNull(ref rarityFrameRareSprite, $"{UiArtDir}/card_rare.png");
        changed |= TryAssignIfNull(ref rarityFrameEpicSprite, $"{UiArtDir}/card_epic.png");
        changed |= TryAssignIfNull(ref rarityFrameLegendarySprite, $"{UiArtDir}/card_regendary.png");
        changed |= TryAssignIfNull(ref rarityFrameDreamSprite, $"{UiArtDir}/card_dream.png");

        if (changed)
            EditorUtility.SetDirty(this);
    }

    private static bool TryAssignIfNull(ref Sprite target, string assetPath)
    {
        if (target != null)
            return false;

        var loaded = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (loaded == null)
            return false;

        target = loaded;
        return true;
    }
#endif

    private void EnforceRuntimeDragDefaults()
    {
        if (!requireDragToPlay)
            return;

        if (dragPlayRadius < MinLargeDragPlayRadius)
            dragPlayRadius = MinLargeDragPlayRadius;
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
        EnsureDragPlayGuide();
        UpdateDragPlayGuideVisibility();

        if (handCardIndices == null)
            handCardIndices = Array.Empty<int>();

        var cards = CardRegistry.DefaultCards;
        for (int i = 0; i < handCardIndices.Count; i++)
        {
            int handIndex = i;
            int registryIndex = handCardIndices[i];
            ICardEffect card = registryIndex >= 0 && registryIndex < cards.Count
                ? cards[registryIndex]
                : null;

            var btn = AcquireButton();
            if (btn == null)
                continue;

            ResolveCardDisplayData(registryIndex, card, out string name, out string desc, out int rarityTier);

            btn.interactable = _isInteractable;
            ConfigureCardStyle(btn);
            BindCardVisual(btn, name, desc, rarityTier);
            ConfigurePointerAndDrag(btn, handIndex, desc);

            if (!requireDragToPlay)
            {
                btn.onClick.AddListener(() =>
                {
                    if (!_isInteractable)
                        return;

                    HideTooltip();
                    OnHandCardClicked?.Invoke(handIndex);
                });
            }
        }

        if (contentRoot is RectTransform rt)
            LayoutRebuilder.MarkLayoutForRebuild(rt);
    }

    public bool TryRenderCardPreview(Button targetButton, int registryIndex)
    {
        if (targetButton == null)
            return false;

        var cards = CardRegistry.DefaultCards;
        ICardEffect card = registryIndex >= 0 && registryIndex < cards.Count
            ? cards[registryIndex]
            : null;

        ResolveCardDisplayData(registryIndex, card, out string name, out string desc, out int rarityTier);
        BindCardVisual(targetButton, name, desc, rarityTier);
        return true;
    }

    public Button CreatePreviewCardButton(Transform parent, int registryIndex, Action onClick)
    {
        if (buttonPrefab == null || parent == null)
            return null;

        var btn = Instantiate(buttonPrefab, parent);
        btn.gameObject.SetActive(true);
        EnsureButtonLayout(btn);
        btn.interactable = true;
        TryRenderCardPreview(btn, registryIndex);
        SetCardInteractableState(btn, true);
        SetCardHoverState(btn, false);
        SetCardDraggingState(btn, dragging: false, isValidDropZone: false);

        btn.onClick.RemoveAllListeners();
        if (onClick != null)
            btn.onClick.AddListener(() => onClick());

        return btn;
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
        float targetWidth = cardWidth;
        float targetHeight = cardHeight;
        if (usePrefabCardSize && rt != null)
        {
            targetWidth = rt.sizeDelta.x;
            targetHeight = rt.sizeDelta.y;
        }
        else if (rt != null)
        {
            rt.sizeDelta = new Vector2(targetWidth, targetHeight);
        }

        var le = btn.GetComponent<LayoutElement>();
        if (le == null)
            le = btn.gameObject.AddComponent<LayoutElement>();
        le.minHeight = targetHeight;
        le.preferredHeight = targetHeight;
        le.flexibleHeight = 0f;
        le.minWidth = targetWidth;
        le.preferredWidth = targetWidth;
        le.flexibleWidth = 0f;

        // Text layout/styling is authored in prefab/CardView. Do not override here.
    }

    private void ConfigureCardStyle(Button btn)
    {
        if (btn == null)
            return;

        var img = btn.GetComponent<Image>();
        if (img != null)
        {
            img.raycastTarget = true;
        }

        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = Color.white;
        colors.disabledColor = new Color(1f, 1f, 1f, 0.55f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        btn.colors = colors;
        btn.transition = Selectable.Transition.None;

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

        if (TryGetCardView(btn, out var view))
        {
            view.SetColors(
                normalColor: cardNormalColor,
                hoverColor: cardHoverColor,
                disabledColor: cardDisabledColor,
                dragValidColor: dragValidColor,
                dragValidOutlineColor: dragValidOutlineColor);
            view.SetInteractable(_isInteractable);
        }
    }

    private void ConfigurePointerAndDrag(Button btn, int handIndex, string description)
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
            if (_draggingButton == btn)
                return;

            btn.transform.localScale = Vector3.one * Mathf.Max(1f, hoverScale);
            ApplyHoverVisual(btn, hovered: true, normalColor: cardNormalColor, hoverColor: cardHoverColor);
            ShowTooltip(description);
        });

        var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener(_ =>
        {
            if (_draggingButton == btn)
                return;

            btn.transform.localScale = Vector3.one;
            ApplyHoverVisual(btn, hovered: false, normalColor: cardNormalColor, hoverColor: cardHoverColor);
            HideTooltip();
        });

        var beginDrag = new EventTrigger.Entry { eventID = EventTriggerType.BeginDrag };
        beginDrag.callback.AddListener(ev =>
        {
            if (!requireDragToPlay || !_isInteractable)
                return;
            if (ev is not PointerEventData ped)
                return;

            BeginCardDrag(btn, ped);
        });

        var drag = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
        drag.callback.AddListener(ev =>
        {
            if (!requireDragToPlay || ev is not PointerEventData ped)
                return;

            UpdateCardDrag(btn, ped);
        });

        var endDrag = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
        endDrag.callback.AddListener(ev =>
        {
            if (!requireDragToPlay || ev is not PointerEventData ped)
                return;

            EndCardDrag(btn, handIndex, ped);
        });

        trigger.triggers.Add(enter);
        trigger.triggers.Add(exit);
        trigger.triggers.Add(beginDrag);
        trigger.triggers.Add(drag);
        trigger.triggers.Add(endDrag);
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
        EnsureCardView(btn);
        btn.onClick.RemoveAllListeners();
        _activeButtons.Add(btn);
        return btn;
    }

    private void ReleaseAllActiveButtons()
    {
        if (_draggingButton != null)
            ResetDragState(restoreVisualToContentRoot: true);

        HideTooltip();

        for (int i = 0; i < _activeButtons.Count; i++)
        {
            var btn = _activeButtons[i];
            if (btn == null)
                continue;

            btn.onClick.RemoveAllListeners();
            btn.transform.localScale = Vector3.one;
            SetCardDraggingState(btn, dragging: false, isValidDropZone: false);
            SetCardHoverState(btn, hovered: false);
            ApplyHoverVisual(btn, hovered: false, normalColor: cardNormalColor, hoverColor: cardHoverColor);
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

        UpdateDragPlayGuideVisibility();
    }

    public void SetInteractable(bool interactable)
    {
        _isInteractable = interactable;
        for (int i = 0; i < _activeButtons.Count; i++)
        {
            var btn = _activeButtons[i];
            if (btn != null)
            {
                btn.interactable = interactable;
                SetCardInteractableState(btn, interactable);
            }
        }

        if (!interactable)
            HideTooltip();

        UpdateDragPlayGuideVisibility();
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
        _tooltipText.fontSize = 16;
        _tooltipText.alignment = TextAnchor.UpperLeft;
        _tooltipText.color = new Color(0.92f, 0.95f, 1f, 1f);
        _tooltipText.supportRichText = true;
        _tooltipText.horizontalOverflow = HorizontalWrapMode.Wrap;
        _tooltipText.verticalOverflow = VerticalWrapMode.Overflow;
        _tooltipText.raycastTarget = false;

        var fitter = tipGO.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = tipGO.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var layoutElement = tipGO.GetComponent<LayoutElement>();
        if (layoutElement == null)
            layoutElement = tipGO.AddComponent<LayoutElement>();
        layoutElement.minHeight = 76f;
        layoutElement.preferredHeight = 76f;

        _tooltipRoot.gameObject.SetActive(false);
    }

    private void ShowTooltip(string text)
    {
        if (_tooltipRoot == null || _tooltipText == null)
            return;

        _tooltipText.text = string.IsNullOrWhiteSpace(text) ? "효과 설명 없음" : text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_tooltipRoot);
        _tooltipRoot.gameObject.SetActive(true);
    }

    private void HideTooltip()
    {
        if (_tooltipRoot != null)
            _tooltipRoot.gameObject.SetActive(false);
    }

    private static void ResolveCardDisplayData(int registryIndex, ICardEffect card, out string name, out string description, out int rarityTier)
    {
        name = card?.Name ?? $"Card {registryIndex}";
        description = "효과 설명 없음";
        rarityTier = 0;

        if (CardRegistry.TryGetDefinition(registryIndex, out var def) && def != null)
        {
            if (!string.IsNullOrWhiteSpace(def.name))
                name = def.name;
            if (!string.IsNullOrWhiteSpace(def.description))
                description = def.description;
            rarityTier = Mathf.Clamp(def.rarity, 0, 4);
            description = StylizeDescription(description);
            return;
        }

        if (card != null && !string.IsNullOrWhiteSpace(card.Name))
            description = card.Name;
        description = StylizeDescription(description);
    }

    private static string StylizeDescription(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        string styled = text;
        styled = Highlight(styled, "만수", "#C73E3A");
        styled = Highlight(styled, "통수", "#2F6FDB");
        styled = Highlight(styled, "삭수", "#2E8B57");
        styled = Highlight(styled, "자패", "#B8860B");
        styled = Highlight(styled, "드로우", "#5B6EE1");
        styled = Highlight(styled, "쯔모", "#5B6EE1");
        styled = Highlight(styled, "교환", "#D97706");
        styled = Highlight(styled, "랜덤", "#6B7280");
        return styled;
    }

    private static string Highlight(string source, string keyword, string colorHex)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(keyword))
            return source;

        string tag = $"<color={colorHex}>{keyword}</color>";
        return source.Replace(keyword, tag);
    }

    private void BeginCardDrag(Button btn, PointerEventData ped)
    {
        if (_draggingButton != null || btn == null)
            return;

        EnsureRootCanvasRefs();
        if (_rootCanvasRect == null)
            return;

        var rt = btn.transform as RectTransform;
        if (rt == null)
            return;

        _draggingButton = btn;
        _dragOriginalParent = btn.transform.parent;
        _dragOriginalSiblingIndex = btn.transform.GetSiblingIndex();

        var cg = btn.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = btn.gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        btn.transform.SetParent(_rootCanvasRect, true);
        btn.transform.SetAsLastSibling();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvasRect,
            ped.position,
            ped.pressEventCamera,
            out var pointerLocal))
        {
            _dragPointerOffset = rt.anchoredPosition - pointerLocal;
        }
        else
        {
            _dragPointerOffset = Vector2.zero;
        }

        rt.localScale = Vector3.one * Mathf.Max(1f, dragLiftScale);
        _dragOverValidZone = false;
        SetCardDraggingState(btn, dragging: true, isValidDropZone: false);
        UpdateDragDropFeedback(btn, isValidDropZone: false);
        HideTooltip();
    }

    private void UpdateCardDrag(Button btn, PointerEventData ped)
    {
        if (_draggingButton != btn || btn == null || _rootCanvasRect == null)
            return;

        var rt = btn.transform as RectTransform;
        if (rt == null)
            return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvasRect,
            ped.position,
            ped.pressEventCamera,
            out var pointerLocal))
        {
            return;
        }

        rt.anchoredPosition = pointerLocal + _dragPointerOffset;
        UpdateDragDropFeedback(btn, IsInsideDragPlayTarget(ped));
    }

    private void EndCardDrag(Button btn, int handIndex, PointerEventData ped)
    {
        if (_draggingButton != btn || btn == null)
            return;

        bool shouldPlay = IsInsideDragPlayTarget(ped);
        ResetDragState(restoreVisualToContentRoot: true);

        if (shouldPlay && _isInteractable)
        {
            HideTooltip();
            OnHandCardClicked?.Invoke(handIndex);
        }
    }

    private bool IsInsideDragPlayTarget(PointerEventData ped)
    {
        if (ped == null)
            return false;

        if (requireDropOutsidePanel && !IsPointerOutsidePanelArea(ped.position))
            return false;

        var target = new Vector2(
            Screen.width * dragPlayViewportTarget.x,
            Screen.height * dragPlayViewportTarget.y);

        return Vector2.Distance(ped.position, target) <= Mathf.Max(16f, dragPlayRadius);
    }

    private bool IsPointerOutsidePanelArea(Vector2 screenPoint)
    {
        if (panelRoot == null || panelRoot.transform is not RectTransform panelRt)
            return true;

        EnsureRootCanvasRefs();
        Camera cam = _rootCanvas != null && _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : _rootCanvas != null ? _rootCanvas.worldCamera : null;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRt, screenPoint, cam, out var localPoint))
            return true;

        var rect = panelRt.rect;
        float pad = Mathf.Max(0f, outsidePanelPadding);
        rect.xMin -= pad;
        rect.xMax += pad;
        rect.yMin -= pad;
        rect.yMax += pad;

        return !rect.Contains(localPoint);
    }

    private void ResetDragState(bool restoreVisualToContentRoot)
    {
        var btn = _draggingButton;
        if (btn == null)
            return;

        var cg = btn.GetComponent<CanvasGroup>();
        if (cg != null)
            cg.blocksRaycasts = true;

        if (restoreVisualToContentRoot && contentRoot != null)
        {
            btn.transform.SetParent(contentRoot, false);
            if (_dragOriginalSiblingIndex >= 0)
            {
                int maxSibling = Mathf.Max(0, contentRoot.childCount - 1);
                btn.transform.SetSiblingIndex(Mathf.Min(_dragOriginalSiblingIndex, maxSibling));
            }
        }
        else if (restoreVisualToContentRoot && _dragOriginalParent != null)
        {
            btn.transform.SetParent(_dragOriginalParent, false);
            if (_dragOriginalSiblingIndex >= 0)
                btn.transform.SetSiblingIndex(_dragOriginalSiblingIndex);
        }

        btn.transform.localScale = Vector3.one;
        SetCardDraggingState(btn, dragging: false, isValidDropZone: false);
        SetCardHoverState(btn, hovered: false);
        ApplyHoverVisual(btn, hovered: false, normalColor: cardNormalColor, hoverColor: cardHoverColor);

        _draggingButton = null;
        _dragOriginalParent = null;
        _dragOriginalSiblingIndex = -1;
        _dragPointerOffset = Vector2.zero;
        _dragOverValidZone = false;

        if (contentRoot is RectTransform rt)
            LayoutRebuilder.MarkLayoutForRebuild(rt);
    }

    private void EnsureRootCanvasRefs()
    {
        if (_rootCanvasRect != null)
            return;

        if (panelRoot == null)
            return;

        var canvas = panelRoot.GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        _rootCanvas = canvas.rootCanvas != null ? canvas.rootCanvas : canvas;
        _rootCanvasRect = _rootCanvas.transform as RectTransform;
    }

    private void EnsureDragPlayGuide()
    {
        EnsureRootCanvasRefs();
        if (_rootCanvasRect == null || _dragPlayGuideRoot != null)
            return;

        var root = new GameObject("DragPlayGuide", typeof(RectTransform));
        _dragPlayGuideRoot = root.GetComponent<RectTransform>();
        _dragPlayGuideRoot.SetParent(_rootCanvasRect, false);
        _dragPlayGuideRoot.anchorMin = new Vector2(0.5f, 0.5f);
        _dragPlayGuideRoot.anchorMax = new Vector2(0.5f, 0.5f);
        _dragPlayGuideRoot.pivot = new Vector2(0.5f, 0.5f);

        _dragPlayGuideFill = CreateGuideImage("Fill", _dragPlayGuideRoot, GetCircleFillSprite());
        _dragPlayGuideRing = CreateGuideImage("Ring", _dragPlayGuideRoot, GetCircleRingSprite());

        var textGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
        var textRt = textGo.GetComponent<RectTransform>();
        textRt.SetParent(_dragPlayGuideRoot, false);
        textRt.anchorMin = new Vector2(0.5f, 0.5f);
        textRt.anchorMax = new Vector2(0.5f, 0.5f);
        textRt.pivot = new Vector2(0.5f, 0.5f);
        textRt.sizeDelta = new Vector2(260f, 42f);

        _dragPlayGuideLabel = textGo.GetComponent<Text>();
        var builtInFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (builtInFont != null)
            _dragPlayGuideLabel.font = builtInFont;
        _dragPlayGuideLabel.fontSize = 18;
        _dragPlayGuideLabel.alignment = TextAnchor.MiddleCenter;
        _dragPlayGuideLabel.text = dragPlayGuideText;
        _dragPlayGuideLabel.color = dragGuideTextColor;
        _dragPlayGuideLabel.raycastTarget = false;

        _dragPlayGuideRoot.gameObject.SetActive(false);
        UpdateDragPlayGuideLayout(force: true);
    }

    private static Image CreateGuideImage(string name, RectTransform parent, Sprite sprite)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        var img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.type = Image.Type.Simple;
        img.preserveAspect = true;
        img.raycastTarget = false;
        return img;
    }

    private void UpdateDragPlayGuideVisibility()
    {
        if (_dragPlayGuideRoot == null)
            return;

        bool visible = ShouldShowDragPlayGuide();
        if (_dragPlayGuideRoot.gameObject.activeSelf != visible)
            _dragPlayGuideRoot.gameObject.SetActive(visible);

        if (!visible)
            return;

        if (_dragPlayGuideFill != null)
            _dragPlayGuideFill.color = dragGuideFillColor;
        if (_dragPlayGuideRing != null)
            _dragPlayGuideRing.color = dragGuideRingColor;
        if (_dragPlayGuideLabel != null)
        {
            _dragPlayGuideLabel.text = dragPlayGuideText;
            _dragPlayGuideLabel.color = dragGuideTextColor;
        }

        UpdateDragPlayGuideLayout(force: true);
    }

    private bool ShouldShowDragPlayGuide()
    {
        if (!showDragPlayGuide || !requireDragToPlay || !_isInteractable)
            return false;
        if (panelRoot == null || !panelRoot.activeInHierarchy)
            return false;
        if (_draggingButton == null)
            return false;
        return true;
    }

    private void UpdateDragDropFeedback(Button btn, bool isValidDropZone)
    {
        if (btn == null)
            return;

        if (_dragOverValidZone == isValidDropZone)
            return;

        _dragOverValidZone = isValidDropZone;

        float baseScale = Mathf.Max(1f, dragLiftScale);
        float validScale = baseScale * Mathf.Max(1f, dragValidScaleMultiplier);
        btn.transform.localScale = Vector3.one * (isValidDropZone ? validScale : baseScale);
        SetCardDraggingState(btn, dragging: true, isValidDropZone: isValidDropZone);

        var img = btn.GetComponent<Image>();
        if (img != null)
            img.raycastTarget = true;

        var outline = btn.GetComponent<Outline>();
        if (outline != null)
        {
            outline.effectColor = isValidDropZone ? dragValidOutlineColor : new Color(0.92f, 0.96f, 1f, 0.98f);
            outline.effectDistance = isValidDropZone ? new Vector2(3f, -3f) : new Vector2(2f, -2f);
        }
    }

    private void UpdateDragPlayGuideLayout(bool force = false)
    {
        if (_dragPlayGuideRoot == null || !_dragPlayGuideRoot.gameObject.activeInHierarchy)
            return;
        if (_rootCanvasRect == null)
            return;

        int radiusPx = Mathf.RoundToInt(Mathf.Max(16f, dragPlayRadius));
        if (force || _lastGuideRadiusPx != radiusPx)
        {
            _lastGuideRadiusPx = radiusPx;
            float diameter = radiusPx * 2f;
            _dragPlayGuideRoot.sizeDelta = new Vector2(diameter, diameter);

            if (_dragPlayGuideFill != null && _dragPlayGuideFill.transform is RectTransform fillRt)
                fillRt.sizeDelta = new Vector2(diameter, diameter);
            if (_dragPlayGuideRing != null && _dragPlayGuideRing.transform is RectTransform ringRt)
                ringRt.sizeDelta = new Vector2(diameter, diameter);
        }

        var screenPos = new Vector2(
            Screen.width * dragPlayViewportTarget.x,
            Screen.height * dragPlayViewportTarget.y);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvasRect,
            screenPos,
            _rootCanvas != null && _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _rootCanvas.worldCamera,
            out var localPos))
        {
            _dragPlayGuideRoot.anchoredPosition = localPos;
        }
    }

    private static Sprite GetCircleFillSprite()
    {
        if (_sharedCircleFillSprite != null)
            return _sharedCircleFillSprite;

        _sharedCircleFillSprite = BuildCircleSprite(size: 128, innerCutoffNormalized: -1f);
        return _sharedCircleFillSprite;
    }

    private static Sprite GetCircleRingSprite()
    {
        if (_sharedCircleRingSprite != null)
            return _sharedCircleRingSprite;

        _sharedCircleRingSprite = BuildCircleSprite(size: 128, innerCutoffNormalized: 0.88f);
        return _sharedCircleRingSprite;
    }

    private static Sprite BuildCircleSprite(int size, float innerCutoffNormalized)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, mipChain: false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        float c = (size - 1) * 0.5f;
        float r = size * 0.5f;
        const float edge = 0.015f;
        var pixels = new Color32[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x - c) / r;
                float dy = (y - c) / r;
                float d = Mathf.Sqrt(dx * dx + dy * dy);

                float alphaOuter = 1f - Mathf.Clamp01((d - (1f - edge)) / edge);
                float alphaInner = 1f;
                if (innerCutoffNormalized >= 0f)
                    alphaInner = Mathf.Clamp01((d - innerCutoffNormalized) / edge);

                float alpha = Mathf.Clamp01(alphaOuter * alphaInner);
                byte a = (byte)Mathf.RoundToInt(alpha * 255f);
                pixels[y * size + x] = new Color32(255, 255, 255, a);
            }
        }

        tex.SetPixels32(pixels);
        tex.Apply(updateMipmaps: false, makeNoLongerReadable: false);
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    private void ApplyHoverVisual(Button btn, bool hovered, Color normalColor, Color hoverColor)
    {
        if (btn == null)
            return;

        SetCardHoverState(btn, hovered);

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
            img.raycastTarget = true;
        }
    }

    private void BindCardVisual(Button btn, string name, string desc, int rarityTier)
    {
        if (btn == null)
            return;

        var view = EnsureCardView(btn);
        if (view == null)
            return;

        view.Bind(name, desc);
        view.SetSkinSprites(
            frameSprite: cardFrameSprite,
            bannerSprite: nameBannerSprite,
            artSprite: artFrameSprite,
            descriptionSprite: descriptionPanelSprite,
            gemCommon: rarityGemCommonSprite,
            gemRare: rarityGemRareSprite,
            gemEpic: rarityGemEpicSprite,
            gemLegendary: rarityGemLegendarySprite,
            gemDream: rarityGemDreamSprite);
        view.SetRarityFrameSprites(
            common: rarityFrameCommonSprite,
            rare: rarityFrameRareSprite,
            epic: rarityFrameEpicSprite,
            legendary: rarityFrameLegendarySprite,
            dream: rarityFrameDreamSprite);
        view.SetColors(
            normalColor: cardNormalColor,
            hoverColor: cardHoverColor,
            disabledColor: cardDisabledColor,
            dragValidColor: dragValidColor,
            dragValidOutlineColor: dragValidOutlineColor);
        view.SetRarityTier(rarityTier);
        view.SetInteractable(_isInteractable);
        view.SetHover(false);
        view.SetDragging(dragging: false, dragValid: false);
    }

    private static bool TryGetCardView(Button btn, out CardView view)
    {
        view = null;
        if (btn == null)
            return false;

        view = btn.GetComponent<CardView>();
        return view != null;
    }

    private static CardView EnsureCardView(Button btn)
    {
        if (btn == null)
            return null;

        var view = btn.GetComponent<CardView>();
        if (view == null)
            view = btn.gameObject.AddComponent<CardView>();

        view.EnsureScaffold();
        return view;
    }

    private static void SetCardHoverState(Button btn, bool hovered)
    {
        if (TryGetCardView(btn, out var view))
            view.SetHover(hovered);
    }

    private static void SetCardDraggingState(Button btn, bool dragging, bool isValidDropZone)
    {
        if (TryGetCardView(btn, out var view))
            view.SetDragging(dragging, isValidDropZone);
    }

    private static void SetCardInteractableState(Button btn, bool interactable)
    {
        if (TryGetCardView(btn, out var view))
            view.SetInteractable(interactable);
    }
}
