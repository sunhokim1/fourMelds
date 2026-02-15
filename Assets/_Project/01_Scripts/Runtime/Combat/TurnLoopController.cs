// Assets/_Project/01_Scripts/Runtime/Combat/TurnLoopController.cs

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FourMelds.Combat.TurnIntegration;
using FourMelds.Cards;
using FourMelds.Core.Turn;
using Project.Core.Turn;
using Project.Core.Melds;
using Project.Core.Tiles;

namespace FourMelds.Combat
{
    public sealed class TurnLoopController : MonoBehaviour
    {
        [SerializeField] private ActionMenuController _actionMenu;
        [SerializeField] private CardPanelController _cardPanel;
        [SerializeField] private HandTilesView _handTilesView;
        [SerializeField] private MeldSlotsView _meldSlotsView;
        [SerializeField] private GameObject _cardUseBoardRoot;
        [SerializeField] private GameObject _buildBoardRoot;
        [SerializeField] private Button _buildDoneButton;
        [SerializeField] private Text _turnStatusText;
        [SerializeField] private int _playerHp = 50;
        [SerializeField] private int _enemyHp = 60;
        [SerializeField] private int _cardsPerTurn = 5;
        [SerializeField] private int[] _startingDeckCardIndices = Array.Empty<int>();
        [SerializeField] private bool _animatePhaseSwap = true;
        [SerializeField] private float _phaseSwapDuration = 0.22f;
        [SerializeField] private float _cardPanelHiddenYOffset = -280f;
        [SerializeField] private float _meldPanelHiddenYOffset = -220f;
        [SerializeField] private bool _cardPanelUsesMeldShownPosition = true;
        [SerializeField] private Vector2 _cardPanelShownOffsetFromMeld = Vector2.zero;

        // 카드 시스템 전까지 임시 시작 손패 수
        [SerializeField] private int _initialHandSize = 8;
        [SerializeField] private bool _ensureAtLeastOneHonorInInitialHand = true;

        private TurnState _turnState;
        private CombatState _combatState;

        private TurnAttackContextBuilder _builder;
        private TurnEndAttackExecutor _executor;
        private AttackResultApplier _applier;

        // 카드/드로우용 TilePool (요청사항: 턴 시작마다 초기화)
        private TilePool _tilePool;
        private CardDeckState _cardDeck;
        private RectTransform _cardPanelRect;
        private RectTransform _meldPanelRect;
        private Vector2 _cardPanelShownPos;
        private Vector2 _cardPanelHiddenPos;
        private Vector2 _meldPanelShownPos;
        private Vector2 _meldPanelHiddenPos;
        private Coroutine _phaseSwapRoutine;
        private Coroutine _deferredAdvanceRoutine;
        private bool _phaseShownAnchorsCaptured;
        private Vector2 _baseMeldShownPos;
        private Vector2 _baseCardShownPos;
        private bool _phaseSwapInitialized;
        private int _lastScreenWidth;
        private int _lastScreenHeight;

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
            if (_cardPanel == null)
                _cardPanel = FindFirstObjectByType<CardPanelController>();
            if (_handTilesView == null)
                _handTilesView = FindFirstObjectByType<HandTilesView>();
            if (_meldSlotsView == null)
                _meldSlotsView = FindFirstObjectByType<MeldSlotsView>();

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

            _cardDeck = new CardDeckState();
            InitializeDeck();
            if (_cardPanel != null)
                _cardPanel.OnHandCardClicked += OnHandCardClicked;

            StartCoroutine(BeginTurnLoopAfterUiLayout());
        }

        private void OnDestroy()
        {
            if (_cardPanel != null)
                _cardPanel.OnHandCardClicked -= OnHandCardClicked;

            if (_phaseSwapRoutine != null)
            {
                StopCoroutine(_phaseSwapRoutine);
                _phaseSwapRoutine = null;
            }

            if (_deferredAdvanceRoutine != null)
            {
                StopCoroutine(_deferredAdvanceRoutine);
                _deferredAdvanceRoutine = null;
            }
        }

        private IEnumerator BeginTurnLoopAfterUiLayout()
        {
            // Wait one frame so all UI Start() and layout calculations are completed.
            yield return null;
            Canvas.ForceUpdateCanvases();

            InitializePhaseSwapAnchors();
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;

            // Day4: 턴 시작 트리거
            _turnState.SetPhase(TurnPhase.Draw);
            ScheduleAdvance();
        }

        // UI 버튼(= Build 완료)에서 호출
        public void OnClick_BuildDone()
        {
            if (_turnState == null)
            {
                Debug.LogError("[TURN] TurnState is null. Did Start() run?");
                return;
            }

            if (_turnState.Phase == TurnPhase.CardUse)
            {
                EnterBuildFromCardUse();
                return;
            }

            if (_turnState.Phase != TurnPhase.Build)
            {
                Debug.LogWarning($"[TURN] BuildDone ignored. Current phase={_turnState.Phase}");
                return;
            }

            SetBuildDoneInteractable(false);

            // "지금 상태는 Build다. Build에서 해야 할 다음 행동(ResolvePlayer)을 실행해라"
            _turnState.SetPhase(TurnPhase.Build);
            RefreshTurnStatus();
            ScheduleAdvance();
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
                    EnterCardUseFromDraw();
                    break;

                case TurnPhase.CardUse:
                    // Wait for user input (cards / done button).
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

        private void ScheduleAdvance()
        {
            if (!isActiveAndEnabled || _turnState == null)
                return;

            if (_deferredAdvanceRoutine != null)
                return;

            _deferredAdvanceRoutine = StartCoroutine(AdvanceNextFrame());
        }

        private IEnumerator AdvanceNextFrame()
        {
            yield return null;
            _deferredAdvanceRoutine = null;

            if (!isActiveAndEnabled || _turnState == null)
                yield break;

            Advance();
        }

        /// <summary>
        /// Draw 단계에서 "턴 시작 처리"를 하고 CardUse로 들어간다.
        /// - Head 1장 자동 지급(손패에 넣지 않음)
        /// - 카드 시스템 전까지 임시 시작 손패 드로우
        /// </summary>
        private void EnterCardUseFromDraw()
        {
            // 요청사항: 턴마다 카드/드로우 풀 초기화
            _tilePool = new TilePool(TileCatalog.AllTileIds, copiesPerTile: 4);
            _turnState.SetPool(_tilePool);

            // 1) 턴 시작마다 Head 1장 (A안: 별도 슬롯)
            if (!_tilePool.TryDrawRandom(_ => true, out var head))
                throw new InvalidOperationException("TilePool exhausted - cannot draw Head.");

            _turnState.SetHeadTile(head);

            // 2) 카드 시스템 전까지 임시 시작 손패
            //    (룰: '턴 종료 시 손패 소멸'이므로, 매 턴 새로 뽑아야 플레이가 됨)
            int drawnCount = 0;
            if (_ensureAtLeastOneHonorInInitialHand && _initialHandSize > 0)
            {
                if (_tilePool.TryDrawRandom(id => id / 100 == 4, out var honorTile))
                {
                    _turnState.AddHandTile(honorTile);
                    drawnCount++;
                }
            }

            for (int i = drawnCount; i < _initialHandSize; i++)
            {
                if (!_tilePool.TryDrawRandom(_ => true, out var t))
                    break;

                _turnState.AddHandTile(t);
            }

            _turnState.SetPhase(TurnPhase.CardUse);
            _cardDeck?.StartTurnDraw(_cardsPerTurn);
            SetBuildDoneInteractable(true);
            UpdateAdvanceButtonLabel();
            UpdatePhaseBoardState();
            UpdateCardPanelState();
            RefreshTurnStatus();

            if (_actionMenu != null)
                _actionMenu.RefreshUIFromState(hideActionMenu: true, clearPendingQuickMeld: true);

            Debug.Log("[TURN] TilePool reset for this turn.");
            Debug.Log($"[TURN] Enter CardUse. Turn={_turnState.TurnIndex}, Head={_turnState.HeadTileId}, PlayerHP={_combatState.PlayerHP}, EnemyHP={_combatState.EnemyHP}");
        }

        private void EnterBuildFromCardUse()
        {
            _cardDeck?.DiscardHand();
            _turnState.SetPhase(TurnPhase.Build);
            SetBuildDoneInteractable(true);
            UpdateAdvanceButtonLabel();
            UpdatePhaseBoardState();
            UpdateCardPanelState();
            RefreshTurnStatus();

            if (_actionMenu != null)
                _actionMenu.RefreshUIFromState(hideActionMenu: true, clearPendingQuickMeld: true);

            Debug.Log($"[TURN] CardUseDone -> Build. Turn={_turnState.TurnIndex}");
        }

        private void ResolvePlayer()
        {
            _turnState.SetPhase(TurnPhase.Resolve);
            SetBuildDoneInteractable(false);
            UpdateAdvanceButtonLabel();
            UpdatePhaseBoardState();
            UpdateCardPanelState();
            RefreshTurnStatus();

            Debug.Log($"[TURN] BuildDone -> Resolve. EnemyHP(before)={_combatState.EnemyHP}");

            var ctx = _builder.Build(_turnState, _combatState);
            var result = _executor.Execute(ctx);
            _applier.Apply(_combatState, result);

            if (result.LogEntries != null && result.LogEntries.Count > 0)
            {
                for (int i = 0; i < result.LogEntries.Count; i++)
                {
                    var l = result.LogEntries[i];
                    Debug.Log($"[DMGLOG] {l.StepId}: {l.BeforeDamage} -> {l.AfterDamage} ({l.Reason})");
                }
            }

            Debug.Log($"[TURN] Resolve done. Damage={result.FinalDamage}, EnemyHP(after)={_combatState.EnemyHP}");

            ScheduleAdvance(); // Resolve -> Enemy
        }

        private void EnterEnemy()
        {
            _turnState.SetPhase(TurnPhase.Enemy);
            SetBuildDoneInteractable(false);
            UpdateAdvanceButtonLabel();
            UpdatePhaseBoardState();
            UpdateCardPanelState();
            RefreshTurnStatus();
            ScheduleAdvance(); // Day4: 적 턴은 자동 진행
        }

        private void RunEnemy()
        {
            // Day4 최소 구현: 고정 데미지 1
            const int dmg = 1;
            _combatState.ApplyPlayerDamage(dmg);

            Debug.Log($"[TURN] EnemyAct. Damage={dmg}, PlayerHP(after)={_combatState.PlayerHP}");

            _turnState.SetPhase(TurnPhase.Cleanup);
            SetBuildDoneInteractable(false);
            UpdateAdvanceButtonLabel();
            UpdatePhaseBoardState();
            UpdateCardPanelState();
            RefreshTurnStatus();
            ScheduleAdvance();
        }

        private void StartNextTurn()
        {
            _turnState.CleanupForNextTurn();
            UpdateAdvanceButtonLabel();
            UpdatePhaseBoardState();
            UpdateCardPanelState();
            RefreshTurnStatus();

            Debug.Log($"[TURN] Cleanup -> NextTurn. Turn={_turnState.TurnIndex}");

            ScheduleAdvance(); // -> Draw
        }

        private void SetBuildDoneInteractable(bool enabled)
        {
            if (_buildDoneButton != null)
                _buildDoneButton.interactable = enabled;
        }

        private void UpdateAdvanceButtonLabel()
        {
            if (_buildDoneButton == null)
                return;

            var label = _buildDoneButton.GetComponentInChildren<Text>(true);
            if (label == null)
                return;

            label.text = _turnState != null && _turnState.Phase == TurnPhase.CardUse
                ? "카드 사용 완료"
                : "빌드 완료";
        }

        private void UpdateCardPanelState()
        {
            if (_cardPanel == null || _turnState == null)
                return;

            bool isCardUse = _turnState.Phase == TurnPhase.CardUse;
            _cardPanel.SetInteractable(isCardUse);
            _cardPanel.RenderHand(_cardDeck?.HandCards ?? Array.Empty<int>());
        }

        private void UpdatePhaseBoardState()
        {
            if (_turnState == null)
                return;

            MaybeRefreshPhaseSwapAnchors();

            bool isCardUse = _turnState.Phase == TurnPhase.CardUse;

            AlignCardPanelToMeldPanel();

            // Explicit phase board roots are optional.
            // If not assigned, keep hand visible and toggle only meld area.
            if (_cardUseBoardRoot != null || _buildBoardRoot != null)
            {
                SetActiveIfNotNull(_cardUseBoardRoot, isCardUse);
                SetActiveIfNotNull(_buildBoardRoot, !isCardUse);
                return;
            }

            if (_handTilesView != null)
                _handTilesView.gameObject.SetActive(true);

            if (_animatePhaseSwap && _cardPanelRect != null && _meldPanelRect != null)
            {
                StartPhaseSwapAnimation(isCardUse);
                return;
            }

            if (_cardPanel != null)
                _cardPanel.SetVisible(isCardUse);
            if (_meldSlotsView != null)
                _meldSlotsView.gameObject.SetActive(!isCardUse);
        }

        private static void SetActiveIfNotNull(GameObject go, bool active)
        {
            if (go != null)
                go.SetActive(active);
        }

        private void MaybeRefreshPhaseSwapAnchors()
        {
            if (!_animatePhaseSwap)
                return;

            int width = Screen.width;
            int height = Screen.height;
            bool screenChanged = width != _lastScreenWidth || height != _lastScreenHeight;
            bool missingRects = _cardPanelRect == null || _meldPanelRect == null;

            if (!screenChanged && !missingRects)
                return;

            _lastScreenWidth = width;
            _lastScreenHeight = height;
            _phaseShownAnchorsCaptured = false;

            InitializePhaseSwapAnchors();

            if (_turnState != null && _phaseSwapInitialized)
                SnapPhasePanelsImmediately(_turnState.Phase == TurnPhase.CardUse);
        }

        private void InitializePhaseSwapAnchors()
        {
            // Ensure UI layout is settled before capturing initial shown positions.
            Canvas.ForceUpdateCanvases();

            if (_cardPanel != null)
                _cardPanelRect = _cardPanel.transform as RectTransform;

            if (_meldSlotsView != null)
            {
                _meldPanelRect = _meldSlotsView.transform.parent as RectTransform;
                if (_meldPanelRect == null)
                    _meldPanelRect = _meldSlotsView.transform as RectTransform;
            }

            if (_cardPanelRect != null)
            {
                _baseCardShownPos = _cardPanelRect.anchoredPosition;
                _cardPanelShownPos = _baseCardShownPos;
            }

            if (_meldPanelRect != null)
            {
                _baseMeldShownPos = _meldPanelRect.anchoredPosition;
                _meldPanelShownPos = _baseMeldShownPos;
                _meldPanelHiddenPos = _meldPanelShownPos + new Vector2(0f, _meldPanelHiddenYOffset);
            }

            AlignCardPanelToMeldPanel();

            if (_cardPanelRect != null)
                _cardPanelHiddenPos = _cardPanelShownPos + new Vector2(0f, _cardPanelHiddenYOffset);
        }

        private void AlignCardPanelToMeldPanel()
        {
            if (!_cardPanelUsesMeldShownPosition || _cardPanelRect == null || _meldPanelRect == null)
                return;

            if (!_phaseShownAnchorsCaptured)
            {
                Canvas.ForceUpdateCanvases();
                _baseMeldShownPos = _meldPanelRect.anchoredPosition;
                _baseCardShownPos = _cardPanelRect.anchoredPosition;
                _phaseShownAnchorsCaptured = true;
            }

            // Match card panel rect basis to meld panel every phase update.
            _cardPanelRect.anchorMin = _meldPanelRect.anchorMin;
            _cardPanelRect.anchorMax = _meldPanelRect.anchorMax;
            _cardPanelRect.pivot = _meldPanelRect.pivot;
            _cardPanelRect.sizeDelta = _meldPanelRect.sizeDelta;

            _meldPanelShownPos = _baseMeldShownPos;
            _meldPanelHiddenPos = _meldPanelShownPos + new Vector2(0f, _meldPanelHiddenYOffset);
            _cardPanelShownPos = _meldPanelShownPos + _cardPanelShownOffsetFromMeld;
            _cardPanelHiddenPos = _cardPanelShownPos + new Vector2(0f, _cardPanelHiddenYOffset);
        }

        private void StartPhaseSwapAnimation(bool isCardUse)
        {
            if (!_phaseSwapInitialized)
            {
                SnapPhasePanelsImmediately(isCardUse);
                _phaseSwapInitialized = true;
                return;
            }

            if (_phaseSwapRoutine != null)
                StopCoroutine(_phaseSwapRoutine);

            _phaseSwapRoutine = StartCoroutine(PhaseSwapRoutine(isCardUse));
        }

        private void SnapPhasePanelsImmediately(bool isCardUse)
        {
            if (_cardPanel != null)
                _cardPanel.SetVisible(true);
            if (_meldSlotsView != null)
                _meldSlotsView.gameObject.SetActive(true);

            if (_cardPanelRect != null)
                _cardPanelRect.anchoredPosition = isCardUse ? _cardPanelShownPos : _cardPanelHiddenPos;
            if (_meldPanelRect != null)
                _meldPanelRect.anchoredPosition = isCardUse ? _meldPanelHiddenPos : _meldPanelShownPos;

            if (!isCardUse && _cardPanel != null)
                _cardPanel.SetVisible(false);
            if (isCardUse && _meldSlotsView != null)
                _meldSlotsView.gameObject.SetActive(false);
        }

        private IEnumerator PhaseSwapRoutine(bool isCardUse)
        {
            if (_cardPanel != null)
                _cardPanel.SetVisible(true);
            if (_meldSlotsView != null)
                _meldSlotsView.gameObject.SetActive(true);

            var cardStart = _cardPanelRect.anchoredPosition;
            var meldStart = _meldPanelRect.anchoredPosition;
            var cardTarget = isCardUse ? _cardPanelShownPos : _cardPanelHiddenPos;
            var meldTarget = isCardUse ? _meldPanelHiddenPos : _meldPanelShownPos;

            float duration = Mathf.Max(0.01f, _phaseSwapDuration);
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float u = Mathf.Clamp01(t / duration);
                float eased = u * u * (3f - 2f * u);
                _cardPanelRect.anchoredPosition = Vector2.Lerp(cardStart, cardTarget, eased);
                _meldPanelRect.anchoredPosition = Vector2.Lerp(meldStart, meldTarget, eased);
                yield return null;
            }

            _cardPanelRect.anchoredPosition = cardTarget;
            _meldPanelRect.anchoredPosition = meldTarget;

            if (!isCardUse && _cardPanel != null)
                _cardPanel.SetVisible(false);
            if (isCardUse && _meldSlotsView != null)
                _meldSlotsView.gameObject.SetActive(false);

            _phaseSwapRoutine = null;
        }

        private void RefreshTurnStatus()
        {
            if (_turnStatusText == null || _turnState == null)
                return;

            _turnStatusText.text =
                $"Turn {_turnState.TurnIndex} | Phase: {_turnState.Phase} | PlayerHP: {_combatState.PlayerHP} | EnemyHP: {_combatState.EnemyHP} | Deck:{_cardDeck?.DrawPileCount ?? 0} Discard:{_cardDeck?.DiscardPileCount ?? 0}";
        }

        private void InitializeDeck()
        {
            if (_cardDeck == null)
                return;

            if (_startingDeckCardIndices != null && _startingDeckCardIndices.Length > 0)
            {
                _cardDeck.SetDeck(_startingDeckCardIndices);
                return;
            }

            var starter = new int[CardRegistry.DefaultCards.Count];
            for (int i = 0; i < starter.Length; i++)
                starter[i] = i;

            _cardDeck.SetDeck(starter);
        }

        private void OnHandCardClicked(int handIndex)
        {
            if (_turnState == null || _turnState.Phase != TurnPhase.CardUse)
                return;

            if (_cardDeck == null || !_cardDeck.TryGetHandCard(handIndex, out var cardIndex))
                return;

            if (!CardPlayService.TryPlay(cardIndex, _turnState, out var reason))
            {
                Debug.LogWarning($"[CARD] Fail idx={cardIndex} reason={reason}");
                return;
            }

            _cardDeck.TryPlayHandCard(handIndex, out _);
            Debug.Log($"[CARD] Played idx={cardIndex}");

            if (_actionMenu != null)
                _actionMenu.RefreshUIFromState(hideActionMenu: true, clearPendingQuickMeld: true);

            UpdateCardPanelState();
            RefreshTurnStatus();
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
