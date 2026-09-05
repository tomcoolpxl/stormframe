# Repository Guidelines

## Project Structure & Module Organization

This repository contains a Unity prototype. `PLAN.md` is the source of truth for scope, architecture, and milestone order. Use Unity 6000.6.0f1 and target Windows first.

When adding the Unity project, keep gameplay code under `Assets/Scripts/`, editor automation under `Assets/Editor/`, data definitions under `Assets/Data/`, and tests under `Assets/Tests/EditMode/` or `Assets/Tests/PlayMode/`. Commit `Packages/` and `ProjectSettings/`; do not commit generated directories such as `Library/`, `Temp/`, `Logs/`, or `Builds/`.

## Build, Test, and Development Commands

Use the repository scripts from PowerShell:

```powershell
./scripts/Test.ps1
./scripts/Build.ps1
```

The first command runs Edit Mode and Play Mode tests. The second creates `Builds/Windows/Stormframe.exe`. Both resolve the editor version from `ProjectSettings/ProjectVersion.txt`. For interactive work, add this directory in Unity Hub.

## Coding Style & Naming Conventions

Use four-space indentation in C#. Name types and public members with `PascalCase`, locals and parameters with `camelCase`, interfaces with an `I` prefix, and private fields with `_camelCase`. Keep one primary type per file and match its filename (for example, `PlacePieceCommand.cs`).

Keep authoritative simulation state separate from `GameObject` presentation. Prefer explicit, serializable configuration, stable entity identifiers, command-based mutations, and event-driven updates. Avoid important behavior hidden in Inspector-only values or scattered `MonoBehaviour.Update()` methods.

## Testing Guidelines

Use Unity Test Framework tests. Put deterministic simulation and validation tests in Edit Mode; reserve Play Mode for scene, input, physics, and presentation integration. Name fixtures `*Tests.cs` and tests by behavior, such as `PlacePiece_RejectsUnsupportedConnection`. Each milestone must test its design question, not merely feature completion. The first construction prototype should also sanity-check 1,000+ visible pieces.

## Commit & Pull Request Guidelines

No Git history is present to infer an established convention. Use short, imperative subjects with a focused prefix, such as `feat: add grid snapping` or `docs: refine camera criteria`. Keep commits narrowly scoped.

Pull requests should state the milestone or question addressed, summarize behavior changes, list tests performed, and link relevant issues. Include screenshots or short captures for camera, construction, rendering, physics, or editor-tool changes. Call out new Inspector setup and update `PLAN.md` when a decision gate changes.
