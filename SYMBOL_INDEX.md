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
