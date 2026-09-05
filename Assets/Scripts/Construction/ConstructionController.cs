using System.Collections.Generic;
using UnityEngine;

namespace Stormframe.Construction
{
    public sealed class ConstructionController : MonoBehaviour
    {
        private readonly ConstructionWorld _world = new();
        private readonly Dictionary<System.Guid, GameObject> _views = new();
        private Camera _camera;
        private GameObject _ghost;
        private PieceKind _selectedKind;
        private Vector3Int _candidateCell;
        private int _quarterTurns;
        private bool _hasCandidate;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            ReadSelection();
            UpdateCandidate();

            if (Input.GetKeyDown(KeyCode.R))
            {
                _quarterTurns = (_quarterTurns + 1) % 4;
                RebuildGhost();
            }

            if (_hasCandidate && Input.GetMouseButtonDown(0))
            {
                PlaceCandidate();
            }

            if (Input.GetMouseButtonDown(1))
            {
                RemovePointedPiece();
            }
        }

        private void ReadSelection()
        {
            PieceKind previous = _selectedKind;
            if (Input.GetKeyDown(KeyCode.Alpha1)) _selectedKind = PieceKind.Cube;
            if (Input.GetKeyDown(KeyCode.Alpha2)) _selectedKind = PieceKind.Beam;
            if (Input.GetKeyDown(KeyCode.Alpha3)) _selectedKind = PieceKind.Plate;
            if (Input.GetKeyDown(KeyCode.Alpha4)) _selectedKind = PieceKind.Slope;
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
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
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
            GUI.Box(new Rect(16, 16, 330, 88), "Stormframe Construction Prototype");
            GUI.Label(new Rect(28, 42, 310, 22), "WASD move | Middle-drag orbit | Wheel zoom");
            GUI.Label(new Rect(28, 62, 310, 22), "1 Cube | 2 Beam | 3 Plate | 4 Slope | R rotate");
            GUI.Label(new Rect(28, 82, 310, 22), $"Left place | Right delete | Pieces: {_world.PieceCount}");
        }
    }
}
