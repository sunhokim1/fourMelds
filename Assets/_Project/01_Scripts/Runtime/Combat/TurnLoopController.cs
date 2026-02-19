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
using UnityEngine.EventSystems;

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
        [Header("Exchange UI")]
        [SerializeField] private GameObject _exchangePanelRoot;
        [SerializeField] private Text _exchangeTitleText;
        [SerializeField] private Text _exchangeStatusText;
        [SerializeField] private Transform _exchangeSelectedTilesRoot;
        [SerializeField] private Image _exchangeSelectedTilesFrame;
        [SerializeField] private Button _exchangeConfirmButton;
        [SerializeField] private Button _exchangeCancelButton;
        [Header("Reward UI")]
        [SerializeField] private GameObject _rewardPanelRoot;
        [SerializeField] private Text _rewardTitleText;
        [SerializeField] private Text _rewardStatusText;
        [SerializeField] private Transform _rewardChoicesRoot;
        [SerializeField] private Button _rewardSkipButton;
        [SerializeField] [Range(1f, 1.8f)] private float _rewardCardPreviewScale = 1.22f;
        [SerializeField] private int _playerHp = 50;
        [SerializeField] private int _enemyHp = 60;
        [SerializeField] private int _cardsPerTurn = 5;
        [SerializeField] private int[] _startingDeckCardIndices = Array.Empty<int>();
        [SerializeField] private int _rewardEveryTurns = 3;
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
        private bool _isExchangePending;
        private int _pendingExchangeCardHandIndex = -1;
        private int _pendingExchangeCardIndex = -1;
        private int _pendingExchangeMaxCount;
        private CardDefinition _pendingExchangeDefinition;
        private readonly System.Collections.Generic.List<int> _pendingExchangeTiles = new System.Collections.Generic.List<int>();
        private readonly System.Collections.Generic.List<TileView> _pendingExchangeTileViews = new System.Collections.Generic.List<TileView>();
        private readonly System.Collections.Generic.List<GameObject> _exchangePreviewTiles = new System.Collections.Generic.List<GameObject>();
        private readonly System.Collections.Generic.List<CanvasGroup> _exchangeDimCanvasGroups = new System.Collections.Generic.List<CanvasGroup>();
        private readonly System.Collections.Generic.List<int> _rewardPoolCardIndices = new System.Collections.Generic.List<int>();
        private bool _rewardPoolBuilt;
        private bool _isRewardSelectionPending;
        private readonly System.Collections.Generic.List<int> _pendingRewardChoices = new System.Collections.Generic.List<int>();

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
            TileView.OnTileClicked += HandleTileViewClicked;
            if (_exchangeConfirmButton != null)
                _exchangeConfirmButton.onClick.AddListener(OnExchangeConfirmClicked);
            if (_exchangeCancelButton != null)
                _exchangeCancelButton.onClick.AddListener(OnExchangeCancelClicked);

            StartCoroutine(BeginTurnLoopAfterUiLayout());
        }

        private void OnDestroy()
        {
            if (_cardPanel != null)
                _cardPanel.OnHandCardClicked -= OnHandCardClicked;
            TileView.OnTileClicked -= HandleTileViewClicked;
            SetExchangeFocusUI(enabled: false);
            SetRewardFocusUI(enabled: false);
            ClearExchangePreviewTiles();
            if (_rewardPanelRoot != null)
                _rewardPanelRoot.SetActive(false);
            if (_exchangeConfirmButton != null)
                _exchangeConfirmButton.onClick.RemoveListener(OnExchangeConfirmClicked);
            if (_exchangeCancelButton != null)
                _exchangeCancelButton.onClick.RemoveListener(OnExchangeCancelClicked);
            if (_rewardSkipButton != null)
                _rewardSkipButton.onClick.RemoveListener(OnRewardSkipClicked);

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
                if (_isRewardSelectionPending)
                    return;
                if (_isExchangePending)
                    CancelPendingExchange("phase advance");
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
            CancelPendingExchange("turn start");
            TryStartPeriodicRewardSelection();
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
            CancelPendingExchange("leave card use");
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
            _cardPanel.SetInteractable(isCardUse && !_isExchangePending && !_isRewardSelectionPending);
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

            const string starterDrawId = "draw.random5";
            const string starterExchangeId = "exchange.2";
            const int starterDrawCopies = 9;
            const int starterExchangeCopies = 1;

            if (TryFindCardIndexById(starterDrawId, out int drawIndex) &&
                TryFindCardIndexById(starterExchangeId, out int exchangeIndex))
            {
                int total = starterDrawCopies + starterExchangeCopies;
                var starter = new int[total];
                int cursor = 0;
                for (int i = 0; i < starterDrawCopies; i++)
                    starter[cursor++] = drawIndex;
                for (int i = 0; i < starterExchangeCopies; i++)
                    starter[cursor++] = exchangeIndex;

                _cardDeck.SetDeck(starter);
                Debug.Log($"[CARD] Starter deck initialized: {starterDrawId}x{starterDrawCopies}, {starterExchangeId}x{starterExchangeCopies}");
                return;
            }

            // Fallback if card id mapping is broken.
            var fallback = new int[CardRegistry.DefaultCards.Count];
            for (int i = 0; i < fallback.Length; i++)
                fallback[i] = i;

            _cardDeck.SetDeck(fallback);
            Debug.LogWarning("[CARD] Starter deck id mapping failed. Fallback to full registered card set.");
        }

        private void TryStartPeriodicRewardSelection()
        {
            if (_turnState == null || _cardDeck == null)
                return;
            if (_rewardEveryTurns <= 0)
                return;
            if (_turnState.TurnIndex <= 0 || _turnState.TurnIndex % _rewardEveryTurns != 0)
                return;

            if (!_rewardPoolBuilt)
                BuildRewardPool();
            if (_rewardPoolCardIndices.Count == 0)
                return;

            _pendingRewardChoices.Clear();
            var picks = new System.Collections.Generic.List<int>(_rewardPoolCardIndices);
            for (int i = picks.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (picks[i], picks[j]) = (picks[j], picks[i]);
            }

            int choiceCount = Mathf.Min(3, picks.Count);
            for (int i = 0; i < choiceCount; i++)
                _pendingRewardChoices.Add(picks[i]);

            if (_pendingRewardChoices.Count == 0)
                return;

            _isRewardSelectionPending = true;
            EnsureRewardPanel();
            RebuildRewardChoicesUI();
            SetRewardFocusUI(enabled: true);
            UpdateCardPanelState();
            Debug.Log($"[REWARD] Turn {_turnState.TurnIndex}: choose 1 of {_pendingRewardChoices.Count} cards.");
        }

        private void EnsureRewardPanel()
        {
            if (_rewardPanelRoot != null && _rewardTitleText != null && _rewardStatusText != null && _rewardChoicesRoot != null && _rewardSkipButton != null)
            {
                EnsureRewardChoicesLayout();
                return;
            }

            var parent = FindRootCanvasRect();
            if (parent == null)
                return;

            if (_rewardPanelRoot == null)
            {
                var rootGo = new GameObject("RewardOverlay", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup));
                rootGo.transform.SetParent(parent, false);
                _rewardPanelRoot = rootGo;

                var rootRt = rootGo.GetComponent<RectTransform>();
                rootRt.anchorMin = new Vector2(0.5f, 0.5f);
                rootRt.anchorMax = new Vector2(0.5f, 0.5f);
                rootRt.pivot = new Vector2(0.5f, 0.5f);
                rootRt.sizeDelta = new Vector2(980f, 560f);
                rootRt.anchoredPosition = Vector2.zero;

                var bg = rootGo.GetComponent<Image>();
                bg.color = new Color(0.05f, 0.07f, 0.10f, 0.97f);
                bg.raycastTarget = true;

                var vlg = rootGo.GetComponent<VerticalLayoutGroup>();
                vlg.padding = new RectOffset(18, 18, 16, 16);
                vlg.spacing = 10f;
                vlg.childAlignment = TextAnchor.UpperLeft;
                vlg.childControlHeight = true;
                vlg.childControlWidth = true;
                vlg.childForceExpandHeight = false;
                vlg.childForceExpandWidth = true;

                rootGo.transform.SetAsLastSibling();
            }

            var root = _rewardPanelRoot.transform;
            var builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (_rewardTitleText == null)
            {
                var titleGo = new GameObject("TitleText", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
                titleGo.transform.SetParent(root, false);
                _rewardTitleText = titleGo.GetComponent<Text>();
                _rewardTitleText.font = builtinFont;
                _rewardTitleText.fontSize = 26;
                _rewardTitleText.fontStyle = FontStyle.Bold;
                _rewardTitleText.alignment = TextAnchor.UpperLeft;
                _rewardTitleText.color = Color.white;
                var le = titleGo.GetComponent<LayoutElement>();
                le.preferredHeight = 40f;
            }

            if (_rewardStatusText == null)
            {
                var infoGo = new GameObject("StatusText", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
                infoGo.transform.SetParent(root, false);
                _rewardStatusText = infoGo.GetComponent<Text>();
                _rewardStatusText.font = builtinFont;
                _rewardStatusText.fontSize = 18;
                _rewardStatusText.alignment = TextAnchor.UpperLeft;
                _rewardStatusText.color = new Color(0.86f, 0.92f, 0.98f, 0.95f);
                var le = infoGo.GetComponent<LayoutElement>();
                le.preferredHeight = 30f;
            }

            if (_rewardChoicesRoot == null)
            {
                var rowGo = new GameObject("ChoicesRow", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
                rowGo.transform.SetParent(root, false);
                _rewardChoicesRoot = rowGo.transform;
                var le = rowGo.GetComponent<LayoutElement>();
                le.minHeight = 0f;
                le.preferredHeight = -1f;
                le.flexibleHeight = 1f;
                le.flexibleWidth = 1f;
            }
            EnsureRewardChoicesLayout();

            Transform footerRow = root.Find("RewardFooterRow");
            if (footerRow == null)
            {
                var rowGo = new GameObject("RewardFooterRow", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
                rowGo.transform.SetParent(root, false);
                footerRow = rowGo.transform;
                var hlg = rowGo.GetComponent<HorizontalLayoutGroup>();
                hlg.spacing = 8f;
                hlg.childAlignment = TextAnchor.MiddleRight;
                hlg.childControlHeight = true;
                hlg.childControlWidth = false;
                hlg.childForceExpandHeight = false;
                hlg.childForceExpandWidth = false;
                var le = rowGo.GetComponent<LayoutElement>();
                le.preferredHeight = 40f;
            }

            if (_rewardSkipButton == null)
                _rewardSkipButton = CreateRuntimeButton(footerRow, "보상 건너뛰기");

            if (_rewardSkipButton != null)
            {
                var le = _rewardSkipButton.GetComponent<LayoutElement>();
                if (le == null)
                    le = _rewardSkipButton.gameObject.AddComponent<LayoutElement>();
                le.minWidth = 170f;
                le.preferredWidth = 170f;
                le.flexibleWidth = 0f;

                var text = _rewardSkipButton.GetComponentInChildren<Text>(true);
                if (text != null)
                {
                    text.text = "보상 건너뛰기";
                    text.fontSize = 16;
                    text.alignment = TextAnchor.MiddleCenter;
                }

                _rewardSkipButton.onClick.RemoveListener(OnRewardSkipClicked);
                _rewardSkipButton.onClick.AddListener(OnRewardSkipClicked);
                _rewardSkipButton.interactable = false;
            }
        }

        private void EnsureRewardChoicesLayout()
        {
            if (_rewardChoicesRoot == null)
                return;

            var hlg = _rewardChoicesRoot.GetComponent<HorizontalLayoutGroup>();
            if (hlg == null)
                hlg = _rewardChoicesRoot.gameObject.AddComponent<HorizontalLayoutGroup>();

            // Keep prefab-authored card size instead of forcing width/height from row layout.
            hlg.spacing = 14f;
            hlg.childAlignment = TextAnchor.UpperCenter;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;
            hlg.childScaleWidth = true;
            hlg.childScaleHeight = true;
        }

        private void RebuildRewardChoicesUI()
        {
            EnsureRewardPanel();
            if (_rewardPanelRoot != null)
                _rewardPanelRoot.SetActive(_isRewardSelectionPending);
            if (!_isRewardSelectionPending || _rewardChoicesRoot == null)
                return;

            _rewardTitleText.text = "카드 보상 선택";
            _rewardStatusText.text = "3턴 보상: 카드 1장을 선택하거나 건너뛸 수 있습니다.";

            for (int i = _rewardChoicesRoot.childCount - 1; i >= 0; i--)
                Destroy(_rewardChoicesRoot.GetChild(i).gameObject);

            for (int i = 0; i < _pendingRewardChoices.Count; i++)
            {
                int cardIndex = _pendingRewardChoices[i];
                Button btn = null;
                if (_cardPanel != null)
                    btn = _cardPanel.CreatePreviewCardButton(
                        _rewardChoicesRoot,
                        cardIndex,
                        () => OnRewardChoiceSelected(cardIndex),
                        _rewardCardPreviewScale);
                if (btn == null)
                    btn = CreateRuntimeButton(_rewardChoicesRoot, string.Empty);

                var label = btn.GetComponentInChildren<Text>(true);
                if (_cardPanel == null || btn.GetComponent<CardView>() == null)
                {
                    if (label != null)
                    {
                        label.gameObject.SetActive(true);
                        if (CardRegistry.TryGetDefinition(cardIndex, out var def) && def != null)
                            label.text = string.IsNullOrWhiteSpace(def.name) ? def.id : def.name;
                        else
                            label.text = $"Card {cardIndex}";
                        label.alignment = TextAnchor.MiddleCenter;
                    }
                }
            }

            if (_rewardSkipButton != null)
                _rewardSkipButton.interactable = true;
        }

        private void OnRewardChoiceSelected(int rewardCardIndex)
        {
            if (!_isRewardSelectionPending || _cardDeck == null)
                return;

            if (!_cardDeck.TryAddCardToDiscard(rewardCardIndex))
                return;

            string rewardName = $"Card {rewardCardIndex}";
            if (CardRegistry.TryGetDefinition(rewardCardIndex, out var def) && def != null && !string.IsNullOrWhiteSpace(def.name))
                rewardName = def.name;

            Debug.Log($"[REWARD] Selected '{rewardName}' (added to discard).");
            CloseRewardPanel();
        }

        private void OnRewardSkipClicked()
        {
            if (!_isRewardSelectionPending)
                return;

            Debug.Log($"[REWARD] Skipped on turn {_turnState?.TurnIndex ?? 0}.");
            CloseRewardPanel();
        }

        private void CloseRewardPanel()
        {
            _isRewardSelectionPending = false;
            _pendingRewardChoices.Clear();
            if (_rewardPanelRoot != null)
                _rewardPanelRoot.SetActive(false);
            if (_rewardSkipButton != null)
                _rewardSkipButton.interactable = false;
            SetRewardFocusUI(enabled: false);
            UpdateCardPanelState();
            RefreshTurnStatus();
        }

        private void SetRewardFocusUI(bool enabled)
        {
            EnsureExchangeDimTargets();
            float alpha = enabled ? 0.32f : 1f;
            for (int i = 0; i < _exchangeDimCanvasGroups.Count; i++)
            {
                var cg = _exchangeDimCanvasGroups[i];
                if (cg == null)
                    continue;
                cg.alpha = alpha;
                cg.interactable = !enabled;
                cg.blocksRaycasts = !enabled;
            }
        }

        private void BuildRewardPool()
        {
            _rewardPoolCardIndices.Clear();
            _rewardPoolBuilt = true;

            // Starter set is fixed to draw.random5 x9 + exchange.2 x1.
            const string starterDrawId = "draw.random5";
            const string starterExchangeId = "exchange.2";

            var defs = CardRegistry.Definitions;
            for (int i = 0; i < defs.Count; i++)
            {
                var def = defs[i];
                if (def == null || string.IsNullOrWhiteSpace(def.id))
                    continue;

                if (string.Equals(def.id, starterDrawId, StringComparison.Ordinal) ||
                    string.Equals(def.id, starterExchangeId, StringComparison.Ordinal))
                {
                    continue;
                }

                _rewardPoolCardIndices.Add(i);
            }

            // If no extra card exists yet, allow all cards for convenience.
            if (_rewardPoolCardIndices.Count == 0)
            {
                for (int i = 0; i < defs.Count; i++)
                    _rewardPoolCardIndices.Add(i);
            }
        }

        private static bool TryFindCardIndexById(string cardId, out int cardIndex)
        {
            cardIndex = -1;
            if (string.IsNullOrWhiteSpace(cardId))
                return false;

            var defs = CardRegistry.Definitions;
            for (int i = 0; i < defs.Count; i++)
            {
                var def = defs[i];
                if (def == null || string.IsNullOrWhiteSpace(def.id))
                    continue;
                if (!string.Equals(def.id, cardId, StringComparison.Ordinal))
                    continue;

                cardIndex = i;
                return true;
            }

            return false;
        }

        private void OnHandCardClicked(int handIndex)
        {
            if (_turnState == null || _turnState.Phase != TurnPhase.CardUse)
                return;

            if (_isRewardSelectionPending)
                return;

            if (_isExchangePending)
                return;

            if (_cardDeck == null || !_cardDeck.TryGetHandCard(handIndex, out var cardIndex))
                return;

            if (TryGetExchangeDefinition(cardIndex, out var exchangeDef, out var exchangeCount))
            {
                BeginExchangeSelection(handIndex, cardIndex, exchangeDef, exchangeCount);
                return;
            }

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

        private bool TryGetExchangeDefinition(int cardIndex, out CardDefinition definition, out int maxCount)
        {
            definition = null;
            maxCount = 0;

            if (!CardRegistry.TryGetDefinition(cardIndex, out definition) || definition == null)
                return false;

            if (!string.Equals(definition.action, "exchange", StringComparison.OrdinalIgnoreCase))
                return false;

            maxCount = Mathf.Max(1, definition.count);
            return true;
        }

        private void BeginExchangeSelection(int handIndex, int cardIndex, CardDefinition definition, int maxCount)
        {
            _isExchangePending = true;
            _pendingExchangeCardHandIndex = handIndex;
            _pendingExchangeCardIndex = cardIndex;
            _pendingExchangeDefinition = definition;
            _pendingExchangeMaxCount = maxCount;
            _pendingExchangeTiles.Clear();
            _pendingExchangeTileViews.Clear();
            ClearExchangePreviewTiles();

            EnsureExchangePanel();
            SetExchangeFocusUI(enabled: true);
            UpdateExchangePanel();
            UpdateCardPanelState();
            Debug.Log($"[CARD] Exchange select start card={cardIndex} max={maxCount}");
        }

        private void HandleTileViewClicked(TileView tileView, PointerEventData.InputButton button)
        {
            if (!_isExchangePending || _turnState == null || _turnState.Phase != TurnPhase.CardUse)
                return;

            if (button != PointerEventData.InputButton.Left || tileView == null)
                return;

            if (_handTilesView != null && !tileView.transform.IsChildOf(_handTilesView.transform))
                return;

            ToggleExchangeTile(tileView);
        }

        private void ToggleExchangeTile(TileView tileView)
        {
            if (_turnState == null)
                return;

            int tileId = tileView.Id;
            int selectedTileViewIndex = _pendingExchangeTileViews.LastIndexOf(tileView);
            if (selectedTileViewIndex >= 0)
            {
                _pendingExchangeTileViews.RemoveAt(selectedTileViewIndex);
                _pendingExchangeTiles.RemoveAt(selectedTileViewIndex);
                UpdateExchangePanel();
                return;
            }

            if (_pendingExchangeTiles.Count >= _pendingExchangeMaxCount)
            {
                Debug.LogWarning($"[CARD] Exchange selection full ({_pendingExchangeMaxCount}).");
                return;
            }

            int selectedCopies = CountSelectedTile(tileId);
            int handCopies = _turnState.CountOf(tileId);
            if (selectedCopies >= handCopies)
            {
                Debug.LogWarning($"[CARD] Cannot select more tileId={tileId}.");
                return;
            }

            _pendingExchangeTileViews.Add(tileView);
            _pendingExchangeTiles.Add(tileId);
            UpdateExchangePanel();
        }

        private int CountSelectedTile(int tileId)
        {
            int count = 0;
            for (int i = 0; i < _pendingExchangeTiles.Count; i++)
                if (_pendingExchangeTiles[i] == tileId)
                    count++;
            return count;
        }

        private void OnExchangeConfirmClicked()
        {
            if (!_isExchangePending || _turnState == null || _cardDeck == null)
                return;

            if (_pendingExchangeTiles.Count <= 0)
            {
                Debug.LogWarning("[CARD] Exchange confirm ignored: no selected tiles.");
                return;
            }

            if (!CardPlayService.TryApplyExchange(_turnState, _pendingExchangeTiles, _pendingExchangeDefinition, out var reason))
            {
                Debug.LogWarning($"[CARD] Exchange failed: {reason}");
                return;
            }

            if (!_cardDeck.TryPlayHandCard(_pendingExchangeCardHandIndex, out _))
            {
                Debug.LogWarning("[CARD] Exchange card consume failed.");
                return;
            }

            Debug.Log($"[CARD] Exchange applied card={_pendingExchangeCardIndex} count={_pendingExchangeTiles.Count}");
            _pendingExchangeTileViews.Clear();
            _pendingExchangeTiles.Clear();
            _isExchangePending = false;
            _pendingExchangeCardHandIndex = -1;
            _pendingExchangeCardIndex = -1;
            _pendingExchangeMaxCount = 0;
            _pendingExchangeDefinition = null;
            ClearExchangePreviewTiles();
            SetExchangeFocusUI(enabled: false);

            if (_actionMenu != null)
                _actionMenu.RefreshUIFromState(hideActionMenu: true, clearPendingQuickMeld: true);

            UpdateExchangePanel();
            UpdateCardPanelState();
            RefreshTurnStatus();
        }

        private void OnExchangeCancelClicked()
        {
            CancelPendingExchange("cancel button");
        }

        private void CancelPendingExchange(string reason)
        {
            if (!_isExchangePending)
                return;

            _pendingExchangeTileViews.Clear();
            _pendingExchangeTiles.Clear();
            _isExchangePending = false;
            _pendingExchangeCardHandIndex = -1;
            _pendingExchangeCardIndex = -1;
            _pendingExchangeMaxCount = 0;
            _pendingExchangeDefinition = null;
            ClearExchangePreviewTiles();
            SetExchangeFocusUI(enabled: false);

            Debug.Log($"[CARD] Exchange cancelled ({reason})");
            UpdateExchangePanel();
            UpdateCardPanelState();
        }

        private void UpdateExchangePanel()
        {
            bool visible = _isExchangePending && _turnState != null && _turnState.Phase == TurnPhase.CardUse;
            if (!visible && _exchangePanelRoot == null)
                return;

            EnsureExchangePanel();
            if (_exchangePanelRoot != null)
                _exchangePanelRoot.SetActive(visible);
            if (!visible)
            {
                ClearExchangePreviewTiles();
                return;
            }

            if (_exchangeTitleText != null)
                _exchangeTitleText.text = $"교환 패 선택 (최대 {_pendingExchangeMaxCount}장)";

            if (_exchangeStatusText != null)
            {
                _exchangeStatusText.text = $"선택 {_pendingExchangeTiles.Count}/{_pendingExchangeMaxCount} · 클릭으로 선택/해제";
            }

            if (_exchangeConfirmButton != null)
                _exchangeConfirmButton.interactable = _pendingExchangeTiles.Count > 0;

            RebuildExchangeTilePreviews();
        }

        private void EnsureExchangePanel()
        {
            if (_exchangePanelRoot != null &&
                _exchangeTitleText != null &&
                _exchangeStatusText != null &&
                _exchangeSelectedTilesRoot != null &&
                _exchangeConfirmButton != null &&
                _exchangeCancelButton != null)
                return;

            var parent = FindRootCanvasRect();
            if (parent == null)
                return;

            if (_exchangePanelRoot == null)
            {
                var rootGo = new GameObject("ExchangeOverlay", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup));
                rootGo.transform.SetParent(parent, false);
                _exchangePanelRoot = rootGo;

                var rootRt = rootGo.GetComponent<RectTransform>();
                rootRt.anchorMin = new Vector2(0.5f, 0.5f);
                rootRt.anchorMax = new Vector2(0.5f, 0.5f);
                rootRt.pivot = new Vector2(0.5f, 0.5f);
                rootRt.sizeDelta = new Vector2(760f, 280f);
                rootRt.anchoredPosition = Vector2.zero;

                var bg = rootGo.GetComponent<Image>();
                bg.color = new Color(0.06f, 0.08f, 0.11f, 0.96f);
                bg.raycastTarget = true;

                var vlg = rootGo.GetComponent<VerticalLayoutGroup>();
                vlg.padding = new RectOffset(18, 18, 16, 16);
                vlg.spacing = 10f;
                vlg.childAlignment = TextAnchor.UpperLeft;
                vlg.childControlHeight = true;
                vlg.childControlWidth = true;
                vlg.childForceExpandHeight = false;
                vlg.childForceExpandWidth = true;

                rootGo.transform.SetAsLastSibling();
            }

            var root = _exchangePanelRoot.transform;
            var builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (_exchangeTitleText == null)
            {
                var titleGo = new GameObject("TitleText", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
                titleGo.transform.SetParent(root, false);
                _exchangeTitleText = titleGo.GetComponent<Text>();
                _exchangeTitleText.font = builtinFont;
                _exchangeTitleText.fontSize = 24;
                _exchangeTitleText.fontStyle = FontStyle.Bold;
                _exchangeTitleText.alignment = TextAnchor.UpperLeft;
                _exchangeTitleText.color = Color.white;
                var le = titleGo.GetComponent<LayoutElement>();
                le.preferredHeight = 36f;
            }

            if (_exchangeStatusText == null)
            {
                var textGo = new GameObject("StatusText", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
                textGo.transform.SetParent(root, false);
                _exchangeStatusText = textGo.GetComponent<Text>();
                _exchangeStatusText.font = builtinFont;
                _exchangeStatusText.fontSize = 18;
                _exchangeStatusText.alignment = TextAnchor.UpperLeft;
                _exchangeStatusText.color = new Color(0.86f, 0.92f, 0.98f, 0.95f);
                _exchangeStatusText.horizontalOverflow = HorizontalWrapMode.Wrap;
                _exchangeStatusText.verticalOverflow = VerticalWrapMode.Overflow;
                var le = textGo.GetComponent<LayoutElement>();
                le.preferredHeight = 30f;
            }

            if (_exchangeSelectedTilesFrame == null || _exchangeSelectedTilesRoot == null)
            {
                var frameGo = new GameObject("SelectedTilesFrame", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
                frameGo.transform.SetParent(root, false);
                _exchangeSelectedTilesFrame = frameGo.GetComponent<Image>();
                _exchangeSelectedTilesFrame.color = new Color(1f, 1f, 1f, 0.08f);
                var frameLe = frameGo.GetComponent<LayoutElement>();
                frameLe.preferredHeight = 126f;
                frameLe.minHeight = 126f;

                var frameRt = frameGo.GetComponent<RectTransform>();
                frameRt.anchorMin = new Vector2(0f, 0f);
                frameRt.anchorMax = new Vector2(1f, 1f);
                frameRt.pivot = new Vector2(0.5f, 0.5f);

                var contentGo = new GameObject("SelectedTilesContent", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
                contentGo.transform.SetParent(frameGo.transform, false);
                _exchangeSelectedTilesRoot = contentGo.transform;

                var contentRt = contentGo.GetComponent<RectTransform>();
                contentRt.anchorMin = new Vector2(0f, 0.5f);
                contentRt.anchorMax = new Vector2(0f, 0.5f);
                contentRt.pivot = new Vector2(0f, 0.5f);
                contentRt.anchoredPosition = new Vector2(12f, 0f);

                var hlg = contentGo.GetComponent<HorizontalLayoutGroup>();
                hlg.spacing = 12f;
                hlg.childAlignment = TextAnchor.MiddleLeft;
                hlg.childControlWidth = false;
                hlg.childControlHeight = false;
                hlg.childForceExpandWidth = false;
                hlg.childForceExpandHeight = false;

                var fitter = contentGo.GetComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            }

            Transform buttonRow = root.Find("ButtonRow");
            if (buttonRow == null)
            {
                var rowGo = new GameObject("ButtonRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
                rowGo.transform.SetParent(root, false);
                var hlg = rowGo.GetComponent<HorizontalLayoutGroup>();
                hlg.spacing = 8f;
                hlg.childAlignment = TextAnchor.MiddleRight;
                hlg.childControlHeight = true;
                hlg.childControlWidth = false;
                hlg.childForceExpandHeight = false;
                hlg.childForceExpandWidth = false;
                buttonRow = rowGo.transform;
            }

            if (_exchangeConfirmButton == null)
                _exchangeConfirmButton = CreateRuntimeButton(buttonRow, "교환");
            if (_exchangeCancelButton == null)
                _exchangeCancelButton = CreateRuntimeButton(buttonRow, "취소");

            if (_exchangeConfirmButton != null)
            {
                _exchangeConfirmButton.onClick.RemoveListener(OnExchangeConfirmClicked);
                _exchangeConfirmButton.onClick.AddListener(OnExchangeConfirmClicked);
            }

            if (_exchangeCancelButton != null)
            {
                _exchangeCancelButton.onClick.RemoveListener(OnExchangeCancelClicked);
                _exchangeCancelButton.onClick.AddListener(OnExchangeCancelClicked);
            }
        }

        private RectTransform FindRootCanvasRect()
        {
            Canvas canvas = null;
            if (_cardPanel != null)
                canvas = _cardPanel.GetComponentInParent<Canvas>();
            if (canvas == null && _handTilesView != null)
                canvas = _handTilesView.GetComponentInParent<Canvas>();
            if (canvas == null)
                canvas = FindFirstObjectByType<Canvas>();
            return canvas != null ? canvas.transform as RectTransform : null;
        }

        private void SetExchangeFocusUI(bool enabled)
        {
            EnsureExchangeDimTargets();
            float alpha = enabled ? 0.32f : 1f;
            for (int i = 0; i < _exchangeDimCanvasGroups.Count; i++)
            {
                var cg = _exchangeDimCanvasGroups[i];
                if (cg == null)
                    continue;
                cg.alpha = alpha;
                cg.interactable = !enabled;
                cg.blocksRaycasts = !enabled;
            }
        }

        private void EnsureExchangeDimTargets()
        {
            if (_exchangeDimCanvasGroups.Count > 0)
                return;

            TryAddDimTarget(_cardPanel != null ? _cardPanel.gameObject : null);
            TryAddDimTarget(_buildDoneButton != null ? _buildDoneButton.gameObject : null);
            TryAddDimTarget(_turnStatusText != null ? _turnStatusText.gameObject : null);
            TryAddDimTarget(_buildBoardRoot);
        }

        private void TryAddDimTarget(GameObject go)
        {
            if (go == null)
                return;
            if (_handTilesView != null && go == _handTilesView.gameObject)
                return;
            if (_exchangePanelRoot != null && go == _exchangePanelRoot)
                return;
            if (_rewardPanelRoot != null && go == _rewardPanelRoot)
                return;

            var cg = go.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = go.AddComponent<CanvasGroup>();
            if (!_exchangeDimCanvasGroups.Contains(cg))
                _exchangeDimCanvasGroups.Add(cg);
        }

        private void RebuildExchangeTilePreviews()
        {
            ClearExchangePreviewTiles();
            if (_exchangeSelectedTilesRoot == null)
                return;

            for (int i = 0; i < _pendingExchangeTileViews.Count; i++)
            {
                var source = _pendingExchangeTileViews[i];
                if (source == null)
                    continue;
                var preview = CreateExchangePreviewTile(source);
                if (preview != null)
                    _exchangePreviewTiles.Add(preview);
            }
        }

        private GameObject CreateExchangePreviewTile(TileView source)
        {
            if (source == null || _exchangeSelectedTilesRoot == null)
                return null;

            var clone = Instantiate(source.gameObject, _exchangeSelectedTilesRoot);
            clone.name = $"ExchangeTile_{source.Id}";

            var tileView = clone.GetComponent<TileView>();
            if (tileView != null)
                Destroy(tileView);

            var rt = clone.transform as RectTransform;
            if (rt != null)
            {
                rt.localScale = Vector3.one;
                rt.sizeDelta = new Vector2(56f, 84f);
            }

            var graphics = clone.GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < graphics.Length; i++)
            {
                if (graphics[i] != null)
                    graphics[i].raycastTarget = false;
            }

            var button = clone.GetComponent<Button>();
            if (button != null)
                button.interactable = false;

            return clone;
        }

        private void ClearExchangePreviewTiles()
        {
            for (int i = 0; i < _exchangePreviewTiles.Count; i++)
            {
                var go = _exchangePreviewTiles[i];
                if (go != null)
                    Destroy(go);
            }
            _exchangePreviewTiles.Clear();
        }

        private static Button CreateRuntimeButton(Transform parent, string label)
        {
            var go = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = new Color(0.24f, 0.34f, 0.48f, 0.96f);
            var button = go.GetComponent<Button>();
            var le = go.GetComponent<LayoutElement>();
            le.minWidth = 110f;
            le.preferredWidth = 110f;
            le.minHeight = 34f;
            le.preferredHeight = 34f;

            var textGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(go.transform, false);
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;
            var text = textGo.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 17;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.text = label;
            text.raycastTarget = false;
            return button;
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
