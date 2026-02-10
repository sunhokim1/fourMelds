using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandTilesView : MonoBehaviour
{
    private static readonly Vector2 MinRuntimeCellSize = new Vector2(56f, 84f);
    private const float MinRuntimeTileSpacing = 14f;
    private const float MinRuntimeLineSpacing = 16f;
    private const float MinRuntimeRowSpacing = 16f;
    private const float MinRuntimeRowPaddingY = 10f;

    [Header("Tile Prefab")]
    [SerializeField] private TileView tilePrefab;

    [Header("Suit Row Viewports")]
    [SerializeField] private RectTransform manzuRoot;
    [SerializeField] private RectTransform souzuRoot;
    [SerializeField] private RectTransform pinzuRoot;
    [SerializeField] private RectTransform honorRoot;

    [Header("Tile Layout")]
    [SerializeField] private Vector2 tileCellSize = new Vector2(56f, 84f);
    [SerializeField] private float tileSpacing = 12f;
    [SerializeField] private float lineSpacing = 14f;
    [SerializeField] private int tilesPerLine = 5;

    [Header("Rows")]
    [SerializeField] private float rowSpacing = 14f;
    [SerializeField] private float rowPaddingX = 6f;
    [SerializeField] private float rowPaddingY = 8f;
    [SerializeField] private bool showRowBackground = true;
    [SerializeField] private Color rowBackgroundColor = new Color(0f, 0f, 0f, 0.12f);
    [SerializeField] private bool forceTopLeftFill = true;

    private readonly List<TileView> _active = new();
    private readonly Stack<TileView> _pool = new();
    private readonly Dictionary<RectTransform, RectTransform> _contentByRow = new();

    private readonly List<int> _manzu = new();
    private readonly List<int> _souzu = new();
    private readonly List<int> _pinzu = new();
    private readonly List<int> _honor = new();

    private void Awake()
    {
        EnsurePanelLayout();

        ConfigureRow(manzuRoot, "ManzuContent");
        ConfigureRow(souzuRoot, "SouzuContent");
        ConfigureRow(pinzuRoot, "PinzuContent");
        ConfigureRow(honorRoot, "HonorContent");
    }

    public void Render(IReadOnlyList<int> handTiles)
    {
        Clear();

        _manzu.Clear();
        _souzu.Clear();
        _pinzu.Clear();
        _honor.Clear();

        if (handTiles == null)
            return;

        for (int i = 0; i < handTiles.Count; i++)
        {
            int tileId = handTiles[i];
            switch (tileId / 100)
            {
                case 1: _manzu.Add(tileId); break;
                case 2: _souzu.Add(tileId); break;
                case 3: _pinzu.Add(tileId); break;
                default: _honor.Add(tileId); break;
            }
        }

        _manzu.Sort();
        _souzu.Sort();
        _pinzu.Sort();
        _honor.Sort();

        RenderGroup(manzuRoot, _manzu);
        RenderGroup(souzuRoot, _souzu);
        RenderGroup(pinzuRoot, _pinzu);
        RenderGroup(honorRoot, _honor);

    }

    private void RenderGroup(RectTransform row, List<int> tiles)
    {
        if (row == null || tilePrefab == null)
        {
            Debug.LogError("[HandTilesView] Missing row or tilePrefab");
            return;
        }

        if (!_contentByRow.TryGetValue(row, out var content) || content == null)
        {
            Debug.LogError("[HandTilesView] Missing row content");
            return;
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            var tv = GetOrCreateTile(content);
            tv.Bind(tiles[i]);
            var rt = tv.transform as RectTransform;
            if (rt != null)
                rt.localScale = Vector3.one;
            _active.Add(tv);
        }
    }

    private TileView GetOrCreateTile(Transform parent)
    {
        TileView tv;
        if (_pool.Count > 0)
        {
            tv = _pool.Pop();
            tv.transform.SetParent(parent, false);
            tv.gameObject.SetActive(true);
        }
        else
        {
            tv = Instantiate(tilePrefab, parent);
        }

        tv.transform.SetAsLastSibling();
        return tv;
    }

    private void EnsurePanelLayout()
    {
        var vlg = GetComponent<VerticalLayoutGroup>();
        if (vlg == null)
            vlg = gameObject.AddComponent<VerticalLayoutGroup>();

        vlg.padding = new RectOffset(0, 0, 0, 0);
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.spacing = Mathf.Max(rowSpacing, MinRuntimeRowSpacing);
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = true;

        var hlg = GetComponent<HorizontalLayoutGroup>();
        if (hlg != null)
            Destroy(hlg);

        var fitter = GetComponent<ContentSizeFitter>();
        if (fitter != null)
            Destroy(fitter);
    }

    private void ConfigureRow(RectTransform row, string contentName)
    {
        if (row == null)
            return;

        var le = row.GetComponent<LayoutElement>();
        if (le == null)
            le = row.gameObject.AddComponent<LayoutElement>();
        le.flexibleHeight = 1f;
        le.flexibleWidth = 1f;

        if (showRowBackground)
        {
            var bg = row.GetComponent<Image>();
            if (bg == null)
                bg = row.gameObject.AddComponent<Image>();
            bg.color = rowBackgroundColor;
            bg.raycastTarget = false;
        }

        var mask = row.GetComponent<RectMask2D>();
        if (mask == null)
            mask = row.gameObject.AddComponent<RectMask2D>();

        RectTransform content = FindOrCreateContent(row, contentName);
        ConfigureContent(content);
        _contentByRow[row] = content;

        var scroll = row.GetComponent<ScrollRect>();
        if (scroll == null)
            scroll = row.gameObject.AddComponent<ScrollRect>();

        scroll.viewport = row;
        scroll.content = content;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.inertia = true;
        scroll.scrollSensitivity = 30f;

        var wheel = row.GetComponent<HorizontalWheelScroll>();
        if (wheel == null)
            wheel = row.gameObject.AddComponent<HorizontalWheelScroll>();
        wheel.Bind(scroll);
    }

    private RectTransform FindOrCreateContent(RectTransform row, string name)
    {
        for (int i = 0; i < row.childCount; i++)
        {
            if (row.GetChild(i) is RectTransform rt && rt.name == name)
                return rt;
        }

        var go = new GameObject(name, typeof(RectTransform));
        var content = go.GetComponent<RectTransform>();
        content.SetParent(row, false);
        return content;
    }

    private void ConfigureContent(RectTransform content)
    {
        if (forceTopLeftFill)
        {
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(0f, 1f);
            content.pivot = new Vector2(0f, 1f);
        }
        else
        {
            content.anchorMin = new Vector2(0f, 0f);
            content.anchorMax = new Vector2(0f, 1f);
            content.pivot = new Vector2(0f, 0.5f);
        }
        content.anchoredPosition = Vector2.zero;

        var grid = content.GetComponent<GridLayoutGroup>();
        if (grid == null)
            grid = content.gameObject.AddComponent<GridLayoutGroup>();

        // Runtime-applied layout values to avoid prefab/scene override mismatch.
        int appliedRowPadY = Mathf.RoundToInt(Mathf.Max(rowPaddingY, MinRuntimeRowPaddingY));
        grid.padding = new RectOffset((int)rowPaddingX, (int)rowPaddingX, appliedRowPadY, appliedRowPadY);
        grid.childAlignment = forceTopLeftFill ? TextAnchor.UpperLeft : TextAnchor.MiddleLeft;
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.cellSize = new Vector2(
            Mathf.Max(tileCellSize.x, MinRuntimeCellSize.x),
            Mathf.Max(tileCellSize.y, MinRuntimeCellSize.y)
        );
        grid.spacing = new Vector2(
            Mathf.Max(tileSpacing, MinRuntimeTileSpacing),
            Mathf.Max(lineSpacing, MinRuntimeLineSpacing)
        );
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = Mathf.Max(1, tilesPerLine);

        var fitter = content.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        LayoutRebuilder.MarkLayoutForRebuild(content);
    }

    private void Clear()
    {
        for (int i = 0; i < _active.Count; i++)
        {
            var tv = _active[i];
            if (tv == null) continue;
            tv.gameObject.SetActive(false);
            _pool.Push(tv);
        }

        _active.Clear();
    }
}
