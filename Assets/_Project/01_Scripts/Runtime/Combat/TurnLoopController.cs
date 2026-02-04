using UnityEngine;
using Project.Core.Turn;
using FourMelds.Combat.TurnIntegration;

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
        }

        public void OnClick_BuildDone()
        {
            if (_turnState == null)
            {
                Debug.LogError("[TURN] TurnState is null. Did Start() run?");
                return;
            }

            Debug.Log($"[TURN] BuildDone -> Resolve. EnemyHP(before)={_combatState.EnemyHP}");

            var ctx = _builder.Build(_turnState, _combatState);
            var result = _executor.Execute(ctx);
            _applier.Apply(_combatState, result);

            Debug.Log($"[TURN] Resolve done. Damage={result.FinalDamage}, EnemyHP(after)={_combatState.EnemyHP}");
        }

        private DamagePipeline CreatePipeline()
        {
            return new DamagePipeline(new IDamageStep[]
            {
                new BaseDamageStep(),
                new ClampStep()
            });
        }

    }
}
