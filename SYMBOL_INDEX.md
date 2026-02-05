# SYMBOL_INDEX.md
### fourMelds Symbol Index (auto-generated)

> Do not edit manually. Run 	ools/generate_symbol_index.ps1 to regenerate.

- Runtime root: Assets/_Project/01_Scripts/Runtime
- Raw base: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/

---
This file is a lightweight index for:
- namespaces
- class/struct/interface/enum declarations
- method/property signatures (best-effort)
---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/AttackContext.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/AttackContext.cs

### Types
- (none)

### Members (methods)
- (none)

### Members (properties)
- public TurnIndex TurnIndex { ... }
- public PlayerStateSnapshot Player { ... }
- public EnemyStateSnapshot Enemy { ... }
- public int HeadTileId { ... }
- public bool HasHead { ... }
- public int MeldCount { ... }
- public IReadOnlyList<MeldSnapshot> Melds { ... }
- public int BaseDamage { ... }
- public SuitSummary Suits { ... }
- public IReadOnlyList<IYakuEffect> YakuEffects { ... }
- public IReadOnlyList<IRelicEffect> RelicEffects { ... }

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/EnemyStateSnapshot.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/EnemyStateSnapshot.cs

### Types
- public record EnemyStateSnapshot

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/MeldSnapshot.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/MeldSnapshot.cs

### Types
- public record MeldSnapshot

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/PlayerStateSnapshot.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/PlayerStateSnapshot.cs

### Types
- public record PlayerStateSnapshot

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/SuitSummary.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/SuitSummary.cs

### Types
- public record SuitSummary

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/TurnIndex.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/TurnIndex.cs

### Types
- (none)

### Members (methods)
- (none)

### Members (properties)
- public int Value { ... }

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Debug/DamageLogEntry.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Debug/DamageLogEntry.cs

### Types
- public record DamageLogEntry

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/DamageModifier.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/DamageModifier.cs

### Types
- public record DamageModifier

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/IRelicEffect.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/IRelicEffect.cs

### Types
- public interface IRelicEffect

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/IYakuEffect.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/IYakuEffect.cs

### Types
- public interface IYakuEffect

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/ModifyMode.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/ModifyMode.cs

### Types
- public enum ModifyMode

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/Relic_ManzuMeldBonus.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/Relic_ManzuMeldBonus.cs

### Types
- public class Relic_ManzuMeldBonus

### Members (methods)
- public bool IsActive(in AttackContext ctx)
- public void Apply(in AttackContext ctx, AttackMutableState state)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/AttackMutableState.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/AttackMutableState.cs

### Types
- public class AttackMutableState

### Members (methods)
- public void AddLog(string stepId, int before, int after, string reason)
- public int GetFinalDamage()

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/BaseDamageStep.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/BaseDamageStep.cs

### Types
- public class BaseDamageStep

### Members (methods)
- public void Apply(in AttackContext ctx, AttackMutableState state)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/ClampStep.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/ClampStep.cs

### Types
- public class ClampStep

### Members (methods)
- public void Apply(in AttackContext ctx, AttackMutableState state)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/CompressionStep.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/CompressionStep.cs

### Types
- public class CompressionStep

### Members (methods)
- public void Apply(in AttackContext ctx, AttackMutableState state)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/DamagePipeline.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/DamagePipeline.cs

### Types
- public class DamagePipeline

### Members (methods)
- public AttackResult Execute(in AttackContext ctx)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/IDamageStep.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/IDamageStep.cs

### Types
- public interface IDamageStep

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/MeldDamageComponent.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/MeldDamageComponent.cs

### Types
- public class MeldDamageComponent

### Members (methods)
- public int Evaluate()

### Members (properties)
- public int Index { ... }
- public MeldType MeldType { ... }
- public MahjongSuit Suit { ... }

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/RelicStep.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/RelicStep.cs

### Types
- public class RelicStep

### Members (methods)
- public void Apply(in AttackContext ctx, AttackMutableState state)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/SuitDamageStep.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/SuitDamageStep.cs

### Types
- public class SuitDamageStep

### Members (methods)
- public void Apply(in AttackContext ctx, AttackMutableState state)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/YakuStep.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/YakuStep.cs

### Types
- public class YakuStep

### Members (methods)
- public void Apply(in AttackContext ctx, AttackMutableState state)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Result/AttackResult.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Result/AttackResult.cs

### Types
- public record AttackResult

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Result/AttackSideEffect.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Result/AttackSideEffect.cs

### Types
- public record AttackSideEffect

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Result/SideEffectType.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Result/SideEffectType.cs

### Types
- public enum SideEffectType

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/RuntimeState/CombatState.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/RuntimeState/CombatState.cs

### Types
- public class CombatState

### Members (methods)
- public void ApplyEnemyDamage(int damage)
- public void ApplyPlayerDamage(int damage)
- public void ApplyPlayerHeal(int amount)

### Members (properties)
- public int PlayerHP { ... }
- public int EnemyHP { ... }

---

## Assets/_Project/01_Scripts/Runtime/Combat/TurnIntegration/AttackResultApplier.cs
- Namespace: FourMelds.Combat.TurnIntegration
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/TurnIntegration/AttackResultApplier.cs

### Types
- public class AttackResultApplier

### Members (methods)
- public void Apply(CombatState combatState, in AttackResult result)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/TurnIntegration/TurnAttackContextBuilder.cs
- Namespace: FourMelds.Combat.TurnIntegration
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/TurnIntegration/TurnAttackContextBuilder.cs

### Types
- public class TurnAttackContextBuilder

### Members (methods)
- public AttackContext Build(TurnState turnState, CombatState combatState)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/TurnIntegration/TurnEndAttackExecutor.cs
- Namespace: FourMelds.Combat.TurnIntegration
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/TurnIntegration/TurnEndAttackExecutor.cs

### Types
- public class TurnEndAttackExecutor

### Members (methods)
- public AttackResult Execute(in AttackContext ctx)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/TurnLoopController.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/TurnLoopController.cs

### Types
- public class TurnLoopController

### Members (methods)
- public void OnClick_BuildDone()
- public void Dev_Setup_ToitoiSanankou()
- public void Dev_Test_ToitoiSanankou()

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Yaku/YakuEffect.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Yaku/YakuEffect.cs

### Types
- public class Yaku_Tanyao
- public class Yaku_Toitoi
- public class Yaku_Sanankou
- public class Yaku_Suankou

### Members (methods)
- public bool IsActive(in AttackContext ctx)
- public void Apply(in AttackContext ctx, AttackMutableState state)
- public bool IsActive(in AttackContext ctx)
- public void Apply(in AttackContext ctx, AttackMutableState state)
- public bool IsActive(in AttackContext ctx)
- public void Apply(in AttackContext ctx, AttackMutableState state)
- public bool IsActive(in AttackContext ctx)
- public void Apply(in AttackContext ctx, AttackMutableState state)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Yaku/YakuMeta.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Yaku/YakuMeta.cs

### Types
- public class YakuMetaDb

### Members (methods)
- (none)

### Members (properties)
- public string DisplayName { ... }
- public int Order { ... }

---

## Assets/_Project/01_Scripts/Runtime/Combat/Yaku/YakuNames.cs
- Namespace: FourMelds.Combat
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Yaku/YakuNames.cs

### Types
- public class YakuNames

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Compat/IsExternalInit.cs
- Namespace: System.Runtime.CompilerServices
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Compat/IsExternalInit.cs

### Types
- public class IsExternalInit

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Action/Execute/DummyActionExecutionService.cs
- Namespace: Project.Core.Action.Execute
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Action/Execute/DummyActionExecutionService.cs

### Types
- public class DummyActionExecutionService

### Members (methods)
- public bool Execute(ActionCommand command, TurnState state, out string failReason)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Action/Execute/IActionExecutionService.cs
- Namespace: Project.Core.Action.Execute
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Action/Execute/IActionExecutionService.cs

### Types
- public interface IActionExecutionService

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Action/Query/DummyActionQueryService.cs
- Namespace: Project.Core.Action.Query
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Action/Query/DummyActionQueryService.cs

### Types
- public class DummyActionQueryService

### Members (methods)
- public ActionMenuModel Query(ActionRequest request, TurnSnapshot snapshot)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Action/Query/IactionQueryService.cs
- Namespace: Project.Core.Action.Query
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Action/Query/IactionQueryService.cs

### Types
- public interface IActionQueryService

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Melds/MeldState.cs
- Namespace: Project.Core.Melds
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Melds/MeldState.cs

### Types
- public class MeldState

### Members (methods)
- public void Fix()

### Members (properties)
- public int MeldId { ... }
- public MeldType Type { ... }
- public int[] Tiles { ... }
- public bool IsFixed { ... }

---

## Assets/_Project/01_Scripts/Runtime/Core/Melds/MeldType.cs
- Namespace: Project.Core.Melds
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Melds/MeldType.cs

### Types
- public enum MeldType

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Suits/MahjongSuit.cs
- Namespace: FourMelds.Core.Suits
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Suits/MahjongSuit.cs

### Types
- public enum MahjongSuit

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnPhase.cs
- Namespace: FourMelds.Core.Turn
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnPhase.cs

### Types
- public enum TurnPhase

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnSnapshot.cs
- Namespace: Project.Core.Turn
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnSnapshot.cs

### Types
- public class TurnSnapshot

### Members (methods)
- (none)

### Members (properties)
- public TurnPhase Phase { ... }
- public int TurnIndex { ... }
- public IReadOnlyList<int> HandTiles { ... }
- public IReadOnlyList<int> MeldIds { ... }

---

## Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnState.cs
- Namespace: Project.Core.Turn
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnState.cs

### Types
- public class TurnState

### Members (methods)
- public void SetPhase(TurnPhase phase)
- public void SetHeadTile(int tileId)
- public void AddHandTile(int tileId)
- public void AdvanceTurn()
- public void CleanupForNextTurn()
- public int CreateMeld(MeldType type, int[] tiles, bool fixedNow = false)
- public bool TryRemoveTile(int tileId)
- public int CountOf(int tileId)
- public void ClearAllHandTiles()
- public void ClearAllMelds(bool resetMeldId = true)
- public void Dev_SetHandTiles(IEnumerable<int> tiles)
- public void Dev_AddMeld(MeldType type, int[] tiles, bool fixedNow)
- public void Dev_SetMelds(params (MeldType type, bool fixedNow, int[] tiles)

### Members (properties)
- public TurnPhase Phase { ... }
- public int TurnIndex { ... }
- public int HeadTileId { ... }
- public TilePool Pool { ... }

---

## Assets/_Project/01_Scripts/Runtime/Input/Actions/ActionRequest.cs
- Namespace: Project.InputSystem
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Input/Actions/ActionRequest.cs

### Types
- (none)

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Input/Actions/ActionRequestType.cs
- Namespace: Project.InputSystem
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Input/Actions/ActionRequestType.cs

### Types
- public enum ActionRequestType

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Input/Actions/ActionTargetType.cs
- Namespace: Project.InputSystem
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Input/Actions/ActionTargetType.cs

### Types
- public enum ActionTargetType

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Input/Interfaces/IActionRequestSink.cs
- Namespace: Project.InputSystem
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Input/Interfaces/IActionRequestSink.cs

### Types
- public interface IActionRequestSink

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Input/Interfaces/IActionRequestSource.cs
- Namespace: Project.InputSystem
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Input/Interfaces/IActionRequestSource.cs

### Types
- public interface IActionRequestSource

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Input/Sources/MouseActionRequestSource.cs
- Namespace: Project.InputSystem
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Input/Sources/MouseActionRequestSource.cs

### Types
- public class MouseActionRequestSource

### Members (methods)
- public void Raise(ActionRequest request)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Tiles/MeldCandidate.cs
- Namespace: Project.Core.Tiles
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Tiles/MeldCandidate.cs

### Types
- (none)

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Tiles/MeldCandidateCalculator.cs
- Namespace: Project.Core.Tiles
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Tiles/MeldCandidateCalculator.cs

### Types
- public class MeldCandidateCalculator

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Tiles/TileCatalogue.cs
- Namespace: Project.Core.Tiles
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Tiles/TileCatalogue.cs

### Types
- public class TileCatalog

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/Tiles/TileId.cs
- Namespace: Project.Core.Tiles
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Tiles/TileId.cs

### Types
- (none)

### Members (methods)
- public bool Equals(TileId other)

### Members (properties)
- public int Value { ... }

---

## Assets/_Project/01_Scripts/Runtime/Tiles/TilePool.cs
- Namespace: Project.Core.Tiles
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Tiles/TilePool.cs

### Types
- public class TilePool

### Members (methods)
- public int GetRemain(int tileId)
- public bool TryConsume(int tileId, int count = 1)
- public bool TryDrawRandom(Func<int, bool> predicate, out int tileId)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/Controllers/ActionMenuController.cs
- Namespace: (global)
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Controllers/ActionMenuController.cs

### Types
- public class ActionMenuController

### Members (methods)
- public void Handle(ActionRequest request)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/Controllers/ActionMenuView.cs
- Namespace: (global)
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Controllers/ActionMenuView.cs

### Types
- public class ActionMenuView

### Members (methods)
- public void Show(ActionMenuModel model)
- public void Hide()

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/Controllers/MeldSlotsView.cs
- Namespace: (global)
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Controllers/MeldSlotsView.cs

### Types
- public class MeldSlotsView

### Members (methods)
- public void Render(IReadOnlyList<MeldState> melds)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/Controllers/MeldSlotView.cs
- Namespace: (global)
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Controllers/MeldSlotView.cs

### Types
- public class MeldSlotView

### Members (methods)
- public void Bind(MeldState meld)
- public void Clear()

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/Models/ActionCommand.cs
- Namespace: Project.UI.Models
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Models/ActionCommand.cs

### Types
- (none)

### Members (methods)
- (none)

### Members (properties)
- public ActionCommandType Type { ... }
- public int TargetId { ... }
- public object Payload { ... }

---

## Assets/_Project/01_Scripts/Runtime/UI/Models/ActionCommandType.cs
- Namespace: Project.UI.Models
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Models/ActionCommandType.cs

### Types
- public enum ActionCommandType

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/Models/ActionMenuModel.cs
- Namespace: Project.UI.Models
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Models/ActionMenuModel.cs

### Types
- public class ActionMenuModel

### Members (methods)
- public void Add(ActionOption option)

### Members (properties)
- public List<ActionOption> Options { ... }

---

## Assets/_Project/01_Scripts/Runtime/UI/Models/ActionOption.cs
- Namespace: Project.UI.Models
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Models/ActionOption.cs

### Types
- public class ActionOption

### Members (methods)
- (none)

### Members (properties)
- public ActionCommandType Command { ... }
- public string Label { ... }
- public bool IsDangerous { ... }
- public object Payload { ... }
- public int[] PreviewTiles { ... }

---

## Assets/_Project/01_Scripts/Runtime/UI/View/IIdentifiable.cs
- Namespace: Project.UI
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/View/IIdentifiable.cs

### Types
- public interface IIdentifiable

### Members (methods)
- (none)

### Members (properties)
- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/View/TileView.cs
- Namespace: (global)
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/View/TileView.cs

### Types
- public class TileView

### Members (methods)
- public void OnPointerClick(PointerEventData eventData)

### Members (properties)
- (none)

---

