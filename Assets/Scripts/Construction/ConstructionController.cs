using System.Collections.Generic;
using System.IO;
using Stormframe.Construction.Commands;
using Stormframe.Construction.Persistence;
using Stormframe.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Stormframe.Construction
{
    public sealed class ConstructionController : MonoBehaviour
    {
        private readonly ConstructionWorld _world = new();
        private readonly ConstructionCommandHistory _history = new();
        private readonly Dictionary<System.Guid, GameObject> _views = new();
        private readonly HashSet<Vector3Int> _placementStrokeCells = new();
        private readonly List<GameObject> _assemblyGhosts = new();
        private Camera _camera;
        private ThirdPersonCamera _thirdPersonCamera;
        private GameObject _ghost;
        private ConstructionAssembly _assembly;
        private PieceKind _selectedKind;
        private ConstructionVisualStyle _visualStyle = ConstructionVisualStyle.Modular;
        private Vector3Int _candidateCell;
        private int _quarterTurns;
        private bool _hasCandidate;
        private string _status = "Ready";

        private void Awake()
        {
            _camera = Camera.main;
            _thirdPersonCamera = _camera.GetComponent<ThirdPersonCamera>();
        }

        private void Update()
        {
            if (Keyboard.current == null || Mouse.current == null) return;

            ReadSelection();
            ReadEditingCommands();
            UpdateCandidate();

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                _quarterTurns = (_quarterTurns + 1) % 4;
                RebuildGhost();
            }

            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                CycleVisualStyle();
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame && _assembly != null)
            {
                _assembly = null;
                RebuildGhost();
                _status = "Assembly mode cancelled";
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _placementStrokeCells.Clear();
            }

            bool extendingPlacementStroke = Mouse.current.leftButton.wasPressedThisFrame
                || Mouse.current.delta.ReadValue().sqrMagnitude >= 1f;
            if (_hasCandidate
                && Mouse.current.leftButton.isPressed
                && extendingPlacementStroke
                && _placementStrokeCells.Add(_candidateCell))
            {
                PlaceCandidate();
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
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
                _assembly = null;
                RebuildGhost();
            }
        }

        private void UpdateCandidate()
        {
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
                UpdateAssemblyGhosts();
                _thirdPersonCamera?.SetBuildingFocus(ConstructionGrid.CellToWorld(_candidateCell));
                return;
            }

            if (_ghost == null) RebuildGhost();
            _ghost.SetActive(true);
            _ghost.transform.position = ConstructionGrid.CellToWorld(_candidateCell)
                + PieceGeometry.VisualOffset(_selectedKind, _quarterTurns);
            _ghost.transform.rotation = Quaternion.Euler(0f, _quarterTurns * 90f, 0f);
            _thirdPersonCamera?.SetBuildingFocus(_ghost.transform.position);

            bool valid = _world.CanPlace(_selectedKind, _candidateCell, _quarterTurns);
            _ghost.GetComponent<PieceVisual>().SetPlacementValidity(valid);
        }

        private void PlaceCandidate()
        {
            IConstructionCommand command = _assembly == null
                ? new PlacePieceCommand(_selectedKind, _candidateCell, _quarterTurns)
                : new PlaceAssemblyCommand(_assembly, _candidateCell, _quarterTurns);
            if (!_history.Execute(command, _world))
            {
                return;
            }

            RebuildViews();
            _status = _assembly == null
                ? $"Placed {_selectedKind}"
                : $"Stamped assembly ({_assembly.PieceCount} pieces)";
        }

        private void RemovePointedPiece()
        {
            if (!TryRaycast(out RaycastHit hit)) return;
            PlacedPieceView view = hit.collider.GetComponentInParent<PlacedPieceView>();
            if (view == null || !_history.Execute(new RemovePieceCommand(view.Anchor), _world)) return;
            RebuildViews();
            _status = "Removed piece";
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

            if (_assembly != null)
            {
                UpdateAssemblyGhosts();
                return;
            }

            var preview = new PlacedPiece(System.Guid.Empty, _selectedKind, _candidateCell, _quarterTurns);
            _ghost = PieceViewFactory.Create(preview, true, _visualStyle);
        }

        private void UpdateAssemblyGhosts()
        {
            IReadOnlyList<AssemblyPlacement> placements = _assembly.GetPlacements(_candidateCell, _quarterTurns);
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
                    _assemblyGhosts.Add(PieceViewFactory.Create(preview, true, _visualStyle));
                }
            }

            bool valid = _assembly.CanPlace(_world, _candidateCell, _quarterTurns);
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

        private void CycleVisualStyle()
        {
            _visualStyle = (ConstructionVisualStyle)(((int)_visualStyle + 1) % 3);
            foreach (GameObject view in _views.Values)
            {
                view.GetComponent<PieceVisual>().ApplyStyle(_visualStyle);
            }

            if (_ghost != null) _ghost.GetComponent<PieceVisual>().ApplyStyle(_visualStyle);
            foreach (GameObject assemblyGhost in _assemblyGhosts)
            {
                assemblyGhost.GetComponent<PieceVisual>().ApplyStyle(_visualStyle);
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
            _placementStrokeCells.Clear();
            RebuildGhost();
            _status = $"Captured connected assembly ({_assembly.PieceCount} pieces)";
        }

        private void RebuildViews()
        {
            foreach (GameObject view in _views.Values) Destroy(view);
            _views.Clear();
            foreach (PlacedPiece piece in _world.Pieces)
            {
                _views.Add(piece.Id, PieceViewFactory.Create(piece, false, _visualStyle));
            }
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
            GUI.Label(new Rect(28, 102, 460, 22), $"Hold left + drag to place | Right delete | Pieces: {_world.PieceCount}");
            GUI.Label(new Rect(28, 122, 355, 22), "F1 close | F2 medium | F3 high | F4 build | F5 iso");
            GUI.Label(new Rect(28, 142, 375, 22), $"V visual style: {_visualStyle}");
            GUI.Label(new Rect(28, 162, 460, 22), "C pick | Ctrl+Z undo | Ctrl+Y redo | Ctrl+S/L save/load");
            GUI.Label(new Rect(28, 182, 510, 22), "B capture connected assembly | R rotate | Esc cancel assembly");
            string selection = _assembly == null ? _selectedKind.ToString() : $"Assembly x{_assembly.PieceCount}";
            GUI.Label(new Rect(28, 202, 510, 22), $"Selected: {selection} | {_status}");
        }
    }
}
