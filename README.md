# Stormframe

Stormframe is an early Unity construction-game prototype about a small robot stranded on a finite island. The immediate goal is to prove that placing and combining modular pieces is enjoyable before adding weather, survival, resources, or multiplayer.

## Requirements

- Unity `6000.6.0f1` with Windows Build Support
- PowerShell 7 or Windows PowerShell

Open the repository directory as a project in Unity Hub, then load `Assets/Scenes/Prototype.unity` and press Play.

## Prototype controls

- `WASD`: move
- Middle mouse drag: orbit camera
- Mouse wheel: zoom
- `F1`: reset and refocus the camera on the robot
- `1`–`4`: cube, beam, plate, or slope
- `5`–`0`: half block, long block, pillar, wall panel, cylinder, or rod
- `R`: rotate the selected piece or captured assembly
- `V`: toggle structural support visualization (green supported, red unsupported)
- Hold left mouse: preview a line; drag to lock X, Y, or Z; release to place it
- Right click: remove a piece; disconnected pieces collapse as temporary debris
- `C`: pick the pointed-at piece type and rotation
- `B`: capture every piece connected to the pointed piece as a reusable assembly; drag to repeat it in a line
- `Esc`: leave assembly placement mode
- `Ctrl+Z` / `Ctrl+Y`: undo / redo
- `Ctrl+S` / `Ctrl+L`: save / load construction

## Validation

```powershell
./scripts/Test.ps1
./scripts/Build.ps1
```

Build output is written to `Builds/Windows/Stormframe.exe`. See `PROGRESS.md` for the live checkpoint and `PLAN.md` for long-term design decisions.
