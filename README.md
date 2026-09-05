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
- `F1`–`F5`: close, medium, high, building-orbit, or isometric camera
- `1`–`4`: cube, beam, plate, or slope
- `5`–`0`: half block, long block, pillar, wall panel, cylinder, or rod
- `R`: rotate the selected piece
- `V`: cycle modular (seamed), natural, and blockout visual treatments
- Hold left mouse and drag: place continuous rows
- Right click: delete
- `C`: pick the pointed-at piece type and rotation
- `Ctrl+Z` / `Ctrl+Y`: undo / redo
- `Ctrl+S` / `Ctrl+L`: save / load construction

## Validation

```powershell
./scripts/Test.ps1
./scripts/Build.ps1
```

Build output is written to `Builds/Windows/Stormframe.exe`. See `PLAN.md` for design decisions and milestone gates.
