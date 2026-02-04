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
    private TurnState _turnState;
    private IActionExecutionService _execService;
    private IActionQueryService _queryService;
    public TurnState TurnState => _turnState;

    private void Awake()
    {
        _queryService = new DummyActionQueryService();
        _execService = new DummyActionExecutionService();

        _turnState = new TurnState(new[] { 101, 102, 103, 103, 103 });
    }

    private void Start()
    {
        if (meldSlotsView != null)
            meldSlotsView.Render(_turnState.Melds);
    }

    private void OnEnable()
    {
        inputSource.OnRequest += Handle;
        menuView.OnOptionSelected += OnOptionSelected;
    }

    private void OnDisable()
    {
        inputSource.OnRequest -= Handle;
        menuView.OnOptionSelected -= OnOptionSelected;
    }

    public void Handle(ActionRequest request)
    {
        var snapshot = new TurnSnapshot(
            TurnPhase.Build,
            turnIndex: 1,
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
    }

    private void OnOptionSelected(ActionOption option)
    {
        var cmd = new ActionCommand(option.Command, targetId: 0, payload: option.Payload);

        if (_execService.Execute(cmd, _turnState, out var reason))
        {
            Debug.Log($"[EXEC] Success: {cmd}");
            Debug.Log($"[HAND] {string.Join(",", _turnState.HandTiles)}");

            // 👇 여기
            var meldText = string.Join(" | ",
                _turnState.Melds.Select(m =>
                    $"{m.MeldId}:{m.Type}({string.Join(",", m.Tiles)}) fixed={m.IsFixed}"
                )
            );

            Debug.Log($"[MELDS] {meldText}");
            if (meldSlotsView != null)
                meldSlotsView.Render(_turnState.Melds);
        }
        else
        {
            Debug.LogWarning($"[EXEC] Fail: {cmd} reason={reason}");
        }
    }
}