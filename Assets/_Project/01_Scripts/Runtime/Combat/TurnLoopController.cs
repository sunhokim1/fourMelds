// Assets/_Project/01_Scripts/Runtime/Combat/TurnLoopController.cs

using UnityEngine;
using FourMelds.Combat.TurnIntegration;
using FourMelds.Core.Turn;
using Project.Core.Turn;
using Project.Core.Melds;
using Project.Core.Tiles;

namespace FourMelds.Combat
{
    public sealed class TurnLoopController : MonoBehaviour
    {
        [SerializeField] private ActionMenuController _actionMenu;
        [SerializeField] private int _playerHp = 50;
        [SerializeField] private int _enemyHp = 60;

        private TurnState _turnState;
        private CombatState _combatState;

        private TurnAttackContextBuilder _builder;
        private TurnEndAttackExecutor _executor;
        private AttackResultApplier _applier;
        private TilePool _tilePool;


        private void Awake()
        {
            _combatState = new CombatState(_playerHp, _enemyHp);

            _builder = new TurnAttackContextBuilder();
            _executor = new TurnEndAttackExecutor(CreatePipeline());
            _applier = new AttackResultApplier();
        }

        private void Start()
        {
            if (_actionMenu == null)
                _actionMenu = FindFirstObjectByType<ActionMenuController>();

            if (_actionMenu == null)
            {
                Debug.LogError("[TURN] ActionMenuController not found in scene.");
                enabled = false;
                return;
            }

            _turnState = _actionMenu.TurnState;

            if (_turnState == null)
            {
                Debug.LogError("[TURN] ActionMenuController.TurnState is null. (Awake order?)");
                enabled = false;
                return;
            }

            Debug.Log("[TURN] TurnState linked.");

            Dev_Test_ToitoiSanankou();
            // Day4: 턴 시작 트리거
            _turnState.SetPhase(TurnPhase.Draw);
            Advance();
        }

        // UI 버튼(= Build 완료)에서 호출
        public void OnClick_BuildDone()
        {
            if (_turnState == null)
            {
                Debug.LogError("[TURN] TurnState is null. Did Start() run?");
                return;
            }

            // "지금 상태는 Build다. Build에서 해야 할 다음 행동(ResolvePlayer)을 실행해라"
            _turnState.SetPhase(TurnPhase.Build);
            Advance();
        }

        private DamagePipeline CreatePipeline()
        {
            return new DamagePipeline(new IDamageStep[]
            {
                new BaseDamageStep(),
                new YakuStep(),
                new ClampStep() // 아주 큰 값만 컷
            });


        }


        private void Advance()
        {
            switch (_turnState.Phase)
            {
                case TurnPhase.Draw:
                    EnterBuild();
                    break;

                case TurnPhase.Build:
                    ResolvePlayer();
                    break;

                case TurnPhase.Resolve:
                    EnterEnemy();
                    break;

                case TurnPhase.Enemy:
                    RunEnemy();
                    break;

                case TurnPhase.Cleanup:
                    StartNextTurn();
                    break;
            }
        }

        private void EnterBuild()
        {
            _turnState.SetPhase(TurnPhase.Build);
            Debug.Log($"[TURN] Enter Build. Turn={_turnState.TurnIndex}, PlayerHP={_combatState.PlayerHP}, EnemyHP={_combatState.EnemyHP}");
            // Day4: Draw/Hand 생성은 아직 없음. (Day8 카드/드로우 붙일 때 여기 확장)
        }

        private void ResolvePlayer()
        {
            _turnState.SetPhase(TurnPhase.Resolve);

            Debug.Log($"[TURN] BuildDone -> Resolve. EnemyHP(before)={_combatState.EnemyHP}");

            var ctx = _builder.Build(_turnState, _combatState);
            var result = _executor.Execute(ctx);
            _applier.Apply(_combatState, result);
            if (result.LogEntries != null && result.LogEntries.Count > 0)
            {
                for (int i = 0; i < result.LogEntries.Count; i++)
                {
                    var l = result.LogEntries[i];
                    Debug.Log(
                        $"[DMGLOG] {l.StepId}: {l.BeforeDamage} -> {l.AfterDamage} ({l.Reason})"
                    );
                }
            }



            Debug.Log($"[TURN] Resolve done. Damage={result.FinalDamage}, EnemyHP(after)={_combatState.EnemyHP}");

            Advance(); // Resolve -> Enemy
        }

        private void EnterEnemy()
        {
            _turnState.SetPhase(TurnPhase.Enemy);
            Advance(); // Day4: 적 턴은 자동 진행
        }

        private void RunEnemy()
        {
            // Day4 최소 구현: 고정 데미지 1
            const int dmg = 1;
            _combatState.ApplyPlayerDamage(dmg);

            Debug.Log($"[TURN] EnemyAct. Damage={dmg}, PlayerHP(after)={_combatState.PlayerHP}");

            _turnState.SetPhase(TurnPhase.Cleanup);
            Advance();
        }

        private void StartNextTurn()
        {
            _turnState.CleanupForNextTurn();

            Debug.Log($"[DEV] Head before draw = {_turnState.HeadTileId}");
            Debug.Log($"[TURN] Cleanup -> NextTurn. Turn={_turnState.TurnIndex}");

            Advance();
        }


        public void Dev_Setup_ToitoiSanankou()
        {
            _turnState.ClearAllHandTiles();
            _turnState.ClearAllMelds();

            // 예: 커쯔 3개 + 아무 몸통 1개
            _turnState.Dev_AddMeld(MeldType.Koutsu, new[] { 101, 101, 101 }, fixedNow: false); // 앙커로 취급
            _turnState.Dev_AddMeld(MeldType.Koutsu, new[] { 102, 102, 102 }, fixedNow: false);
            _turnState.Dev_AddMeld(MeldType.Koutsu, new[] { 103, 103, 103 }, fixedNow: false);
            _turnState.Dev_AddMeld(MeldType.Koutsu, new[] { 201, 201, 201 }, fixedNow: true);  // 밍커로 취급(고정)

            Debug.Log("[DEV] Setup Toitoi + Sanankou");
        }

        public void Dev_Test_ToitoiSanankou()
        {
            if (_turnState == null) return;

            _turnState.Dev_SetMelds(
                (MeldType.Koutsu, false, new[] { 102, 102, 102 }),
                (MeldType.Koutsu, false, new[] { 103, 103, 103 }),
                (MeldType.Koutsu, false, new[] { 104, 104, 104 }),
                (MeldType.Koutsu, true, new[] { 202, 202, 202 })
            );

            Debug.Log("[DEV] Melds set: Toitoi + Sanankou expected");
        }

    }
}
