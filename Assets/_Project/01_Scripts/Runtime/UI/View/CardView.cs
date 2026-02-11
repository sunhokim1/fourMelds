using UnityEngine;
using UnityEngine.UI;

public sealed class CardView : MonoBehaviour
{
    private const float BorderSize = 10f;

    [Header("Skin Sprites")]
    [SerializeField] private Sprite cardFrameSprite;
    [SerializeField] private Sprite nameBannerSprite;
    [SerializeField] private Sprite artFrameSprite;
    [SerializeField] private Sprite descriptionPanelSprite;
    [SerializeField] private Sprite rarityGemCommonSprite;
    [SerializeField] private Sprite rarityGemRareSprite;
    [SerializeField] private Sprite rarityGemEpicSprite;
    [SerializeField] private Sprite rarityGemLegendarySprite;

    private Image _frameImage;
    private Outline _frameOutline;
    private RectTransform _contentRoot;
    private Image _nameBannerImage;
    private Image _artFrameImage;
    private Image _descriptionPanelImage;
    private Image _rarityGemImage;

    private Text _nameText;
    private Text _descriptionText;

    private bool _hovered;
    private bool _dragging;
    private bool _dragValid;
    private bool _interactable = true;

    private Color _normalColor = new Color(0.12f, 0.16f, 0.24f, 0.94f);
    private Color _hoverColor = new Color(0.18f, 0.24f, 0.34f, 0.98f);
    private Color _disabledColor = new Color(0.10f, 0.12f, 0.18f, 0.65f);
    private Color _dragValidColor = new Color(0.14f, 0.50f, 0.30f, 0.98f);
    private Color _dragValidOutlineColor = new Color(0.66f, 1f, 0.78f, 0.98f);

    private void Awake()
    {
        EnsureScaffold();
    }

    private void OnValidate()
    {
        if (!isActiveAndEnabled)
            return;

        EnsureScaffold();
        ApplySkinSprites();
    }

    public void EnsureScaffold()
    {
        if (_frameImage == null)
            _frameImage = GetComponent<Image>();
        if (_frameImage == null)
            _frameImage = gameObject.AddComponent<Image>();
        _frameImage.raycastTarget = true;

        if (_frameOutline == null)
            _frameOutline = GetComponent<Outline>();
        if (_frameOutline == null)
            _frameOutline = gameObject.AddComponent<Outline>();

        _frameOutline.effectDistance = new Vector2(1f, -1f);
        _frameOutline.effectColor = new Color(0.72f, 0.78f, 0.93f, 0.72f);

        var rootRt = transform as RectTransform;
        if (rootRt == null)
            return;

        _contentRoot = EnsureRectChild("CardContent", rootRt);
        _contentRoot.anchorMin = Vector2.zero;
        _contentRoot.anchorMax = Vector2.one;
        _contentRoot.offsetMin = new Vector2(BorderSize, BorderSize);
        _contentRoot.offsetMax = new Vector2(-BorderSize, -BorderSize);

        EnsureNameBanner();
        EnsureArtFrame();
        EnsureDescriptionPanel();
        EnsureRarityGem();
        DisableLegacyRootText();
        ApplySkinSprites();

        ApplyVisualState();
    }

    public void Bind(string cardName, string description)
    {
        EnsureScaffold();

        if (_nameText != null)
            _nameText.text = cardName;
        if (_descriptionText != null)
            _descriptionText.text = description;
    }

    public void SetInteractable(bool interactable)
    {
        _interactable = interactable;
        ApplyVisualState();
    }

    public void SetHover(bool hovered)
    {
        _hovered = hovered;
        ApplyVisualState();
    }

    public void SetDragging(bool dragging, bool dragValid)
    {
        _dragging = dragging;
        _dragValid = dragValid;
        ApplyVisualState();
    }

    public void SetColors(Color normalColor, Color hoverColor, Color disabledColor, Color dragValidColor, Color dragValidOutlineColor)
    {
        _normalColor = normalColor;
        _hoverColor = hoverColor;
        _disabledColor = disabledColor;
        _dragValidColor = dragValidColor;
        _dragValidOutlineColor = dragValidOutlineColor;
        ApplyVisualState();
    }

    public void SetSkinSprites(
        Sprite frameSprite,
        Sprite bannerSprite,
        Sprite artSprite,
        Sprite descriptionSprite,
        Sprite gemCommon,
        Sprite gemRare,
        Sprite gemEpic,
        Sprite gemLegendary)
    {
        cardFrameSprite = frameSprite;
        nameBannerSprite = bannerSprite;
        artFrameSprite = artSprite;
        descriptionPanelSprite = descriptionSprite;
        rarityGemCommonSprite = gemCommon;
        rarityGemRareSprite = gemRare;
        rarityGemEpicSprite = gemEpic;
        rarityGemLegendarySprite = gemLegendary;

        ApplySkinSprites();
    }

    public void SetRarityTier(int tier)
    {
        if (_rarityGemImage == null)
            return;

        _rarityGemImage.sprite = tier switch
        {
            3 => rarityGemLegendarySprite != null ? rarityGemLegendarySprite : rarityGemCommonSprite,
            2 => rarityGemEpicSprite != null ? rarityGemEpicSprite : rarityGemCommonSprite,
            1 => rarityGemRareSprite != null ? rarityGemRareSprite : rarityGemCommonSprite,
            _ => rarityGemCommonSprite
        };

        _rarityGemImage.type = ShouldUseSliced(_rarityGemImage.sprite) ? Image.Type.Sliced : Image.Type.Simple;
        _rarityGemImage.preserveAspect = true;
    }

    private void ApplyVisualState()
    {
        if (_frameImage == null)
            return;

        if (!_interactable)
        {
            _frameImage.color = _disabledColor;
            SetOutlineColor(new Color(0.35f, 0.38f, 0.48f, 0.45f), new Vector2(1f, -1f));
            return;
        }

        if (_dragging && _dragValid)
        {
            _frameImage.color = _dragValidColor;
            SetOutlineColor(_dragValidOutlineColor, new Vector2(3f, -3f));
            return;
        }

        if (_hovered || _dragging)
        {
            _frameImage.color = _hoverColor;
            SetOutlineColor(new Color(0.92f, 0.96f, 1f, 0.98f), new Vector2(2f, -2f));
            return;
        }

        _frameImage.color = _normalColor;
        SetOutlineColor(new Color(0.72f, 0.78f, 0.93f, 0.72f), new Vector2(1f, -1f));
    }

    private void SetOutlineColor(Color color, Vector2 distance)
    {
        if (_frameOutline == null)
            return;

        _frameOutline.effectColor = color;
        _frameOutline.effectDistance = distance;
    }

    private void EnsureNameBanner()
    {
        var bannerRt = EnsureRectChild("NameBanner", _contentRoot);
        bannerRt.anchorMin = new Vector2(0f, 1f);
        bannerRt.anchorMax = new Vector2(1f, 1f);
        bannerRt.pivot = new Vector2(0.5f, 1f);
        bannerRt.anchoredPosition = Vector2.zero;
        bannerRt.sizeDelta = new Vector2(0f, 34f);

        _nameBannerImage = EnsureImage(bannerRt.gameObject, new Color(0.20f, 0.26f, 0.40f, 0.90f));
        _nameText = EnsureTextChild(
            bannerRt,
            "NameText",
            18,
            TextAnchor.MiddleLeft,
            new Color(0.95f, 0.98f, 1f, 1f),
            new Vector2(10f, 2f),
            new Vector2(-10f, -2f));

        _nameText.horizontalOverflow = HorizontalWrapMode.Overflow;
        _nameText.verticalOverflow = VerticalWrapMode.Truncate;
    }

    private void EnsureArtFrame()
    {
        var artRt = EnsureRectChild("ArtFrame", _contentRoot);
        artRt.anchorMin = new Vector2(0f, 1f);
        artRt.anchorMax = new Vector2(1f, 1f);
        artRt.pivot = new Vector2(0.5f, 1f);
        artRt.anchoredPosition = new Vector2(0f, -40f);
        artRt.sizeDelta = new Vector2(0f, 94f);

        _artFrameImage = EnsureImage(artRt.gameObject, new Color(0.10f, 0.12f, 0.16f, 0.76f));
    }

    private void EnsureDescriptionPanel()
    {
        var descRt = EnsureRectChild("DescriptionPanel", _contentRoot);
        descRt.anchorMin = new Vector2(0f, 0f);
        descRt.anchorMax = new Vector2(1f, 0f);
        descRt.pivot = new Vector2(0.5f, 0f);
        descRt.anchoredPosition = new Vector2(0f, 0f);
        descRt.sizeDelta = new Vector2(0f, 72f);

        _descriptionPanelImage = EnsureImage(descRt.gameObject, new Color(0.14f, 0.19f, 0.28f, 0.88f));
        _descriptionText = EnsureTextChild(
            descRt,
            "DescriptionText",
            14,
            TextAnchor.UpperLeft,
            new Color(0.88f, 0.92f, 1f, 1f),
            new Vector2(8f, 6f),
            new Vector2(-8f, -6f));

        _descriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        _descriptionText.verticalOverflow = VerticalWrapMode.Truncate;
    }

    private void EnsureRarityGem()
    {
        var gemRt = EnsureRectChild("RarityGem", _contentRoot);
        gemRt.anchorMin = new Vector2(1f, 1f);
        gemRt.anchorMax = new Vector2(1f, 1f);
        gemRt.pivot = new Vector2(1f, 1f);
        gemRt.anchoredPosition = new Vector2(-6f, -6f);
        gemRt.sizeDelta = new Vector2(16f, 16f);

        _rarityGemImage = EnsureImage(gemRt.gameObject, new Color(0.83f, 0.89f, 1f, 0.95f));
    }

    private void DisableLegacyRootText()
    {
        var texts = GetComponentsInChildren<Text>(includeInactive: true);
        for (int i = 0; i < texts.Length; i++)
        {
            var t = texts[i];
            if (t == null || t == _nameText || t == _descriptionText)
                continue;

            if (_contentRoot != null && t.transform.IsChildOf(_contentRoot))
                continue;

            t.gameObject.SetActive(false);
        }
    }

    private static RectTransform EnsureRectChild(string name, RectTransform parent)
    {
        var child = parent.Find(name) as RectTransform;
        if (child != null)
            return child;

        var go = new GameObject(name, typeof(RectTransform));
        child = go.GetComponent<RectTransform>();
        child.SetParent(parent, false);
        child.localScale = Vector3.one;
        return child;
    }

    private static Image EnsureImage(GameObject go, Color color)
    {
        var img = go.GetComponent<Image>();
        if (img == null)
            img = go.AddComponent<Image>();

        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    private void ApplySkinSprites()
    {
        ApplySprite(_frameImage, cardFrameSprite);
        ApplySprite(_nameBannerImage, nameBannerSprite);
        ApplySprite(_artFrameImage, artFrameSprite);
        ApplySprite(_descriptionPanelImage, descriptionPanelSprite);
        SetRarityTier(0);
    }

    private static void ApplySprite(Image target, Sprite sprite)
    {
        if (target == null || sprite == null)
            return;

        target.sprite = sprite;
        target.type = ShouldUseSliced(sprite) ? Image.Type.Sliced : Image.Type.Simple;
        target.preserveAspect = false;
    }

    private static bool ShouldUseSliced(Sprite sprite)
    {
        if (sprite == null)
            return false;

        var b = sprite.border;
        return b.x > 0f || b.y > 0f || b.z > 0f || b.w > 0f;
    }

    private static Text EnsureTextChild(
        RectTransform parent,
        string name,
        int fontSize,
        TextAnchor alignment,
        Color color,
        Vector2 offsetMin,
        Vector2 offsetMax)
    {
        var textRt = parent.Find(name) as RectTransform;
        if (textRt == null)
        {
            var textGo = new GameObject(name, typeof(RectTransform), typeof(Text));
            textRt = textGo.GetComponent<RectTransform>();
            textRt.SetParent(parent, false);
        }

        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = offsetMin;
        textRt.offsetMax = offsetMax;

        var txt = textRt.GetComponent<Text>();
        var builtInFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (builtInFont != null)
            txt.font = builtInFont;
        txt.fontSize = fontSize;
        txt.alignment = alignment;
        txt.color = color;
        txt.supportRichText = true;
        txt.raycastTarget = false;
        return txt;
    }
}
