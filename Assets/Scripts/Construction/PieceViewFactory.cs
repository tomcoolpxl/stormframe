using UnityEngine;

namespace Stormframe.Construction
{
    public static class PieceViewFactory
    {
        public static GameObject Create(PlacedPiece piece, bool ghost)
        {
            GameObject view = piece.Kind == PieceKind.Slope
                ? CreateSlope()
                : GameObject.CreatePrimitive(PrimitiveType.Cube);

            view.name = ghost ? $"Ghost {piece.Kind}" : $"{piece.Kind} {piece.Id:N}";
            view.transform.position = ConstructionGrid.CellToWorld(piece.Anchor)
                + PieceGeometry.VisualOffset(piece.Kind);
            view.transform.rotation = Quaternion.Euler(0f, piece.QuarterTurns * 90f, 0f);
            view.transform.localScale = ScaleFor(piece.Kind);

            Renderer renderer = view.GetComponent<Renderer>();
            renderer.sharedMaterial = CreateMaterial(piece.Kind, ghost);

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

        private static Vector3 ScaleFor(PieceKind kind)
        {
            return kind switch
            {
                PieceKind.Beam => new Vector3(3f, 1f, 1f),
                PieceKind.Plate => new Vector3(2f, 0.25f, 2f),
                _ => Vector3.one
            };
        }

        private static Material CreateMaterial(PieceKind kind, bool ghost)
        {
            Shader shader = Shader.Find("Standard");
            var material = new Material(shader);
            Color color = kind switch
            {
                PieceKind.Beam => new Color(0.45f, 0.23f, 0.10f),
                PieceKind.Plate => new Color(0.72f, 0.64f, 0.45f),
                PieceKind.Slope => new Color(0.35f, 0.48f, 0.60f),
                _ => new Color(0.68f, 0.34f, 0.16f)
            };

            if (ghost)
            {
                color.a = 0.45f;
                material.SetFloat("_Mode", 3f);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.renderQueue = 3000;
            }

            material.color = color;
            return material;
        }

        private static GameObject CreateSlope()
        {
            var gameObject = new GameObject("Slope");
            var mesh = new Mesh { name = "Slope Mesh" };
            mesh.vertices = new[]
            {
                new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f)
            };
            mesh.triangles = new[]
            {
                0, 2, 1, 1, 2, 3,
                2, 4, 3, 3, 4, 5,
                0, 4, 2, 0, 1, 4,
                1, 5, 4, 1, 3, 5,
                0, 5, 1, 0, 4, 5
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
            return gameObject;
        }
    }
}
