using FourMelds.Cards;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardAuthoringAsset))]
public sealed class CardAuthoringAssetEditor : Editor
{
    private SerializedProperty _cardId;
    private SerializedProperty _cardName;
    private SerializedProperty _cardImage;
    private SerializedProperty _rarity;
    private SerializedProperty _suitTheme;
    private SerializedProperty _useAutoName;
    private SerializedProperty _useAutoDescription;
    private SerializedProperty _manualName;
    private SerializedProperty _manualDescription;
    private SerializedProperty _effects;

    private void OnEnable()
    {
        _cardId = serializedObject.FindProperty("cardId");
        _cardName = serializedObject.FindProperty("cardName");
        _cardImage = serializedObject.FindProperty("cardImage");
        _rarity = serializedObject.FindProperty("rarity");
        _suitTheme = serializedObject.FindProperty("suitTheme");
        _useAutoName = serializedObject.FindProperty("useAutoName");
        _useAutoDescription = serializedObject.FindProperty("useAutoDescription");
        _manualName = serializedObject.FindProperty("manualName");
        _manualDescription = serializedObject.FindProperty("manualDescription");
        _effects = serializedObject.FindProperty("effects");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_cardId);
        EditorGUILayout.PropertyField(_cardName);

        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("Visual", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_cardImage);
        EditorGUILayout.PropertyField(_rarity);
        EditorGUILayout.PropertyField(_suitTheme);

        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_useAutoName, new GUIContent("Use Auto Name"));
        if (!_useAutoName.boolValue)
            EditorGUILayout.PropertyField(_manualName);
        EditorGUILayout.PropertyField(_useAutoDescription, new GUIContent("Use Auto Description"));
        if (!_useAutoDescription.boolValue)
            EditorGUILayout.PropertyField(_manualDescription);

        EditorGUILayout.Space(6f);
        DrawEffectsList();

        EditorGUILayout.Space(8f);
        DrawPreview();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawEffectsList()
    {
        EditorGUILayout.LabelField("Effects (Order Matters)", EditorStyles.boldLabel);
        if (_effects == null)
            return;

        for (int i = 0; i < _effects.arraySize; i++)
        {
            var element = _effects.GetArrayElementAtIndex(i);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"Effect #{i + 1}", EditorStyles.miniBoldLabel);

                var action = element.FindPropertyRelative("action");
                var suit = element.FindPropertyRelative("suit");
                var count = element.FindPropertyRelative("count");
                var minRank = element.FindPropertyRelative("minRank");
                var maxRank = element.FindPropertyRelative("maxRank");
                var preferHonorFirst = element.FindPropertyRelative("preferHonorFirst");

                EditorGUILayout.PropertyField(action);
                EditorGUILayout.PropertyField(suit);
                EditorGUILayout.PropertyField(count, new GUIContent("Count"));

                if ((CardEffectSuitType)suit.enumValueIndex == CardEffectSuitType.Number)
                {
                    EditorGUILayout.PropertyField(minRank);
                    EditorGUILayout.PropertyField(maxRank);
                }

                if ((CardEffectActionType)action.enumValueIndex == CardEffectActionType.Tsumo &&
                    (CardEffectSuitType)suit.enumValueIndex == CardEffectSuitType.Random)
                {
                    EditorGUILayout.PropertyField(preferHonorFirst, new GUIContent("Prefer Honor First"));
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.enabled = i > 0;
                    if (GUILayout.Button("Up"))
                        _effects.MoveArrayElement(i, i - 1);

                    GUI.enabled = i < _effects.arraySize - 1;
                    if (GUILayout.Button("Down"))
                        _effects.MoveArrayElement(i, i + 1);

                    GUI.enabled = true;
                    if (GUILayout.Button("Remove"))
                    {
                        _effects.DeleteArrayElementAtIndex(i);
                        break;
                    }
                }
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Add Tsumo Effect"))
                AddEffect(CardEffectActionType.Tsumo);

            if (GUILayout.Button("Add Exchange Effect"))
                AddEffect(CardEffectActionType.Exchange);
        }
    }

    private void AddEffect(CardEffectActionType actionType)
    {
        int index = _effects.arraySize;
        _effects.InsertArrayElementAtIndex(index);
        var element = _effects.GetArrayElementAtIndex(index);
        if (element == null)
            return;

        element.FindPropertyRelative("action").enumValueIndex = (int)actionType;
        element.FindPropertyRelative("suit").enumValueIndex = (int)CardEffectSuitType.Random;
        element.FindPropertyRelative("count").intValue = 1;
        element.FindPropertyRelative("minRank").intValue = 2;
        element.FindPropertyRelative("maxRank").intValue = 8;
        element.FindPropertyRelative("preferHonorFirst").boolValue = false;
    }

    private void DrawPreview()
    {
        var asset = target as CardAuthoringAsset;
        if (asset == null)
            return;

        string namePreview = asset.GetResolvedName();
        string descPreview = asset.GetResolvedDescription();
        EditorGUILayout.HelpBox($"자동 이름: {namePreview}\n\n설명 프리뷰:\n{descPreview}", MessageType.None);
    }
}
