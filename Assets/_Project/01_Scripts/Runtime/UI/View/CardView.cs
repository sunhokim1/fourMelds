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
    [SerializeField] private Sprite rarityGemDreamSprite;
    [SerializeField] private Sprite rarityFrameCommonSprite;
    [SerializeField] private Sprite rarityFrameRareSprite;
    [SerializeField] private Sprite rarityFrameEpicSprite;
    [SerializeField] private Sprite rarityFrameLegendarySprite;
    [SerializeField] private Sprite rarityFrameDreamSprite;
    [SerializeField] private bool useRarityTint = false;
    [Header("Typography")]
    [SerializeField] private Font preferredTextFont;

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
    private bool _usingSimpleLayout;
    private int _rarityTier;
    private Graphic _simpleCardGraphic;
    private RawImage _simpleCardRawImage;
    private static Font _cachedJuaFont;

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

        _contentRoot = EnsureRectChild("CardContent", rootRt, out bool contentCreated);
        bool hasSimpleLayout = HasSimpleLayout(_contentRoot);
        if (contentCreated || !hasSimpleLayout)
        {
            _contentRoot.anchorMin = Vector2.zero;
            _contentRoot.anchorMax = Vector2.one;
            _contentRoot.pivot = new Vector2(0.5f, 0.5f);
            _contentRoot.anchoredPosition = Vector2.zero;
            _contentRoot.offsetMin = new Vector2(BorderSize, BorderSize);
            _contentRoot.offsetMax = new Vector2(-BorderSize, -BorderSize);
        }
        EnsureManualLayoutContainer(_contentRoot);

        _usingSimpleLayout = TryBindSimpleLayout();
        if (!_usingSimpleLayout)
        {
            EnsureNameBanner();
            EnsureArtFrame();
            EnsureDescriptionPanel();
            EnsureRarityGem();
        }

        DisableLegacyRootText();
        ApplyPreferredTextFonts();
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
        Sprite gemLegendary,
        Sprite gemDream)
    {
        cardFrameSprite = frameSprite;
        nameBannerSprite = bannerSprite;
        artFrameSprite = artSprite;
        descriptionPanelSprite = descriptionSprite;
        rarityGemCommonSprite = gemCommon;
        rarityGemRareSprite = gemRare;
        rarityGemEpicSprite = gemEpic;
        rarityGemLegendarySprite = gemLegendary;
        rarityGemDreamSprite = gemDream;

        ApplySkinSprites();
    }

    public void SetRarityTier(int tier)
    {
        _rarityTier = Mathf.Clamp(tier, 0, 4);

        if (_rarityGemImage != null)
            _rarityGemImage.gameObject.SetActive(false);

        ApplyRarityVisual();
    }

    public void SetRarityFrameSprites(
        Sprite common,
        Sprite rare,
        Sprite epic,
        Sprite legendary,
        Sprite dream)
    {
        rarityFrameCommonSprite = common;
        rarityFrameRareSprite = rare;
        rarityFrameEpicSprite = epic;
        rarityFrameLegendarySprite = legendary;
        rarityFrameDreamSprite = dream;
        ApplyRarityVisual();
    }

    private void ApplyVisualState()
    {
        if (_frameImage == null)
            return;

        bool useArtColor = HasCustomFrameArt();

        if (!_interactable)
        {
            _frameImage.color = useArtColor ? new Color(1f, 1f, 1f, 0.55f) : _disabledColor;
            SetOutlineColor(new Color(0.35f, 0.38f, 0.48f, 0.45f), new Vector2(1f, -1f));
            return;
        }

        if (_dragging && _dragValid)
        {
            _frameImage.color = useArtColor ? Color.white : _dragValidColor;
            SetOutlineColor(_dragValidOutlineColor, new Vector2(3f, -3f));
            return;
        }

        if (_hovered || _dragging)
        {
            _frameImage.color = useArtColor ? Color.white : _hoverColor;
            SetOutlineColor(new Color(0.92f, 0.96f, 1f, 0.98f), new Vector2(2f, -2f));
            return;
        }

        _frameImage.color = useArtColor ? Color.white : _normalColor;
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
        var bannerRt = EnsureRectChild("NameBanner", _contentRoot, out bool created);
        if (created)
        {
            bannerRt.anchorMin = new Vector2(0f, 1f);
            bannerRt.anchorMax = new Vector2(1f, 1f);
            bannerRt.pivot = new Vector2(0.5f, 1f);
            bannerRt.anchoredPosition = Vector2.zero;
            bannerRt.sizeDelta = new Vector2(0f, 34f);
        }

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
        var artRt = EnsureRectChild("ArtFrame", _contentRoot, out bool created);
        if (created)
        {
            artRt.anchorMin = new Vector2(0f, 1f);
            artRt.anchorMax = new Vector2(1f, 1f);
            artRt.pivot = new Vector2(0.5f, 1f);
            artRt.anchoredPosition = new Vector2(0f, -40f);
            artRt.sizeDelta = new Vector2(0f, 94f);
        }

        _artFrameImage = EnsureImage(artRt.gameObject, new Color(0.10f, 0.12f, 0.16f, 0.76f));
    }

    private void EnsureDescriptionPanel()
    {
        var descRt = EnsureRectChild("DescriptionPanel", _contentRoot, out bool created);
        if (created)
        {
            descRt.anchorMin = new Vector2(0f, 0f);
            descRt.anchorMax = new Vector2(1f, 0f);
            descRt.pivot = new Vector2(0.5f, 0f);
            descRt.anchoredPosition = new Vector2(0f, 0f);
            descRt.sizeDelta = new Vector2(0f, 72f);
        }

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
        var gemRt = EnsureRectChild("RarityGem", _contentRoot, out bool created);
        if (created)
        {
            gemRt.anchorMin = new Vector2(1f, 1f);
            gemRt.anchorMax = new Vector2(1f, 1f);
            gemRt.pivot = new Vector2(1f, 1f);
            gemRt.anchoredPosition = new Vector2(-6f, -6f);
            gemRt.sizeDelta = new Vector2(16f, 16f);
        }

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
        return EnsureRectChild(name, parent, out _);
    }

    private static RectTransform EnsureRectChild(string name, RectTransform parent, out bool created)
    {
        var child = parent.Find(name) as RectTransform;
        if (child != null)
        {
            created = false;
            return child;
        }

        var go = new GameObject(name, typeof(RectTransform));
        child = go.GetComponent<RectTransform>();
        child.SetParent(parent, false);
        child.localScale = Vector3.one;
        created = true;
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

    private bool TryBindSimpleLayout()
    {
        if (_contentRoot == null)
            return false;

        var nameRt = _contentRoot.Find("NameText") as RectTransform;
        var cardRt = _contentRoot.Find("CardImage") as RectTransform;
        var descRt = _contentRoot.Find("DescriptionText") as RectTransform;

        if (nameRt == null || cardRt == null || descRt == null)
            return false;

        _nameText = GetOrAddText(nameRt.gameObject);
        _descriptionText = GetOrAddText(descRt.gameObject);
        _descriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        _descriptionText.verticalOverflow = VerticalWrapMode.Truncate;

        _simpleCardGraphic = cardRt.GetComponent<Graphic>();
        if (_simpleCardGraphic == null)
            _simpleCardGraphic = EnsureImage(cardRt.gameObject, Color.white);

        _simpleCardRawImage = cardRt.GetComponent<RawImage>();
        _artFrameImage = cardRt.GetComponent<Image>();
        _nameBannerImage = null;
        _descriptionPanelImage = null;
        _rarityGemImage = null;
        return true;
    }

    private static void EnsureManualLayoutContainer(RectTransform rt)
    {
        if (rt == null)
            return;

        RemoveIfExists<HorizontalLayoutGroup>(rt.gameObject);
        RemoveIfExists<VerticalLayoutGroup>(rt.gameObject);
        RemoveIfExists<GridLayoutGroup>(rt.gameObject);
        RemoveIfExists<ContentSizeFitter>(rt.gameObject);
    }

    private static bool HasSimpleLayout(RectTransform root)
    {
        if (root == null)
            return false;

        return root.Find("NameText") != null
            && root.Find("CardImage") != null
            && root.Find("DescriptionText") != null;
    }

    private static void RemoveIfExists<T>(GameObject go) where T : Component
    {
        var comp = go.GetComponent<T>();
        if (comp == null)
            return;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            DestroyImmediate(comp);
        else
            Destroy(comp);
#else
        Destroy(comp);
#endif
    }

    private void ApplySkinSprites()
    {
        ApplySprite(_frameImage, cardFrameSprite);
        ApplySprite(_nameBannerImage, nameBannerSprite);
        ApplySprite(_artFrameImage, artFrameSprite);
        ApplySprite(_descriptionPanelImage, descriptionPanelSprite);
        ApplyRarityVisual();
    }

    private void ApplyRarityVisual()
    {
        var rarityFrame = ResolveFrameSpriteForTier();
        if (_frameImage != null)
            ApplySprite(_frameImage, rarityFrame != null ? rarityFrame : cardFrameSprite);

        if (_usingSimpleLayout && _simpleCardRawImage != null && _simpleCardRawImage.texture == null)
        {
            _simpleCardRawImage.color = new Color(1f, 1f, 1f, 0f);
            return;
        }

        if (!useRarityTint)
        {
            if (_usingSimpleLayout && _simpleCardGraphic != null)
                _simpleCardGraphic.color = Color.white;
            else if (_artFrameImage != null)
                _artFrameImage.color = Color.white;
            return;
        }

        Color tint = _rarityTier switch
        {
            4 => new Color(0.88f, 0.84f, 0.96f, 1f),
            3 => new Color(0.78f, 0.70f, 0.56f, 1f),
            2 => new Color(0.78f, 0.72f, 0.86f, 1f),
            1 => new Color(0.84f, 0.90f, 0.98f, 1f),
            _ => Color.white
        };

        if (_usingSimpleLayout && _simpleCardGraphic != null)
        {
            _simpleCardGraphic.color = tint;
            return;
        }

        if (_artFrameImage != null)
            _artFrameImage.color = tint;
    }

    private Sprite ResolveFrameSpriteForTier()
    {
        return _rarityTier switch
        {
            4 => rarityFrameDreamSprite,
            3 => rarityFrameLegendarySprite,
            2 => rarityFrameEpicSprite,
            1 => rarityFrameRareSprite,
            _ => rarityFrameCommonSprite
        };
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

    private bool HasCustomFrameArt()
    {
        return cardFrameSprite != null || (_frameImage != null && _frameImage.sprite != null);
    }

    private void ApplyPreferredTextFonts()
    {
        Font target = ResolvePreferredTextFont();
        if (target == null)
            return;

        if (_nameText != null)
            _nameText.font = target;
        if (_descriptionText != null)
            _descriptionText.font = target;
    }

    private Font ResolvePreferredTextFont()
    {
        if (preferredTextFont != null)
            return preferredTextFont;

        if (_cachedJuaFont == null)
            _cachedJuaFont = Resources.Load<Font>("Fonts/Jua-Regular");
        if (_cachedJuaFont != null)
            return _cachedJuaFont;

        return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
        bool created = false;
        if (textRt == null)
        {
            var textGo = new GameObject(name, typeof(RectTransform), typeof(Text));
            textRt = textGo.GetComponent<RectTransform>();
            textRt.SetParent(parent, false);
            created = true;
        }

        if (created)
        {
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = offsetMin;
            textRt.offsetMax = offsetMax;
        }

        var txt = textRt.GetComponent<Text>();
        if (txt == null)
            txt = textRt.gameObject.AddComponent<Text>();
        if (txt.font == null)
        {
            var builtInFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (builtInFont != null)
                txt.font = builtInFont;
        }
        txt.fontSize = fontSize;
        txt.alignment = alignment;
        txt.color = color;
        txt.supportRichText = true;
        txt.raycastTarget = false;
        return txt;
    }

    private static Text GetOrAddText(GameObject go)
    {
        var txt = go.GetComponent<Text>();
        if (txt == null)
        {
            txt = go.AddComponent<Text>();
            if (txt.font == null)
            {
                var builtInFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (builtInFont != null)
                    txt.font = builtInFont;
            }
            txt.fontSize = 14;
            txt.alignment = TextAnchor.UpperLeft;
            txt.color = Color.white;
            txt.supportRichText = true;
            txt.raycastTarget = false;
        }
        return txt;
    }
}
