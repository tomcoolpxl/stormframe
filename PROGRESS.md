# Development Progress

## Current State

The playable Unity prototype opens at `Assets/Scenes/Prototype.unity`. It currently includes third-person robot movement, F1/F2 construction cameras, grid-snapped building, ten modular pieces, rotation, transactional line drawing, deletion, piece picking, reusable assemblies, undo/redo, and construction save/load. The preferred visual direction is modular with visible seams and no connector knobs.

Latest validated baseline: 22 automated tests pass and the Windows player builds successfully.

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
- Line placement is orthogonal only. Mouse-down fixes the start; dragging locks X, Y, or Z and changes an unfixed preview; mouse-up commits the complete line.
- Construction input must never unexpectedly reframe away from the robot.

## Completed Bundles

- Unity foundation, Input System movement, and five camera experiments.
- Ten-piece construction palette with half-height vertical snapping.
- Modular no-knob visuals and a closed slope mesh.
- Transactional orthogonal line placement, picking, delete, undo/redo, and save/load.
- Player-created reusable assemblies:
  - `B` captures the connected component containing the pointed piece.
  - The captured group receives a full green/red placement preview.
  - `R` rotates the group and left-drag previews a repeated orthogonal line.
  - Mouse release validates and commits the complete line as one atomic command.

## Line Drawing Behavior

- No real pieces are created while the mouse is held, so previews cannot alter their own raycast target.
- After a short drag threshold, screen direction selects and locks the closest projected world axis.
- The selected piece or assembly footprint controls spacing, including half-height vertical layers.
- Diagonal lines are impossible, blocked lines place nothing, and one undo removes the full line.
- Lines are capped at 128 stamps to protect the prototype from accidental runaway placement.

## Camera Framing Guarantees

- F1 and F2 always focus on the robot and discard any previous construction focus.
- F4 and F5 frame a bounded midpoint between the robot and construction pointer instead of replacing the robot as the focus.
- Obstacle correction is smoothed and cannot collapse the camera to a near-zero distance.

## Active Gate

Playtest orthogonal line drawing and reusable assemblies. Confirm axis choice, vertical intent, preview readability, footprint spacing, capture scope, and rotation feel natural.

## After This Bundle

If assembly building feels good, begin the structural prototype as one bundle: support calculation, structural visualization, predictable failure, and temporary debris physics.
