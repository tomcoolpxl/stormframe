# Development Progress

## Current State

The playable Unity prototype opens at `Assets/Scenes/Prototype.unity`. It includes third-person robot movement, one refocusable construction camera, grid-snapped building, ten modular pieces, rotation, transactional line drawing, deletion and collapse, support visualization, piece picking, reusable assemblies, undo/redo, and construction save/load. The visual direction is modular with visible seams and no connector knobs.

Latest validated baseline: 26 automated tests pass and the Windows player builds successfully.

## Hard Development Rules

- Work directly on `main` and push completed, validated bundles.
- Use Unity `6000.6.0f1` and the Input System package.
- Development saves are disposable. Never add migrations, legacy readers, or compatibility shims. Increment the save version and reject older data after schema changes.
- Keep authoritative construction state in plain C# models; Unity objects are presentation.
- Keep the robot low enough to pass through a two-unit-high doorway.

## Decisions From Playtesting

- Use one third-person camera. `F1` resets and refocuses it; there are no selectable camera modes.
- Modular is the only construction visual style. Use visible seams without stud-like knobs.
- The player is a small hovering robot stranded on a natural island.
- Do not restore the removed crash-site decoration.
- Line placement is orthogonal only. Mouse-down fixes the start; dragging locks X, Y, or Z and changes an unfixed preview; mouse-up commits the complete line.
- Construction input must never unexpectedly reframe away from the robot.

## Completed Bundles

- Unity foundation, Input System movement, and a single refocusable camera selected after camera experiments.
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

- The camera always focuses on the robot; construction input never changes its focus target.
- `F1` restores the standard yaw, pitch, distance, field of view, and robot focus.
- Obstacle correction is smoothed and cannot collapse the camera to a near-zero distance.

## Structural Prototype

- Every piece touching ground is a support root. Support propagates through face-adjacent occupied grid cells.
- `V` toggles the structural overlay: green is connected to ground and red is unsupported.
- Removing a piece recalculates support immediately. Newly disconnected pieces collapse together.
- Collapsed pieces become physical debris for four seconds, then disappear.
- Removal and the complete resulting collapse are one undoable command.

## Active Gate

Playtest structural readability and failure. Confirm that support paths are understandable, expected structures remain standing, collapse feels predictable, and debris is useful rather than noisy. Continue checking line-axis choice and reusable assemblies.

## After This Bundle

If structural behavior feels good, begin the storm prototype as one bundle: controllable wind and rain, structural loading, clear damage feedback, and a resettable test storm.
