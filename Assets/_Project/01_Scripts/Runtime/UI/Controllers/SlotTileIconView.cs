using System;
using System.Collections.Generic;
using Project.Core.Tiles;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class SlotTileIconView : MonoBehaviour, IPointerClickHandler
{
    [Header("Visual")]
    [SerializeField] private Image tileImage;
    [SerializeField] private Text fallbackText;
    [SerializeField] private TileSpriteSet spriteSet;

    [Header("Sheet Fallback")]
    [SerializeField] private Texture2D fallbackSheetTexture;
    [SerializeField] private string fallbackSheetResourcesPath = "";
    [SerializeField] private Vector2Int fallbackCellSize = new Vector2Int(64, 64);
    [SerializeField] private int fallbackSheetColumns = 10;
    [SerializeField] private bool fallbackStartsFromTopLeft = true;

    [Header("Tile Crop (inside each cell)")]
    [SerializeField] private bool fallbackUseInnerCrop = true;
    [SerializeField] private Vector2Int fallbackInnerSize = new Vector2Int(46, 62);
    [SerializeField] private Vector2Int fallbackInnerOffsetTopLeft = new Vector2Int(9, 1);

    [Header("Mahjong Layout")]
    [SerializeField] private bool fallbackUseMahjongDeckLayout = true;

    public int SlotIndex { get; private set; }
    public int TileId { get; private set; }

    private Action<int, int> _onLeftClicked;
    private Action<int> _onRightClicked;
    private Dictionary<int, Sprite> _sheetMap;

    private void Awake()
    {
        ResolveVisualReferences();
    }

    public void Bind(int slotIndex, int tileId, Action<int, int> onLeftClicked, Action<int> onRightClicked = null)
    {
        SlotIndex = slotIndex;
        TileId = tileId;

        _onLeftClicked = onLeftClicked;
        _onRightClicked = onRightClicked;

        SetTile(tileId);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _onLeftClicked?.Invoke(SlotIndex, TileId);
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            _onRightClicked?.Invoke(SlotIndex);
            return;
        }
    }

    private void ResolveVisualReferences()
    {
        if (tileImage == null)
        {
            var images = GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i] == null) continue;
                if (images[i].gameObject == gameObject) continue;
                tileImage = images[i];
                break;
            }

            if (tileImage == null)
                tileImage = GetComponent<Image>();
        }

        // The root Image in SlotTileIcon prefab is a legacy white panel.
        // Keep only the actual tile image renderer active to avoid covering the sprite.
        var rootImage = GetComponent<Image>();
        if (rootImage != null && tileImage != null && rootImage != tileImage)
        {
            rootImage.enabled = false;
            rootImage.raycastTarget = false;
        }

        if (fallbackText == null)
            fallbackText = GetComponentInChildren<Text>(true);
    }

    private void SetTile(int tileId)
    {
        if (spriteSet != null && spriteSet.TryGet(tileId, out var spr))
        {
            ApplySprite(spr);
            return;
        }

        if (TryGetSheetFallbackSprite(tileId, out var fallbackSprite))
        {
            ApplySprite(fallbackSprite);
            return;
        }

        if (tileImage != null)
        {
            tileImage.enabled = false;
            tileImage.sprite = null;
        }

        if (fallbackText != null)
        {
            fallbackText.gameObject.SetActive(true);
            fallbackText.text = (tileId % 100).ToString();
        }
    }

    private void ApplySprite(Sprite sprite)
    {
        if (tileImage != null)
        {
            tileImage.enabled = true;
            tileImage.sprite = sprite;
        }

        if (fallbackText != null)
            fallbackText.gameObject.SetActive(false);
    }

    private bool TryGetSheetFallbackSprite(int tileId, out Sprite sprite)
    {
        if (_sheetMap == null)
            BuildSheetFallbackMap();

        if (_sheetMap != null && _sheetMap.TryGetValue(tileId, out sprite) && sprite != null)
            return true;

        sprite = null;
        return false;
    }

    private void BuildSheetFallbackMap()
    {
        var texture = fallbackSheetTexture;
        if (texture == null && !string.IsNullOrWhiteSpace(fallbackSheetResourcesPath))
            texture = Resources.Load<Texture2D>(fallbackSheetResourcesPath);

        if (texture == null)
        {
            _sheetMap = new Dictionary<int, Sprite>(0);
            return;
        }

        int cw = Mathf.Max(1, fallbackCellSize.x);
        int ch = Mathf.Max(1, fallbackCellSize.y);
        int columns = Mathf.Max(1, fallbackSheetColumns);
        int totalRows = Mathf.Max(1, texture.height / ch);

        _sheetMap = new Dictionary<int, Sprite>(TileCatalog.AllTileIds.Count);

        for (int i = 0; i < TileCatalog.AllTileIds.Count; i++)
        {
            int tileId = TileCatalog.AllTileIds[i];
            if (!TryGetCellIndexForTile(tileId, out int cellIndex, columns))
                continue;

            int cellX = cellIndex % columns;
            int rowFromTop = cellIndex / columns;
            int cellY = fallbackStartsFromTopLeft
                ? totalRows - rowFromTop - 1
                : rowFromTop;

            float cellPx = cellX * cw;
            float cellPy = cellY * ch;
            if (cellY < 0 || cellPx + cw > texture.width || cellPy + ch > texture.height)
                continue;

            Rect rect;
            if (fallbackUseInnerCrop)
            {
                int iw = Mathf.Clamp(fallbackInnerSize.x, 1, cw);
                int ih = Mathf.Clamp(fallbackInnerSize.y, 1, ch);
                int ox = Mathf.Clamp(fallbackInnerOffsetTopLeft.x, 0, cw - iw);
                int oyTop = Mathf.Clamp(fallbackInnerOffsetTopLeft.y, 0, ch - ih);
                int oyBottom = ch - oyTop - ih;

                rect = new Rect(cellPx + ox, cellPy + oyBottom, iw, ih);
            }
            else
            {
                rect = new Rect(cellPx, cellPy, cw, ch);
            }

            var created = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f);
            _sheetMap[tileId] = created;
        }
    }

    private bool TryGetCellIndexForTile(int tileId, out int cellIndex, int columns)
    {
        if (!fallbackUseMahjongDeckLayout)
        {
            int idx = -1;
            var ids = TileCatalog.AllTileIds;
            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i] == tileId)
                {
                    idx = i;
                    break;
                }
            }

            if (idx >= 0)
            {
                cellIndex = idx;
                return true;
            }

            cellIndex = 0;
            return false;
        }

        int suit = tileId / 100;
        int rank = tileId % 100;

        if (rank <= 0)
        {
            cellIndex = 0;
            return false;
        }

        int rowFromTop;
        switch (suit)
        {
            case 1:
                if (rank > 9) { cellIndex = 0; return false; }
                rowFromTop = 2;
                cellIndex = (rowFromTop * columns) + (rank - 1);
                return true;
            case 2:
                if (rank > 9) { cellIndex = 0; return false; }
                rowFromTop = 1;
                cellIndex = (rowFromTop * columns) + (rank - 1);
                return true;
            case 3:
                if (rank > 9) { cellIndex = 0; return false; }
                rowFromTop = 0;
                cellIndex = (rowFromTop * columns) + (rank - 1);
                return true;
            case 4:
                if (rank > 7) { cellIndex = 0; return false; }
                rowFromTop = 3;
                cellIndex = (rowFromTop * columns) + (rank - 1);
                return true;
        }

        cellIndex = 0;
        return false;
    }
}
