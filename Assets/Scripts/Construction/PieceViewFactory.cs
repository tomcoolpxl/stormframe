using UnityEngine;

namespace Stormframe.Construction
{
    public static class PieceViewFactory
    {
        public static GameObject Create(
            PlacedPiece piece,
            bool ghost,
            ConstructionVisualStyle style = ConstructionVisualStyle.Modular)
        {
            var view = new GameObject();
            view.name = ghost ? $"Ghost {piece.Kind}" : $"{piece.Kind} {piece.Id:N}";
            view.transform.position = ConstructionGrid.CellToWorld(piece.Anchor)
                + PieceGeometry.VisualOffset(piece.Kind, piece.QuarterTurns);
            view.transform.rotation = Quaternion.Euler(0f, piece.QuarterTurns * 90f, 0f);

            Mesh slopeMesh = piece.Kind == PieceKind.Slope ? CreateSlopeMesh() : null;
            GameObject geometry = CreateGeometry(piece.Kind, slopeMesh);
            geometry.name = "Geometry";
            geometry.transform.SetParent(view.transform, false);
            Vector3 baseScale = ScaleFor(piece.Kind);

            AddCollider(view, piece.Kind, ColliderSizeFor(piece.Kind), slopeMesh);
            var visual = view.AddComponent<PieceVisual>();
            visual.Initialize(
                piece.Kind,
                ghost,
                geometry.transform,
                geometry.GetComponent<Renderer>(),
                baseScale);
            visual.ApplyStyle(style);

            if (ghost)
            {
                view.layer = LayerMask.NameToLayer("Ignore Raycast");
                foreach (Collider collider in view.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = false;
                }
            }
            else
            {
                view.AddComponent<PlacedPieceView>().Initialize(piece);
            }

            return view;
        }

        private static GameObject CreateGeometry(PieceKind kind, Mesh slopeMesh)
        {
            if (kind != PieceKind.Slope)
            {
                PrimitiveType primitive = kind == PieceKind.Cylinder || kind == PieceKind.Rod
                    ? PrimitiveType.Cylinder
                    : PrimitiveType.Cube;
                GameObject geometry = GameObject.CreatePrimitive(primitive);
                DestroyObject(geometry.GetComponent<Collider>());
                return geometry;
            }

            var slope = new GameObject();
            slope.AddComponent<MeshFilter>().sharedMesh = slopeMesh;
            slope.AddComponent<MeshRenderer>();
            return slope;
        }

        private static void AddCollider(
            GameObject view,
            PieceKind kind,
            Vector3 scale,
            Mesh slopeMesh)
        {
            if (kind == PieceKind.Slope)
            {
                view.AddComponent<MeshCollider>().sharedMesh = slopeMesh;
                return;
            }

            view.AddComponent<BoxCollider>().size = scale;
        }

        private static Vector3 ScaleFor(PieceKind kind)
        {
            return kind switch
            {
                PieceKind.Beam => new Vector3(3f, 1f, 1f),
                PieceKind.Plate => new Vector3(2f, 0.25f, 2f),
                PieceKind.HalfBlock => new Vector3(1f, 0.5f, 1f),
                PieceKind.LongBlock => new Vector3(2f, 1f, 1f),
                PieceKind.Pillar => new Vector3(1f, 2f, 1f),
                PieceKind.WallPanel => new Vector3(2f, 2f, 0.25f),
                PieceKind.Cylinder => new Vector3(0.45f, 0.5f, 0.45f),
                PieceKind.Rod => new Vector3(0.14f, 1f, 0.14f),
                _ => Vector3.one
            };
        }

        private static Vector3 ColliderSizeFor(PieceKind kind)
        {
            return kind switch
            {
                PieceKind.Cylinder => new Vector3(0.9f, 1f, 0.9f),
                PieceKind.Rod => new Vector3(0.28f, 2f, 0.28f),
                _ => ScaleFor(kind)
            };
        }

        private static void DestroyObject(Object target)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(target);
            }
            else
            {
                Object.DestroyImmediate(target);
            }
        }

        private static Mesh CreateSlopeMesh()
        {
            var mesh = new Mesh { name = "Slope Mesh" };
            mesh.vertices = new[]
            {
                new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f)
            };
            mesh.triangles = new[]
            {
                0, 1, 2, 1, 3, 2,
                2, 3, 4, 3, 5, 4,
                0, 2, 4,
                1, 5, 3,
                0, 4, 1, 1, 4, 5
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
