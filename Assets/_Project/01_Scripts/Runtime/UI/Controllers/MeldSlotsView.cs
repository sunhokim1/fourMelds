using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Project.Core.Turn;

public class MeldSlotsView : MonoBehaviour
{
    private const float MinRuntimePackedSpacing = 80f;

    [SerializeField] private Transform slotsRoot;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private float meldSlotWidth = 220f;
    [SerializeField] private float headSlotWidth = 150f;
    [SerializeField] private float packedSpacing = 80f;
    [SerializeField] private float headToMeldSpacing = 64f;
    [SerializeField] private int horizontalPadding = 12;

    public event Action<int> OnSlotClicked;
    public event Action<int, int> OnSlotTileClicked;
    public event Action<int> OnSlotRightClicked; // ✅ 슬롯 우클릭(전체 되돌리기)

    private GameObject _headSlot;
    private GameObject _headGapSpacer;
    private readonly List<GameObject> _slots = new();

    private void Awake()
    {
        // Scene에 slotsRoot가 패널(부모)로 물려 있으면, 현재 오브젝트를 실제 콘텐츠 루트로 사용한다.
        if (slotsRoot == null || slotsRoot == transform.parent)
            slotsRoot = transform;

        if (transform is RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
        }

        // 부모 HorizontalLayoutGroup의 child 계산에서 제외해서 왼쪽 쏠림/빈칸을 방지한다.
        var le = gameObject.GetComponent<LayoutElement>();
        if (le == null)
            le = gameObject.AddComponent<LayoutElement>();
        le.ignoreLayout = true;
    }

    public void Render(TurnState state)
        => Render(state, pendingPlacement: false, pendingLabel: null);

    public void Render(TurnState state, bool pendingPlacement, string pendingLabel)
    {
        EnsurePackedLayout();
        EnsureHeadSlot();
        EnsureHeadGapSpacer();
        EnsureSlots(4);

        if (_headSlot != null)
        {
            if (_headSlot.transform.parent != slotsRoot)
                _headSlot.transform.SetParent(slotsRoot, false);
            _headSlot.transform.SetAsFirstSibling();
            var headView = _headSlot.GetComponent<MeldSlotView>();
            if (headView != null)
                headView.BindHeadTile(state.HeadTileId);
        }

        if (_headGapSpacer != null)
        {
            if (_headGapSpacer.transform.parent != slotsRoot)
                _headGapSpacer.transform.SetParent(slotsRoot, false);
            _headGapSpacer.transform.SetSiblingIndex(1);
            ApplySlotWidth(_headGapSpacer, Mathf.Max(0f, headToMeldSpacing));
        }

        for (int i = 0; i < 4; i++)
        {
            var slotGo = _slots[i];
            if (slotGo != null && slotGo.transform.parent != slotsRoot)
                slotGo.transform.SetParent(slotsRoot, false);
            if (slotGo != null)
                slotGo.transform.SetSiblingIndex(i + 2);

            var slot = slotGo.GetComponent<MeldSlotView>();

            slot.SetIndex(i);
            slot.SetSelected(i == state.SelectedSlotIndex);
            slot.SetPendingPlacement(pendingPlacement, pendingLabel);

            slot.OnSlotClicked = idx => OnSlotClicked?.Invoke(idx);
            slot.OnSlotTileClicked = (slotIdx, tileId) => OnSlotTileClicked?.Invoke(slotIdx, tileId);
            slot.OnSlotRightClicked = idx => OnSlotRightClicked?.Invoke(idx); // ✅

            slot.BindTiles(i, state.GetSlotTiles(i), state.IsSlotFixed(i));
        }
    }

    private void EnsureSlots(int count)
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            var existing = _slots[i];
            if (existing != null && existing.transform.parent != slotsRoot)
                existing.transform.SetParent(slotsRoot, false);
        }

        while (_slots.Count < count)
        {
            var go = Instantiate(slotPrefab, slotsRoot);
            ApplySlotWidth(go, meldSlotWidth);
            _slots.Add(go);
        }
    }

    private void EnsureHeadSlot()
    {
        if (_headSlot != null)
            return;

        _headSlot = Instantiate(slotPrefab, slotsRoot);
        _headSlot.name = "HeadSlot";
        ApplySlotWidth(_headSlot, headSlotWidth);
    }

    private void EnsureHeadGapSpacer()
    {
        if (_headGapSpacer != null)
            return;

        _headGapSpacer = new GameObject("HeadGapSpacer", typeof(RectTransform), typeof(LayoutElement));
        _headGapSpacer.transform.SetParent(slotsRoot, false);
        ApplySlotWidth(_headGapSpacer, Mathf.Max(0f, headToMeldSpacing));
    }

    private void EnsurePackedLayout()
    {
        if (slotsRoot == null)
            return;

        var hlg = slotsRoot.GetComponent<HorizontalLayoutGroup>();
        if (hlg == null)
            hlg = slotsRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
        if (hlg != null)
        {
            hlg.spacing = Mathf.Max(packedSpacing, MinRuntimePackedSpacing);
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;
            hlg.padding = new RectOffset(horizontalPadding, horizontalPadding, 0, 0);
        }
    }

    private static void ApplySlotWidth(GameObject go, float width)
    {
        if (go == null)
            return;

        if (go.transform is RectTransform rt)
            rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);

        var le = go.GetComponent<LayoutElement>();
        if (le == null)
            le = go.AddComponent<LayoutElement>();

        le.minWidth = width;
        le.preferredWidth = width;
        le.flexibleWidth = 0f;
    }
}
