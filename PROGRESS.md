# Development Progress

## Current State

The playable Unity prototype opens at `Assets/Scenes/Prototype.unity`. It currently includes third-person robot movement, F1/F2 construction cameras, grid-snapped building, ten modular pieces, rotation, continuous row placement, deletion, piece picking, undo/redo, and construction save/load. The preferred visual direction is modular with visible seams and no connector knobs.

Latest validated baseline: 12 automated tests pass and the Windows player builds successfully.

## Hard Development Rules

- Work directly on `main` and push completed, validated bundles.
- Use Unity `6000.6.0f1` and the Input System package.
- Development saves are disposable. Never add migrations, legacy readers, or compatibility shims. Increment the save version and reject older data after schema changes.
- Keep authoritative construction state in plain C# models; Unity objects are presentation.
- Keep the robot low enough to pass through a two-unit-high doorway.

## Decisions From Playtesting

- F2 is the default general camera; F1 is best for close building.
- Use modular pieces without stud-like knobs.
- The player is a small hovering robot stranded on a natural island.
- Do not restore the removed crash-site decoration.
- Holding and dragging the left mouse button should place continuous rows.

## Active Bundle

Finish the construction-toy milestone with player-created reusable assemblies:

1. Capture a connected group of placed pieces relative to a pointed anchor.
2. Preview and stamp the captured assembly as one validated operation.
3. Undo or redo the entire stamped assembly in one step.
4. Add controls, status feedback, tests, and save-compatible logical behavior.

## After This Bundle

Playtest reusable assemblies. If building remains enjoyable, begin the structural prototype: connection graph, support visualization, predictable failure, and temporary debris physics.
