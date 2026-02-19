// Assets/_Project/01_Scripts/Runtime/UI/Controllers/ActionMenuController.cs

using UnityEngine;
using Project.InputSystem;
using Project.Core.Turn;
using Project.Core.Action.Query;
using Project.Core.Action.Execute;
using Project.UI.Models;
using System.Linq;
using FourMelds.Core.Turn;

public class ActionMenuController : MonoBehaviour, IActionRequestSink
{
    [SerializeField] private MouseActionRequestSource inputSource;
    [SerializeField] private ActionMenuView menuView;
    [SerializeField] private MeldSlotsView meldSlotsView;
    [SerializeField] private HandTilesView handTilesView;

    private TurnState _turnState;
    private IActionExecutionService _execService;
    private IActionQueryService _queryService;
    private ActionOption _pendingQuickMeldOption;

    public TurnState TurnState => _turnState;

    private void Awake()
    {
        _queryService = new DummyActionQueryService();
        _execService = new DummyActionExecutionService();

        _turnState = new TurnState(System.Array.Empty<int>());
        _turnState.SelectSlot(0);
    }

    private void Start()
    {

        if (meldSlotsView != null)
        {
            meldSlotsView.OnSlotClicked += OnSlotClicked;
            meldSlotsView.OnSlotTileClicked += OnSlotTileClicked;
            meldSlotsView.OnSlotRightClicked += OnSlotRightClicked;
            meldSlotsView.Render(_turnState);
        }

        if (handTilesView != null)
            handTilesView.Render(_turnState.HandTiles);

    }

    private void OnEnable()
    {
        if (inputSource != null)
            inputSource.OnRequest += Handle;

        if (menuView != null)
            menuView.OnOptionSelected += OnOptionSelected;
    }

    private void OnDisable()
    {
        if (inputSource != null)
            inputSource.OnRequest -= Handle;

        if (menuView != null)
            menuView.OnOptionSelected -= OnOptionSelected;
    }

    public void Handle(ActionRequest request)
    {
        if (_turnState == null)
            return;

        if (_turnState.Phase != TurnPhase.Build)
        {
            if (menuView != null)
                menuView.Hide();
            return;
        }

        // ✅ 좌클릭: 슬롯에 1장씩 넣기
        if (request.RequestType == ActionRequestType.SelectTile && request.TargetType == ActionTargetType.Tile)
        {
            int tileId = request.TargetId;

            if (!_turnState.TryAddTileToSelectedSlot(tileId, out var reason))
                Debug.LogWarning($"[SLOT] Add failed: {reason}");

            RenderAll();
            return;
        }

        // ✅ 우클릭: 액션 메뉴
        if (request.RequestType == ActionRequestType.OpenActionMenu)
        {
            var snapshot = new TurnSnapshot(
                _turnState.Phase,
                turnIndex: _turnState.TurnIndex,
                handTiles: _turnState.HandTiles,
                meldIds: _turnState.Melds.Select(m => m.MeldId).ToArray()
            );

            var menu = _queryService.Query(request, snapshot);

            if (menu.Options.Count == 0)
            {
                menuView.Hide();
                return;
            }

            menuView.Show(menu);
            return;
        }
    }

    private void OnOptionSelected(ActionOption option)
    {
        if (_turnState == null || _turnState.Phase != TurnPhase.Build)
            return;

        if (option != null &&
            (option.Command == ActionCommandType.CreateShuntsu ||
             option.Command == ActionCommandType.CreateKoutsu ||
             option.Command == ActionCommandType.PromoteToKan))
        {
            if (IsSamePendingOption(option))
            {
                CancelPendingQuickMeld("same option clicked");
                return;
            }

            _pendingQuickMeldOption = option;
            Debug.Log($"[QUICK-MELD] Pending {option.Command}. Click a slot to place.");
            RenderAll();
            return;
        }

        var cmd = new ActionCommand(option.Command, targetId: 0, payload: option.Payload);

        if (_execService.Execute(cmd, _turnState, out var reason))
        {
            Debug.Log($"[EXEC] Success: {cmd}");
            Debug.Log($"[HAND] {string.Join(",", _turnState.HandTiles)}");
            RenderAll();
        }
        else
        {
            Debug.LogWarning($"[EXEC] Fail: {cmd} reason={reason}");
        }
    }

    private void OnSlotClicked(int index)
    {
        if (_turnState == null || _turnState.Phase != TurnPhase.Build)
            return;

        if (_pendingQuickMeldOption != null)
        {
            if (_pendingQuickMeldOption.Command == ActionCommandType.PromoteToKan)
            {
                if (_pendingQuickMeldOption.Payload is not int kanTileId)
                {
                    Debug.LogWarning("[QUICK-MELD] Pending Kan option has invalid payload.");
                    _pendingQuickMeldOption = null;
                }
                else if (_turnState.TryPromoteToKanInSlot(index, kanTileId, out var kanReason))
                {
                    _turnState.SelectSlot(index);
                    _pendingQuickMeldOption = null;
                    Debug.Log($"[QUICK-MELD] Kan fixed at slot={index}: {kanTileId}x4 + rinshan");
                }
                else
                {
                    Debug.LogWarning($"[QUICK-MELD] Kan failed: {kanReason}");
                }
            }
            else
            {
                var tiles = ResolvePendingTiles(_pendingQuickMeldOption);
                if (tiles == null || tiles.Length == 0)
                {
                    Debug.LogWarning("[QUICK-MELD] Pending option has no tiles.");
                    _pendingQuickMeldOption = null;
                }
                else if (_turnState.TryReplaceSlotWithTilesFromHand(index, tiles, out var replaceReason))
                {
                    _turnState.SelectSlot(index);
                    _pendingQuickMeldOption = null;
                    Debug.Log($"[QUICK-MELD] Placed into slot={index}: {string.Join(",", tiles)}");
                }
                else
                {
                    Debug.LogWarning($"[QUICK-MELD] Place failed: {replaceReason}");
                }
            }

            RenderAll();
            return;
        }

        _turnState.SelectSlot(index);

        RenderMeldSlots();
    }

    private void OnSlotTileClicked(int slotIndex, int tileId)
    {
        if (_turnState == null || _turnState.Phase != TurnPhase.Build)
            return;

        if (!_turnState.TryRemoveTileFromSlot(slotIndex, tileId, out var reason))
            Debug.LogWarning($"[SLOT] Remove failed: {reason}");

        RenderAll();
    }

    private void OnSlotRightClicked(int slotIndex)
    {
        if (_turnState == null || _turnState.Phase != TurnPhase.Build)
            return;

        if (_pendingQuickMeldOption != null)
        {
            CancelPendingQuickMeld("slot right click");
            return;
        }

        if (!_turnState.TryClearSlotToHand(slotIndex, out var reason))
            Debug.LogWarning($"[SLOT] Clear failed: {reason}");

        RenderAll();
    }

    private static int[] ResolvePendingTiles(ActionOption option)
    {
        if (option == null) return null;

        if (option.Command == ActionCommandType.CreateShuntsu &&
            option.Payload is Project.Core.Tiles.MeldCandidate cand)
        {
            return new[] { cand.A, cand.B, cand.C };
        }

        if (option.Command == ActionCommandType.CreateKoutsu &&
            option.Payload is int tileId)
        {
            return new[] { tileId, tileId, tileId };
        }
        if (option.Command == ActionCommandType.PromoteToKan &&
            option.Payload is int kanTileId)
        {
            return new[] { kanTileId, kanTileId, kanTileId, kanTileId };
        }

        return option.PreviewTiles;
    }

    private bool IsSamePendingOption(ActionOption option)
    {
        if (_pendingQuickMeldOption == null || option == null)
            return false;

        if (_pendingQuickMeldOption.Command != option.Command)
            return false;

        var a = ResolvePendingTiles(_pendingQuickMeldOption);
        var b = ResolvePendingTiles(option);
        if (a == null || b == null || a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;

        return true;
    }

    private void CancelPendingQuickMeld(string reason)
    {
        if (_pendingQuickMeldOption == null) return;
        Debug.Log($"[QUICK-MELD] Cancelled ({reason})");
        _pendingQuickMeldOption = null;
        RenderMeldSlots();
    }

    private string GetPendingLabel()
    {
        if (_pendingQuickMeldOption == null) return null;
        return _pendingQuickMeldOption.Command == ActionCommandType.CreateShuntsu ? "Shuntsu" :
               _pendingQuickMeldOption.Command == ActionCommandType.CreateKoutsu ? "Koutsu" :
               _pendingQuickMeldOption.Command == ActionCommandType.PromoteToKan ? "Kan" :
               _pendingQuickMeldOption.Command.ToString();
    }

    private void RenderMeldSlots()
    {
        if (meldSlotsView == null) return;
        meldSlotsView.Render(
            _turnState,
            pendingPlacement: _pendingQuickMeldOption != null,
            pendingLabel: GetPendingLabel()
        );
    }

    private void RenderAll()
    {
        if (handTilesView != null)
        {
            handTilesView.SetSpecialTileMarker(_turnState.RinshanTileId, _turnState.RinshanTileOccurrence);
            handTilesView.Render(_turnState.HandTiles);
        }

        RenderMeldSlots();
    }

    public void RefreshUIFromState(bool hideActionMenu = true, bool clearPendingQuickMeld = true)
    {
        if (clearPendingQuickMeld)
            _pendingQuickMeldOption = null;

        if (hideActionMenu && menuView != null)
            menuView.Hide();

        RenderAll();
    }

}
