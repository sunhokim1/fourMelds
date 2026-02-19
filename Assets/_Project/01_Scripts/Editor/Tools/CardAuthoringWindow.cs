using System;
using System.Collections.Generic;
using System.IO;
using FourMelds.Cards;
using UnityEditor;
using UnityEngine;

public sealed class CardAuthoringWindow : EditorWindow
{
    private const string AssetSaveFolder = "Assets/_Project/Resources/CardAuthoring";
    private static readonly string[] RarityLabels = { "0 Common", "1 Rare", "2 Epic", "3 Legendary", "4 Dream" };
    private static readonly string[] RaritySpritePaths =
    {
        "Assets/_Project/04_Art/UI/card_common.png",
        "Assets/_Project/04_Art/UI/card_rare.png",
        "Assets/_Project/04_Art/UI/card_epic.png",
        "Assets/_Project/04_Art/UI/card_legendary.png",
        "Assets/_Project/04_Art/UI/card_dream.png",
    };

    private string _cardId = "card.new";
    private string _cardName = "새 카드";
    private Sprite _cardImage;
    private int _rarity;
    private int _suitTheme;
    private bool _autoImageByRarity = true;
    private bool _useAutoName = false;
    private bool _useAutoDescription = true;
    private string _manualName = string.Empty;
    private string _manualDescription = string.Empty;

    private readonly List<CardEffectTemplate> _effects = new List<CardEffectTemplate>();
    private Vector2 _scroll;

    [MenuItem("Window/FourMelds/Card Authoring Studio")]
    public static void Open()
    {
        var window = GetWindow<CardAuthoringWindow>("Card Authoring");
        window.minSize = new Vector2(520f, 620f);
    }

    private void OnEnable()
    {
        if (_effects.Count == 0)
            _effects.Add(new CardEffectTemplate());
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6f);
        EditorGUILayout.LabelField("Card Authoring Studio", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("여기서 카드 생성 후 에셋 저장과 카드 데이터(JSON) 반영을 한 번에 처리합니다.", MessageType.None);

        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        DrawIdentitySection();
        DrawVisualSection();
        DrawTextSection();
        DrawEffectsSection();
        DrawPreviewSection();
        DrawActionsSection();

        EditorGUILayout.EndScrollView();
    }

    private void DrawIdentitySection()
    {
        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
        _cardId = EditorGUILayout.TextField("Card Id", _cardId);
        _cardName = EditorGUILayout.TextField("Card Name", _cardName);
    }

    private void DrawVisualSection()
    {
        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("Visual", EditorStyles.boldLabel);
        int nextRarity = EditorGUILayout.IntPopup("Rarity", Mathf.Clamp(_rarity, 0, 4), RarityLabels, new[] { 0, 1, 2, 3, 4 });
        _autoImageByRarity = EditorGUILayout.ToggleLeft("Auto Apply Rarity Frame Sprite", _autoImageByRarity);
        if (nextRarity != _rarity)
        {
            _rarity = nextRarity;
            if (_autoImageByRarity)
                _cardImage = LoadRaritySprite(_rarity) ?? _cardImage;
        }

        _cardImage = (Sprite)EditorGUILayout.ObjectField("Card Image", _cardImage, typeof(Sprite), false);
        _suitTheme = EditorGUILayout.IntSlider("Suit Theme", Mathf.Clamp(_suitTheme, 0, 4), 0, 4);
    }

    private void DrawTextSection()
    {
        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);

        _useAutoName = EditorGUILayout.ToggleLeft("Use Auto Name", _useAutoName);
        if (!_useAutoName)
            _manualName = EditorGUILayout.TextField("Manual Name", _manualName);

        _useAutoDescription = EditorGUILayout.ToggleLeft("Use Auto Description", _useAutoDescription);
        if (!_useAutoDescription)
            _manualDescription = EditorGUILayout.TextArea(_manualDescription, GUILayout.MinHeight(64f));
    }

    private void DrawEffectsSection()
    {
        EditorGUILayout.Space(6f);
        EditorGUILayout.LabelField("Effects (Order Matters)", EditorStyles.boldLabel);

        for (int i = 0; i < _effects.Count; i++)
        {
            CardEffectTemplate e = _effects[i] ?? new CardEffectTemplate();
            _effects[i] = e;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"Effect #{i + 1}", EditorStyles.miniBoldLabel);

                e.action = (CardEffectActionType)EditorGUILayout.EnumPopup("Action", e.action);
                e.suit = (CardEffectSuitType)EditorGUILayout.EnumPopup("Suit", e.suit);
                e.count = Mathf.Max(1, EditorGUILayout.IntField("Count", e.count));

                if (e.suit == CardEffectSuitType.Number)
                {
                    e.minRank = EditorGUILayout.IntField("Min Rank", e.minRank);
                    e.maxRank = EditorGUILayout.IntField("Max Rank", e.maxRank);
                }

                if (e.action == CardEffectActionType.Tsumo && e.suit == CardEffectSuitType.Random)
                    e.preferHonorFirst = EditorGUILayout.ToggleLeft("Prefer Honor First", e.preferHonorFirst);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.enabled = i > 0;
                    if (GUILayout.Button("Up"))
                        SwapEffects(i, i - 1);

                    GUI.enabled = i < _effects.Count - 1;
                    if (GUILayout.Button("Down"))
                        SwapEffects(i, i + 1);

                    GUI.enabled = true;
                    if (GUILayout.Button("Remove"))
                    {
                        _effects.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Add Tsumo"))
                _effects.Add(new CardEffectTemplate { action = CardEffectActionType.Tsumo, suit = CardEffectSuitType.Random, count = 1 });

            if (GUILayout.Button("Add Exchange"))
                _effects.Add(new CardEffectTemplate { action = CardEffectActionType.Exchange, suit = CardEffectSuitType.Random, count = 1 });
        }
    }

    private void DrawPreviewSection()
    {
        EditorGUILayout.Space(6f);
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
        string previewName = ResolvePreviewName();
        string previewDesc = ResolvePreviewDescription();
        EditorGUILayout.HelpBox($"이름: {previewName}\n\n설명:\n{previewDesc}", MessageType.None);
    }

    private void DrawActionsSection()
    {
        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Create Asset Only", GUILayout.Height(28f)))
                CreateAssetAndOptionallySaveData(saveToJson: false);

            if (GUILayout.Button("Create + Save To Card Data", GUILayout.Height(28f)))
                CreateAssetAndOptionallySaveData(saveToJson: true);
        }
    }

    private void CreateAssetAndOptionallySaveData(bool saveToJson)
    {
        string resolvedName = ResolvePreviewName();
        if (string.IsNullOrWhiteSpace(resolvedName))
        {
            EditorUtility.DisplayDialog("카드 이름 필요", "카드 이름이 비어 있습니다. Card Name 또는 Manual Name을 입력하세요.", "확인");
            return;
        }

        var asset = BuildTempAsset();
        if (string.IsNullOrWhiteSpace(asset.cardId))
            asset.cardId = $"card.{DateTime.Now:yyyyMMddHHmmss}";

        EnsureFolderExists(AssetSaveFolder);
        string safeName = MakeSafeFileName(string.IsNullOrWhiteSpace(asset.cardId) ? "card_new" : asset.cardId);
        string path = AssetDatabase.GenerateUniqueAssetPath($"{AssetSaveFolder}/{safeName}.asset");
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        if (saveToJson)
        {
            var def = asset.ToPrimaryDefinition();
            SaveOrUpdateDefinition(def);
        }

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        Debug.Log($"[CardAuthoringWindow] Created: {path}" + (saveToJson ? " + updated card data JSON" : string.Empty));
    }

    private CardAuthoringAsset BuildTempAsset()
    {
        var asset = CreateInstance<CardAuthoringAsset>();
        asset.cardId = _cardId;
        asset.cardName = _cardName;
        asset.cardImage = _cardImage != null ? _cardImage : (_autoImageByRarity ? LoadRaritySprite(_rarity) : null);
        asset.rarity = Mathf.Clamp(_rarity, 0, 4);
        asset.suitTheme = Mathf.Clamp(_suitTheme, 0, 4);
        asset.useAutoName = _useAutoName;
        asset.useAutoDescription = _useAutoDescription;
        asset.manualName = _manualName;
        asset.manualDescription = _manualDescription;
        asset.effects = new List<CardEffectTemplate>(_effects.Count);
        for (int i = 0; i < _effects.Count; i++)
        {
            var e = _effects[i];
            if (e == null)
                continue;
            asset.effects.Add(new CardEffectTemplate
            {
                action = e.action,
                suit = e.suit,
                count = Mathf.Max(1, e.count),
                minRank = e.minRank,
                maxRank = e.maxRank,
                preferHonorFirst = e.preferHonorFirst
            });
        }

        return asset;
    }

    private string ResolvePreviewName()
    {
        if (!_useAutoName && !string.IsNullOrWhiteSpace(_manualName))
            return _manualName.Trim();
        if (!string.IsNullOrWhiteSpace(_cardName))
            return _cardName.Trim();
        return CardEffectTextComposer.BuildName(_effects);
    }

    private string ResolvePreviewDescription()
    {
        if (!_useAutoDescription && !string.IsNullOrWhiteSpace(_manualDescription))
            return _manualDescription.Trim();
        return CardEffectTextComposer.BuildDescription(_effects);
    }

    private static void SaveOrUpdateDefinition(CardDefinition newDef)
    {
        if (newDef == null || string.IsNullOrWhiteSpace(newDef.id))
        {
            Debug.LogError("[CardAuthoringWindow] Cannot save card data: invalid id.");
            return;
        }

        string dir = Path.Combine(Application.dataPath, "_Project/Resources/CardDefinitions/Cards");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string safeName = MakeSafeFileName(newDef.id);
        string fullPath = Path.Combine(dir, $"{safeName}.json");
        string json = JsonUtility.ToJson(newDef, true);
        File.WriteAllText(fullPath, json);
        AssetDatabase.Refresh();
    }

    private static void EnsureFolderExists(string folderPath)
    {
        string[] split = folderPath.Split('/');
        if (split.Length < 2 || split[0] != "Assets")
            return;

        string current = "Assets";
        for (int i = 1; i < split.Length; i++)
        {
            string next = split[i];
            string candidate = $"{current}/{next}";
            if (!AssetDatabase.IsValidFolder(candidate))
                AssetDatabase.CreateFolder(current, next);
            current = candidate;
        }
    }

    private static string MakeSafeFileName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "card_new";

        char[] invalid = Path.GetInvalidFileNameChars();
        var chars = input.Trim().ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (Array.IndexOf(invalid, chars[i]) >= 0 || chars[i] == '/' || chars[i] == '\\')
                chars[i] = '_';
        }

        return new string(chars);
    }

    private static Sprite LoadRaritySprite(int rarity)
    {
        int idx = Mathf.Clamp(rarity, 0, 4);
        if (idx < 0 || idx >= RaritySpritePaths.Length)
            return null;
        return AssetDatabase.LoadAssetAtPath<Sprite>(RaritySpritePaths[idx]);
    }

    private void SwapEffects(int a, int b)
    {
        if (a < 0 || b < 0 || a >= _effects.Count || b >= _effects.Count || a == b)
            return;

        var t = _effects[a];
        _effects[a] = _effects[b];
        _effects[b] = t;
    }
}
