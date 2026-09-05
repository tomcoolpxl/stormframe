# Development Progress

## Current State

The playable Unity prototype opens at `Assets/Scenes/Prototype.unity`. It currently includes third-person robot movement, F1/F2 construction cameras, grid-snapped building, ten modular pieces, rotation, continuous row placement, deletion, piece picking, undo/redo, and construction save/load. The preferred visual direction is modular with visible seams and no connector knobs.

Latest validated baseline: 16 automated tests pass and the Windows player builds successfully.

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

## Completed Bundles

- Unity foundation, Input System movement, and five camera experiments.
- Ten-piece construction palette with half-height vertical snapping.
- Modular no-knob visuals and a closed slope mesh.
- Continuous row placement, picking, delete, undo/redo, and save/load.
- Player-created reusable assemblies:
  - `B` captures the connected component containing the pointed piece.
  - The captured group receives a full green/red placement preview.
  - `R` rotates the group and left-click stamps it repeatedly.
  - Each stamp validates and undoes/redoes as one atomic command.

## Active Gate

Playtest reusable assemblies in the prototype scene. Confirm that capture scope, anchor choice, rotation, preview readability, and repeated stamping feel natural.

## After This Bundle

If assembly building feels good, begin the structural prototype as one bundle: support calculation, structural visualization, predictable failure, and temporary debris physics.
