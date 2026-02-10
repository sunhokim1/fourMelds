using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MeldSlotView : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField] private Text infoText;

    [Header("Tile Container")]
    [SerializeField] private Transform tileRoot;
    [SerializeField] private SlotTileIconView slotTileIconPrefab;
    [SerializeField] private float tileHorizontalSpacing = 12f;
    [SerializeField] private float minRuntimeTileHorizontalSpacing = 16f;

    [Header("Selection Visual (Optional)")]
    [SerializeField] private Image highlight;
    [SerializeField] private Color normalBgColor = new Color(1f, 1f, 1f, 0.392f);
    [SerializeField] private Color selectedBgColor = new Color(0.45f, 0.65f, 0.95f, 0.55f);
    [SerializeField] private Color normalOutlineColor = new Color(0f, 0f, 0f, 0f);
    [SerializeField] private Color selectedOutlineColor = new Color(0.8f, 0.9f, 1f, 0.95f);

    public Action<int> OnSlotClicked;                 // 좌클릭: 슬롯 선택
    public Action<int, int> OnSlotTileClicked;        // 좌클릭: 타일 1개 되돌리기
    public Action<int> OnSlotRightClicked;            // 우클릭: 슬롯 전체 비우기

    private int _slotIndex;
    private readonly List<SlotTileIconView> _active = new();
    private readonly Stack<SlotTileIconView> _pool = new();
    private Image _rootImage;
    private Outline _outline;
    private bool _isSelected;
    private bool _isPendingPlacement;
    private string _pendingLabel;
    private bool _isHeadSlot;

    private void Awake()
    {
        _rootImage = GetComponent<Image>();
        _outline = GetComponent<Outline>();
        EnsureTileRootLayout();
        ApplySelectedVisual();
    }

    public void SetIndex(int index) => _slotIndex = index;

    public void SetSelected(bool selected)
    {
        _isSelected = selected;

        if (highlight != null)
            highlight.enabled = selected;

        ApplySelectedVisual();
    }

    public void SetPendingPlacement(bool pending, string pendingLabel)
    {
        _isPendingPlacement = pending;
        _pendingLabel = pendingLabel;
    }

    public void BindTiles(int slotIndex, IReadOnlyList<int> tiles)
    {
        _isHeadSlot = false;
        _slotIndex = slotIndex;
        ClearTilesOnly();

        int count = tiles != null ? tiles.Count : 0;
        string baseText = count == 0 ? "(empty)" : $"(building) {count}/3";
        if (_isPendingPlacement && !string.IsNullOrWhiteSpace(_pendingLabel))
            infoText.text = $"{baseText}  [Pending: {_pendingLabel}]";
        else
            infoText.text = baseText;

        if (tiles == null) return;

        if (tileRoot == null || slotTileIconPrefab == null)
        {
            Debug.LogError("[MeldSlotView] tileRoot or slotTileIconPrefab missing");
            return;
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            int tileId = tiles[i];
            var icon = GetOrCreateIcon();

            // ✅ 좌클릭: 1개 빼기 / 우클릭: 슬롯 전체 비우기
            icon.Bind(
                slotIndex,
                tileId,
                onLeftClicked: HandleTileLeftClicked,
                onRightClicked: HandleSlotRightClickedFromTile
            );

            _active.Add(icon);
        }

        if (tileRoot is RectTransform tileRt)
            LayoutRebuilder.MarkLayoutForRebuild(tileRt);
    }

    public void BindHeadTile(int tileId)
    {
        _isHeadSlot = true;
        _slotIndex = -1;
        _isPendingPlacement = false;
        _pendingLabel = null;
        SetSelected(false);
        ClearTilesOnly();

        infoText.text = tileId == 0 ? "머리 (empty)" : "머리";

        if (tileId == 0)
            return;

        if (tileRoot == null || slotTileIconPrefab == null)
        {
            Debug.LogError("[MeldSlotView] tileRoot or slotTileIconPrefab missing");
            return;
        }

        for (int i = 0; i < 2; i++)
        {
            var icon = GetOrCreateIcon();
            icon.Bind(
                slotIndex: -1,
                tileId: tileId,
                onLeftClicked: null,
                onRightClicked: null
            );
            _active.Add(icon);
        }

        if (tileRoot is RectTransform tileRt)
            LayoutRebuilder.MarkLayoutForRebuild(tileRt);
    }

    private void HandleTileLeftClicked(int slotIndex, int tileId)
    {
        OnSlotTileClicked?.Invoke(slotIndex, tileId);
    }

    private void HandleSlotRightClickedFromTile(int slotIndex)
    {
        OnSlotRightClicked?.Invoke(slotIndex);
    }

    public void Clear()
    {
        infoText.text = _isPendingPlacement && !string.IsNullOrWhiteSpace(_pendingLabel)
            ? $"(empty)  [Pending: {_pendingLabel}]"
            : "(empty)";
        ClearTilesOnly();
    }

    private void ClearTilesOnly()
    {
        for (int i = 0; i < _active.Count; i++)
        {
            var icon = _active[i];
            if (icon == null) continue;
            icon.gameObject.SetActive(false);
            _pool.Push(icon);
        }

        _active.Clear();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isHeadSlot)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnSlotClicked?.Invoke(_slotIndex);
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnSlotRightClicked?.Invoke(_slotIndex);
            return;
        }
    }

    private void ApplySelectedVisual()
    {
        if (_rootImage != null)
            _rootImage.color = _isSelected ? selectedBgColor : normalBgColor;

        if (_outline != null)
            _outline.effectColor = _isSelected ? selectedOutlineColor : normalOutlineColor;
    }

    private SlotTileIconView GetOrCreateIcon()
    {
        SlotTileIconView icon;
        if (_pool.Count > 0)
        {
            icon = _pool.Pop();
            icon.transform.SetParent(tileRoot, false);
            icon.gameObject.SetActive(true);
        }
        else
        {
            icon = Instantiate(slotTileIconPrefab, tileRoot);
        }

        icon.transform.SetAsLastSibling();
        return icon;
    }

    private void EnsureTileRootLayout()
    {
        if (tileRoot == null)
            return;

        var hlg = tileRoot.GetComponent<HorizontalLayoutGroup>();
        if (hlg == null)
            hlg = tileRoot.gameObject.AddComponent<HorizontalLayoutGroup>();

        hlg.spacing = Mathf.Max(tileHorizontalSpacing, minRuntimeTileHorizontalSpacing);
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.childScaleWidth = false;
        hlg.childScaleHeight = false;
    }
}
