# SYMBOL_INDEX.md
### fourMelds ??Symbol Index (auto-generated)

> Do not edit manually. Run 	ools/generate_symbol_index.ps1 to regenerate.

- Runtime root: $RuntimePath
- Raw base: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/

---

This file is a lightweight index for:
- namespaces
- class/struct/interface/enum declarations
- method/property signatures (best-effort)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/AttackContext.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/AttackContext.cs

### Types
- (none)

### Members (methods)

- (none)

### Members (properties)

- public TurnIndex TurnIndex { ... }
- public PlayerStateSnapshot Player { ... }
- public EnemyStateSnapshot Enemy { ... }
- public bool HasHead { ... }
- public int MeldCount { ... }
- public IReadOnlyList<MeldSnapshot> Melds { ... }
- public int BaseDamage { ... }
- public SuitSummary Suits { ... }
- public IReadOnlyList<IYakuEffect> YakuEffects { ... }
- public IReadOnlyList<IRelicEffect> RelicEffects { ... }
---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/SuitType.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/SuitType.cs

### Types
- public enum SuitType

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/TurnIndex.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Context/TurnIndex.cs

### Types
- (none)

### Members (methods)

- public override string ToString() => Value.ToString();
### Members (properties)

- public int Value { ... }
---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/IRelicEffect.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/IRelicEffect.cs

### Types
- public interface IRelicEffect

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/IYakuEffect.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/IYakuEffect.cs

### Types
- public interface IYakuEffect

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/ModifyMode.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/ModifyMode.cs

### Types
- public enum ModifyMode

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/Relic_ManzuMeldBonus.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Effects/Relic_ManzuMeldBonus.cs

### Types
- public sealed class Relic_ManzuMeldBonus : IRelicEffect

### Members (methods)

- public bool IsActive(in AttackContext ctx) => true;
### Members (properties)

- public string Id => "relic.manzu_meld_bonus";
---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/AttackMutableState.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/AttackMutableState.cs

### Types
- public sealed class AttackMutableState

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/BaseDamageStep.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/BaseDamageStep.cs

### Types
- public sealed class BaseDamageStep : IDamageStep

### Members (methods)

- (none)

### Members (properties)

- public string Id => "step.base";
---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/ClampStep.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/ClampStep.cs

### Types
- public sealed class ClampStep : IDamageStep

### Members (methods)

- (none)

### Members (properties)

- public string Id => "step.clamp";
---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/DamagePipeline.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/DamagePipeline.cs

### Types
- public sealed class DamagePipeline

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/IDamageStep.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/IDamageStep.cs

### Types
- public interface IDamageStep

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/MeldDamageComponent.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/MeldDamageComponent.cs

### Types
- public sealed class MeldDamageComponent

### Members (methods)

- (none)

### Members (properties)

- public int Index { ... }
- public MeldType MeldType { ... }
- public SuitType Suit { ... }
---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/RelicStep.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/RelicStep.cs

### Types
- public sealed class RelicStep : IDamageStep

### Members (methods)

- (none)

### Members (properties)

- public string Id => "step.relic";
---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/SuitDamageStep.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/SuitDamageStep.cs

### Types
- public sealed class SuitDamageStep : IDamageStep

### Members (methods)

- (none)

### Members (properties)

- public string Id => "step.suit";
---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/YakuStep.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Pipeline/YakuStep.cs

### Types
- public sealed class YakuStep : IDamageStep

### Members (methods)

- (none)

### Members (properties)

- public string Id => "step.yaku";
---

## Assets/_Project/01_Scripts/Runtime/Combat/Damage/Result/SideEffectType.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Combat/Damage/Result/SideEffectType.cs

### Types
- public enum SideEffectType

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Compat/IsExternalInit.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Compat/IsExternalInit.cs

### Types
- internal sealed class IsExternalInit { }

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Action/Execute/DummyActionExecutionService.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Action/Execute/DummyActionExecutionService.cs

### Types
- public class DummyActionExecutionService : IActionExecutionService

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Action/Execute/IActionExecutionService.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Action/Execute/IActionExecutionService.cs

### Types
- public interface IActionExecutionService

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Action/Query/DummyActionQueryService.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Action/Query/DummyActionQueryService.cs

### Types
- public class DummyActionQueryService : IActionQueryService

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Action/Query/IactionQueryService.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Action/Query/IactionQueryService.cs

### Types
- public interface IActionQueryService

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Melds/MeldState.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Melds/MeldState.cs

### Types
- public class MeldState

### Members (methods)

- (none)

### Members (properties)

- public int MeldId { ... }
- public MeldType Type { ... }
- public int[] Tiles { ... }
- public bool IsFixed { ... }
---

## Assets/_Project/01_Scripts/Runtime/Core/Melds/MeldType.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Melds/MeldType.cs

### Types
- public enum MeldType

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnAttackTestRunner.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnAttackTestRunner.cs

### Types
- public sealed class TurnAttackTestRunner : MonoBehaviour

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnPhase.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnPhase.cs

### Types
- public enum TurnPhase

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnSnapshot.cs
- Namespace: $nsName
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
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnState.cs

### Types
- public class TurnState

### Members (methods)

- (none)

### Members (properties)

- public IReadOnlyList<int> HandTiles => _handTiles;
- public IReadOnlyList<MeldState> Melds => _melds;
---

## Assets/_Project/01_Scripts/Runtime/Input/Actions/ActionRequestType.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Input/Actions/ActionRequestType.cs

### Types
- public enum ActionRequestType

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Input/Actions/ActionTargetType.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Input/Actions/ActionTargetType.cs

### Types
- public enum ActionTargetType

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Input/Interfaces/IActionRequestSink.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Input/Interfaces/IActionRequestSink.cs

### Types
- public interface IActionRequestSink

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Input/Interfaces/IActionRequestSource.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Input/Interfaces/IActionRequestSource.cs

### Types
- public interface IActionRequestSource

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Input/Sources/MouseActionRequestSource.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Input/Sources/MouseActionRequestSource.cs

### Types
- public class MouseActionRequestSource : MonoBehaviour, IActionRequestSource

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Tiles/MeldCandidate.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Tiles/MeldCandidate.cs

### Types
- (none)

### Members (methods)

- public override string ToString() => $"{A},{B},{C}";
### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Tiles/MeldCandidateCalculator.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Tiles/MeldCandidateCalculator.cs

### Types
- public static class MeldCandidateCalculator

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Tiles/SuitType.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Tiles/SuitType.cs

### Types
- public enum SuitType

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/Tiles/TileId.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/Tiles/TileId.cs

### Types
- (none)

### Members (methods)

- public override string ToString() => $"{Suit}:{Rank} ({Value})";
- public bool Equals(TileId other) => Value == other.Value;
- public override bool Equals(object obj) => obj is TileId other && Equals(other);
- public override int GetHashCode() => Value;
### Members (properties)

- public int Value { ... }
- public SuitType Suit => (SuitType)(Value / 100);
- public int Rank => Value % 100;
- public bool IsNumberSuit => Suit != SuitType.Honor;
- public bool IsHonor => Suit == SuitType.Honor;
---

## Assets/_Project/01_Scripts/Runtime/UI/Controllers/ActionMenuController.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Controllers/ActionMenuController.cs

### Types
- public class ActionMenuController : MonoBehaviour, IActionRequestSink

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/Controllers/ActionMenuView.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Controllers/ActionMenuView.cs

### Types
- public class ActionMenuView : MonoBehaviour

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/Controllers/MeldSlotsView.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Controllers/MeldSlotsView.cs

### Types
- public class MeldSlotsView : MonoBehaviour

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/Controllers/MeldSlotView.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Controllers/MeldSlotView.cs

### Types
- public class MeldSlotView : MonoBehaviour

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/Models/ActionCommand.cs
- Namespace: $nsName
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
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Models/ActionCommandType.cs

### Types
- public enum ActionCommandType

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/Models/ActionMenuModel.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/Models/ActionMenuModel.cs

### Types
- public class ActionMenuModel

### Members (methods)

- public void Add(ActionOption option) => Options.Add(option);
### Members (properties)

- public List<ActionOption> Options { ... }
---

## Assets/_Project/01_Scripts/Runtime/UI/Models/ActionOption.cs
- Namespace: $nsName
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
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/View/IIdentifiable.cs

### Types
- public interface IIdentifiable

### Members (methods)

- (none)

### Members (properties)

- (none)

---

## Assets/_Project/01_Scripts/Runtime/UI/View/TileView.cs
- Namespace: $nsName
- Raw: https://raw.githubusercontent.com/sunhokim1/fourMelds/main/Assets/_Project/01_Scripts/Runtime/UI/View/TileView.cs

### Types
- public class TileView : MonoBehaviour, IIdentifiable, IPointerClickHandler

### Members (methods)

- (none)

### Members (properties)

- public int Id => id;
