using System.Collections.Generic;
using System.IO;
using Stormframe.Construction.Commands;
using Stormframe.Construction.Persistence;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Stormframe.Construction
{
    public sealed class ConstructionController : MonoBehaviour
    {
        private readonly ConstructionWorld _world = new();
        private readonly ConstructionCommandHistory _history = new();
        private readonly Dictionary<System.Guid, GameObject> _views = new();
        private readonly List<GameObject> _assemblyGhosts = new();
        private Camera _camera;
        private GameObject _ghost;
        private ConstructionAssembly _assembly;
        private ConstructionAssembly _linePreview;
        private PieceKind _selectedKind;
        private bool _showStructuralSupport;
        private Vector3Int _candidateCell;
        private int _quarterTurns;
        private bool _hasCandidate;
        private bool _isDrawingLine;
        private bool _lineAxisLocked;
        private Vector2 _lineStartMouse;
        private Vector3Int _lineStartCell;
        private ConstructionAxis _lineAxis;
        private int _lineSegments;
        private string _status = "Ready";

        private const float AxisLockPixels = 14f;
        private const int MaximumLineSegments = 127;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Keyboard.current == null || Mouse.current == null) return;

            ReadSelection();
            ReadEditingCommands();
            UpdateCandidate();

            if (Keyboard.current.rKey.wasPressedThisFrame && !_isDrawingLine)
            {
                _quarterTurns = (_quarterTurns + 1) % 4;
                RebuildGhost();
            }

            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                ToggleStructuralSupport();
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame && _isDrawingLine)
            {
                CancelLine("Line cancelled");
            }
            else if (Keyboard.current.escapeKey.wasPressedThisFrame && _assembly != null)
            {
                _assembly = null;
                RebuildGhost();
                _status = "Assembly mode cancelled";
            }

            if (Mouse.current.leftButton.wasPressedThisFrame && _hasCandidate)
            {
                BeginLine();
            }

            if (_isDrawingLine && Mouse.current.leftButton.isPressed)
            {
                UpdateLinePreview();
            }

            if (_isDrawingLine && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                CommitLine();
            }

            if (!_isDrawingLine && Mouse.current.rightButton.wasPressedThisFrame)
            {
                RemovePointedPiece();
            }
        }

        private void ReadEditingCommands()
        {
            Keyboard keyboard = Keyboard.current;
            bool control = keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed;

            if (control && keyboard.zKey.wasPressedThisFrame && _history.Undo(_world))
            {
                RebuildViews();
                _status = "Undo";
            }
            else if (control && keyboard.yKey.wasPressedThisFrame && _history.Redo(_world))
            {
                RebuildViews();
                _status = "Redo";
            }
            else if (control && keyboard.sKey.wasPressedThisFrame)
            {
                SaveConstruction();
            }
            else if (control && keyboard.lKey.wasPressedThisFrame)
            {
                LoadConstruction();
            }
            else if (keyboard.cKey.wasPressedThisFrame)
            {
                PickPointedPiece();
            }
            else if (keyboard.bKey.wasPressedThisFrame)
            {
                CapturePointedAssembly();
            }
        }

        private void ReadSelection()
        {
            PieceKind previous = _selectedKind;
            if (Keyboard.current.digit1Key.wasPressedThisFrame) _selectedKind = PieceKind.Cube;
            if (Keyboard.current.digit2Key.wasPressedThisFrame) _selectedKind = PieceKind.Beam;
            if (Keyboard.current.digit3Key.wasPressedThisFrame) _selectedKind = PieceKind.Plate;
            if (Keyboard.current.digit4Key.wasPressedThisFrame) _selectedKind = PieceKind.Slope;
            if (Keyboard.current.digit5Key.wasPressedThisFrame) _selectedKind = PieceKind.HalfBlock;
            if (Keyboard.current.digit6Key.wasPressedThisFrame) _selectedKind = PieceKind.LongBlock;
            if (Keyboard.current.digit7Key.wasPressedThisFrame) _selectedKind = PieceKind.Pillar;
            if (Keyboard.current.digit8Key.wasPressedThisFrame) _selectedKind = PieceKind.WallPanel;
            if (Keyboard.current.digit9Key.wasPressedThisFrame) _selectedKind = PieceKind.Cylinder;
            if (Keyboard.current.digit0Key.wasPressedThisFrame) _selectedKind = PieceKind.Rod;
            if (previous != _selectedKind)
            {
                if (_isDrawingLine) CancelLine("Line cancelled");
                _assembly = null;
                RebuildGhost();
            }
        }

        private void UpdateCandidate()
        {
            if (_isDrawingLine)
            {
                return;
            }

            _hasCandidate = TryRaycast(out RaycastHit hit);
            if (!_hasCandidate)
            {
                if (_ghost != null) _ghost.SetActive(false);
                foreach (GameObject assemblyGhost in _assemblyGhosts) assemblyGhost.SetActive(false);
                return;
            }

            _candidateCell = ConstructionGrid.SurfaceToCell(hit.point, hit.normal);

            if (_assembly != null)
            {
                UpdateAssemblyGhosts(_assembly, _candidateCell, _quarterTurns);
                return;
            }

            if (_ghost == null) RebuildGhost();
            _ghost.SetActive(true);
            _ghost.transform.position = ConstructionGrid.CellToWorld(_candidateCell)
                + PieceGeometry.VisualOffset(_selectedKind, _quarterTurns);
            _ghost.transform.rotation = Quaternion.Euler(0f, _quarterTurns * 90f, 0f);
            bool valid = _world.CanPlace(_selectedKind, _candidateCell, _quarterTurns);
            _ghost.GetComponent<PieceVisual>().SetPlacementValidity(valid);
        }

        private void BeginLine()
        {
            _isDrawingLine = true;
            _lineAxisLocked = false;
            _lineSegments = 0;
            _lineStartCell = _candidateCell;
            _lineStartMouse = Mouse.current.position.ReadValue();
            RefreshLinePreview();
        }

        private void UpdateLinePreview()
        {
            Vector2 drag = Mouse.current.position.ReadValue() - _lineStartMouse;
            if (!_lineAxisLocked && drag.magnitude >= AxisLockPixels)
            {
                _lineAxis = SelectLineAxis(drag);
                _lineAxisLocked = true;
            }

            int segments = _lineAxisLocked ? CalculateLineSegments(drag, _lineAxis) : 0;
            if (segments == _lineSegments) return;
            _lineSegments = segments;
            RefreshLinePreview();
        }

        private void CommitLine()
        {
            ConstructionAssembly completedLine = _linePreview;
            int stampCount = Mathf.Abs(_lineSegments) + 1;
            int pieceCount = completedLine.PieceCount;
            bool placed = _history.Execute(
                new PlaceAssemblyCommand(completedLine, Vector3Int.zero, 0),
                _world);

            _isDrawingLine = false;
            _linePreview = null;
            RebuildGhost();
            if (!placed)
            {
                _status = "Line blocked — nothing placed";
                return;
            }

            RebuildViews();
            _status = $"Placed {stampCount}-stamp line ({pieceCount} pieces)";
        }

        private void CancelLine(string status)
        {
            _isDrawingLine = false;
            _linePreview = null;
            RebuildGhost();
            _status = status;
        }

        private void RemovePointedPiece()
        {
            if (!TryRaycast(out RaycastHit hit)) return;
            PlacedPieceView view = hit.collider.GetComponentInParent<PlacedPieceView>();
            if (view == null) return;
            var command = new DemolishPieceCommand(view.Anchor);
            if (!_history.Execute(command, _world)) return;
            RebuildViews();
            SpawnCollapseDebris(command.CollapsedPieces);
            _status = command.CollapsedPieces.Count == 0
                ? "Removed piece; structure remains supported"
                : $"Removed support; {command.CollapsedPieces.Count} piece(s) collapsed";
        }

        private bool TryRaycast(out RaycastHit hit)
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            return Physics.Raycast(ray, out hit, 250f, ~LayerMask.GetMask("Ignore Raycast"));
        }

        private void RebuildGhost()
        {
            if (_ghost != null) Destroy(_ghost);
            _ghost = null;
            foreach (GameObject assemblyGhost in _assemblyGhosts) Destroy(assemblyGhost);
            _assemblyGhosts.Clear();

            if (_isDrawingLine && _linePreview != null)
            {
                UpdateAssemblyGhosts(_linePreview, Vector3Int.zero, 0);
                return;
            }

            if (_assembly != null)
            {
                UpdateAssemblyGhosts(_assembly, _candidateCell, _quarterTurns);
                return;
            }

            var preview = new PlacedPiece(System.Guid.Empty, _selectedKind, _candidateCell, _quarterTurns);
            _ghost = PieceViewFactory.Create(preview, true);
        }

        private void UpdateAssemblyGhosts(
            ConstructionAssembly assembly,
            Vector3Int origin,
            int quarterTurns)
        {
            IReadOnlyList<AssemblyPlacement> placements = assembly.GetPlacements(origin, quarterTurns);
            if (_assemblyGhosts.Count != placements.Count)
            {
                foreach (GameObject assemblyGhost in _assemblyGhosts) Destroy(assemblyGhost);
                _assemblyGhosts.Clear();
                foreach (AssemblyPlacement placement in placements)
                {
                    var preview = new PlacedPiece(
                        System.Guid.Empty,
                        placement.Kind,
                        placement.Anchor,
                        placement.QuarterTurns);
                    _assemblyGhosts.Add(PieceViewFactory.Create(preview, true));
                }
            }

            bool valid = assembly.CanPlace(_world, origin, quarterTurns);
            for (int index = 0; index < placements.Count; index++)
            {
                AssemblyPlacement placement = placements[index];
                GameObject assemblyGhost = _assemblyGhosts[index];
                assemblyGhost.SetActive(true);
                assemblyGhost.transform.position = ConstructionGrid.CellToWorld(placement.Anchor)
                    + PieceGeometry.VisualOffset(placement.Kind, placement.QuarterTurns);
                assemblyGhost.transform.rotation = Quaternion.Euler(0f, placement.QuarterTurns * 90f, 0f);
                assemblyGhost.GetComponent<PieceVisual>().SetPlacementValidity(valid);
            }
        }

        private void RefreshLinePreview()
        {
            ConstructionAssembly source = SelectedSourceAssembly();
            ConstructionAxis axis = _lineAxisLocked ? _lineAxis : ConstructionAxis.X;
            _linePreview = ConstructionLinePlanner.CreateLine(
                source,
                _lineStartCell,
                _quarterTurns,
                axis,
                _lineSegments);
            RebuildGhost();

            int stampCount = Mathf.Abs(_lineSegments) + 1;
            string axisLabel = _lineAxisLocked ? _lineAxis.ToString() : "waiting for direction";
            _status = $"Preview {stampCount} stamp(s) on {axisLabel}; release to place";
        }

        private ConstructionAssembly SelectedSourceAssembly()
        {
            if (_assembly != null) return _assembly;
            var piece = new PlacedPiece(System.Guid.Empty, _selectedKind, Vector3Int.zero, 0);
            return new ConstructionAssembly(new[] { piece }, Vector3Int.zero);
        }

        private ConstructionAxis SelectLineAxis(Vector2 drag)
        {
            ConstructionAxis bestAxis = ConstructionAxis.X;
            float bestScore = float.NegativeInfinity;
            foreach (ConstructionAxis axis in new[]
                     {
                         ConstructionAxis.X, ConstructionAxis.Y, ConstructionAxis.Z
                     })
            {
                Vector2 screenAxis = ScreenAxisVector(axis);
                if (screenAxis.sqrMagnitude < 16f) continue;
                float alignment = Mathf.Abs(Vector2.Dot(drag.normalized, screenAxis.normalized));
                float score = alignment + Mathf.Min(screenAxis.magnitude, 100f) * 0.0001f;
                if (score <= bestScore) continue;
                bestScore = score;
                bestAxis = axis;
            }

            return bestAxis;
        }

        private int CalculateLineSegments(Vector2 drag, ConstructionAxis axis)
        {
            Vector2 screenAxis = ScreenAxisVector(axis);
            if (screenAxis.sqrMagnitude < 16f) return 0;
            float projectedSteps = Vector2.Dot(drag, screenAxis.normalized) / screenAxis.magnitude;
            return Mathf.Clamp(Mathf.RoundToInt(projectedSteps), -MaximumLineSegments, MaximumLineSegments);
        }

        private Vector2 ScreenAxisVector(ConstructionAxis axis)
        {
            ConstructionAssembly source = SelectedSourceAssembly();
            int spacing = ConstructionLinePlanner.GetStampSpacing(source, _quarterTurns, axis);
            Vector3Int cellStep = ConstructionLinePlanner.AxisVector(axis) * spacing;
            Vector3 startWorld = ConstructionGrid.CellToWorld(_lineStartCell);
            Vector3 endWorld = ConstructionGrid.CellToWorld(_lineStartCell + cellStep);
            Vector3 startScreen = _camera.WorldToScreenPoint(startWorld);
            Vector3 endScreen = _camera.WorldToScreenPoint(endWorld);
            return new Vector2(endScreen.x - startScreen.x, endScreen.y - startScreen.y);
        }

        private void ToggleStructuralSupport()
        {
            _showStructuralSupport = !_showStructuralSupport;
            ApplyStructuralSupportOverlay();
            _status = _showStructuralSupport ? "Support overlay enabled" : "Support overlay disabled";
        }

        private void ApplyStructuralSupportOverlay()
        {
            HashSet<System.Guid> supported = _showStructuralSupport
                ? _world.GetSupportedPieceIds()
                : null;
            foreach (KeyValuePair<System.Guid, GameObject> entry in _views)
            {
                bool? isSupported = _showStructuralSupport
                    ? supported.Contains(entry.Key)
                    : null;
                entry.Value.GetComponent<PieceVisual>().SetStructuralHighlight(isSupported);
            }
        }

        private void SpawnCollapseDebris(IReadOnlyList<PlacedPiece> collapsedPieces)
        {
            foreach (PlacedPiece piece in collapsedPieces)
            {
                GameObject debris = PieceViewFactory.Create(piece, false);
                debris.name = $"Debris {piece.Kind}";
                debris.layer = LayerMask.NameToLayer("Ignore Raycast");
                var body = debris.AddComponent<Rigidbody>();
                body.mass = 0.6f;
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                Vector3 direction = new Vector3(
                    Mathf.Sin(piece.Anchor.x * 1.7f),
                    1.25f,
                    Mathf.Cos(piece.Anchor.z * 1.3f));
                body.AddForce(direction * 1.4f, ForceMode.Impulse);
                body.AddTorque(direction * 2f, ForceMode.Impulse);
                debris.AddComponent<TemporaryDebris>().Initialize();
            }
        }

        private void PickPointedPiece()
        {
            if (!TryRaycast(out RaycastHit hit)) return;
            PlacedPieceView view = hit.collider.GetComponentInParent<PlacedPieceView>();
            if (view == null) return;
            foreach (PlacedPiece piece in _world.Pieces)
            {
                if (piece.Id != view.PieceId) continue;
                _selectedKind = piece.Kind;
                _quarterTurns = piece.QuarterTurns;
                _assembly = null;
                RebuildGhost();
                _status = $"Picked {piece.Kind}";
                return;
            }
        }

        private void CapturePointedAssembly()
        {
            if (!TryRaycast(out RaycastHit hit)) return;
            PlacedPieceView view = hit.collider.GetComponentInParent<PlacedPieceView>();
            if (view == null) return;
            IReadOnlyList<PlacedPiece> connected = _world.GetConnectedPieces(view.PieceId);
            if (connected.Count == 0) return;

            _assembly = new ConstructionAssembly(connected, view.Anchor);
            _quarterTurns = 0;
            RebuildGhost();
            _status = $"Captured connected assembly ({_assembly.PieceCount} pieces)";
        }

        private void RebuildViews()
        {
            foreach (GameObject view in _views.Values) Destroy(view);
            _views.Clear();
            foreach (PlacedPiece piece in _world.Pieces)
            {
                _views.Add(piece.Id, PieceViewFactory.Create(piece, false));
            }
            ApplyStructuralSupportOverlay();
        }

        private void SaveConstruction()
        {
            string path = Path.Combine(Application.persistentDataPath, "construction.json");
            File.WriteAllText(path, ConstructionSaveSerializer.Serialize(_world));
            _status = $"Saved {_world.PieceCount} pieces";
        }

        private void LoadConstruction()
        {
            string path = Path.Combine(Application.persistentDataPath, "construction.json");
            if (!File.Exists(path))
            {
                _status = "No construction save found";
                return;
            }

            if (!ConstructionSaveSerializer.TryRestore(File.ReadAllText(path), _world, out string error))
            {
                _status = $"Load failed: {error}";
                return;
            }

            _history.Clear();
            RebuildViews();
            _status = $"Loaded {_world.PieceCount} pieces";
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(16, 16, 540, 228), "Stormframe: Stranded Robot Prototype");
            GUI.Label(new Rect(28, 42, 310, 22), "WASD move | Middle-drag orbit | Wheel zoom");
            GUI.Label(new Rect(28, 62, 310, 22), "1 Cube | 2 Beam | 3 Plate | 4 Slope | R rotate");
            GUI.Label(new Rect(28, 82, 470, 22), "5 Half | 6 Long | 7 Pillar | 8 Wall | 9 Cylinder | 0 Rod");
            GUI.Label(new Rect(28, 102, 510, 22), $"Left-drag preview, release place | Right delete | Pieces: {_world.PieceCount}");
            GUI.Label(new Rect(28, 122, 355, 22), "F1 refocus camera | Middle-drag orbit | Wheel zoom");
            GUI.Label(new Rect(28, 142, 375, 22), $"V support overlay: {(_showStructuralSupport ? "On" : "Off")}");
            GUI.Label(new Rect(28, 162, 460, 22), "C pick | Ctrl+Z undo | Ctrl+Y redo | Ctrl+S/L save/load");
            GUI.Label(new Rect(28, 182, 510, 22), "B capture connected assembly | R rotate | Esc cancel assembly");
            string selection = _assembly == null ? _selectedKind.ToString() : $"Assembly x{_assembly.PieceCount}";
            GUI.Label(new Rect(28, 202, 510, 22), $"Selected: {selection} | {_status}");
        }
    }
}
