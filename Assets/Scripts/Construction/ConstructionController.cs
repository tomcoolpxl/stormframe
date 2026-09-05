using System.Collections.Generic;
using Stormframe.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Stormframe.Construction
{
    public sealed class ConstructionController : MonoBehaviour
    {
        private readonly ConstructionWorld _world = new();
        private readonly Dictionary<System.Guid, GameObject> _views = new();
        private Camera _camera;
        private ThirdPersonCamera _thirdPersonCamera;
        private GameObject _ghost;
        private PieceKind _selectedKind;
        private Vector3Int _candidateCell;
        private int _quarterTurns;
        private bool _hasCandidate;

        private void Awake()
        {
            _camera = Camera.main;
            _thirdPersonCamera = _camera.GetComponent<ThirdPersonCamera>();
        }

        private void Update()
        {
            if (Keyboard.current == null || Mouse.current == null) return;

            ReadSelection();
            UpdateCandidate();

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                _quarterTurns = (_quarterTurns + 1) % 4;
                RebuildGhost();
            }

            if (_hasCandidate && Mouse.current.leftButton.wasPressedThisFrame)
            {
                PlaceCandidate();
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                RemovePointedPiece();
            }
        }

        private void ReadSelection()
        {
            PieceKind previous = _selectedKind;
            if (Keyboard.current.digit1Key.wasPressedThisFrame) _selectedKind = PieceKind.Cube;
            if (Keyboard.current.digit2Key.wasPressedThisFrame) _selectedKind = PieceKind.Beam;
            if (Keyboard.current.digit3Key.wasPressedThisFrame) _selectedKind = PieceKind.Plate;
            if (Keyboard.current.digit4Key.wasPressedThisFrame) _selectedKind = PieceKind.Slope;
            if (previous != _selectedKind) RebuildGhost();
        }

        private void UpdateCandidate()
        {
            _hasCandidate = TryRaycast(out RaycastHit hit);
            if (!_hasCandidate)
            {
                if (_ghost != null) _ghost.SetActive(false);
                return;
            }

            PlacedPieceView pieceView = hit.collider.GetComponentInParent<PlacedPieceView>();
            if (pieceView != null && Mathf.Abs(hit.normal.y) > 0.5f)
            {
                int direction = hit.normal.y > 0f ? 1 : -1;
                _candidateCell = ConstructionGrid.WorldToCell(hit.point);
                _candidateCell.y = pieceView.Anchor.y + direction;
            }
            else
            {
                _candidateCell = ConstructionGrid.WorldToCell(hit.point + hit.normal * 0.05f);
            }

            if (_ghost == null) RebuildGhost();
            _ghost.SetActive(true);
            _ghost.transform.position = ConstructionGrid.CellToWorld(_candidateCell)
                + PieceGeometry.VisualOffset(_selectedKind);
            _ghost.transform.rotation = Quaternion.Euler(0f, _quarterTurns * 90f, 0f);
            _thirdPersonCamera?.SetBuildingFocus(_ghost.transform.position);

            bool valid = _world.CanPlace(_selectedKind, _candidateCell, _quarterTurns);
            _ghost.GetComponent<Renderer>().sharedMaterial.color = valid
                ? new Color(0.25f, 0.9f, 0.45f, 0.45f)
                : new Color(0.95f, 0.2f, 0.2f, 0.45f);
        }

        private void PlaceCandidate()
        {
            if (!_world.TryPlace(_selectedKind, _candidateCell, _quarterTurns, out PlacedPiece piece))
            {
                return;
            }

            _views.Add(piece.Id, PieceViewFactory.Create(piece, false));
        }

        private void RemovePointedPiece()
        {
            if (!TryRaycast(out RaycastHit hit)) return;
            PlacedPieceView view = hit.collider.GetComponentInParent<PlacedPieceView>();
            if (view == null || !_world.TryRemoveAt(view.Anchor, out PlacedPiece removed)) return;
            if (_views.Remove(removed.Id, out GameObject gameObject)) Destroy(gameObject);
        }

        private bool TryRaycast(out RaycastHit hit)
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            return Physics.Raycast(ray, out hit, 250f, ~LayerMask.GetMask("Ignore Raycast"));
        }

        private void RebuildGhost()
        {
            if (_ghost != null) Destroy(_ghost);
            var preview = new PlacedPiece(System.Guid.Empty, _selectedKind, _candidateCell, _quarterTurns);
            _ghost = PieceViewFactory.Create(preview, true);
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(16, 16, 380, 108), "Stormframe Construction Prototype");
            GUI.Label(new Rect(28, 42, 310, 22), "WASD move | Middle-drag orbit | Wheel zoom");
            GUI.Label(new Rect(28, 62, 310, 22), "1 Cube | 2 Beam | 3 Plate | 4 Slope | R rotate");
            GUI.Label(new Rect(28, 82, 310, 22), $"Left place | Right delete | Pieces: {_world.PieceCount}");
            GUI.Label(new Rect(28, 102, 355, 22), "F1 close | F2 medium | F3 high | F4 build | F5 iso");
        }
    }
}
