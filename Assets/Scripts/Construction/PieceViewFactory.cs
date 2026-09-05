using System.Collections.Generic;
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
                + PieceGeometry.VisualOffset(piece.Kind);
            view.transform.rotation = Quaternion.Euler(0f, piece.QuarterTurns * 90f, 0f);

            Mesh slopeMesh = piece.Kind == PieceKind.Slope ? CreateSlopeMesh() : null;
            GameObject geometry = CreateGeometry(piece.Kind, slopeMesh);
            geometry.name = "Geometry";
            geometry.transform.SetParent(view.transform, false);
            Vector3 baseScale = ScaleFor(piece.Kind);

            AddCollider(view, piece.Kind, baseScale, slopeMesh);
            List<GameObject> connectors = CreateConnectors(view.transform, piece.Kind);
            var visual = view.AddComponent<PieceVisual>();
            visual.Initialize(
                piece.Kind,
                ghost,
                geometry.transform,
                geometry.GetComponent<Renderer>(),
                baseScale,
                connectors);
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
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DestroyObject(cube.GetComponent<Collider>());
                return cube;
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
                _ => Vector3.one
            };
        }

        private static List<GameObject> CreateConnectors(Transform parent, PieceKind kind)
        {
            var connectors = new List<GameObject>();
            Vector3[] positions = kind switch
            {
                PieceKind.Beam => new[]
                {
                    new Vector3(-1f, 0.51f, 0f), Vector3.up * 0.51f, new Vector3(1f, 0.51f, 0f)
                },
                PieceKind.Plate => new[]
                {
                    new Vector3(-0.5f, 0.14f, -0.5f), new Vector3(0.5f, 0.14f, -0.5f),
                    new Vector3(-0.5f, 0.14f, 0.5f), new Vector3(0.5f, 0.14f, 0.5f)
                },
                PieceKind.Slope => new[] { new Vector3(0f, 0.51f, 0.35f) },
                _ => new[] { Vector3.up * 0.51f }
            };

            var connectorMaterial = new Material(Shader.Find("Standard"))
            {
                color = new Color(0.82f, 0.88f, 0.9f),
                name = "Connector Indicator"
            };
            foreach (Vector3 position in positions)
            {
                GameObject connector = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                connector.name = "Connection Indicator";
                connector.transform.SetParent(parent, false);
                connector.transform.localPosition = position;
                connector.transform.localScale = new Vector3(0.13f, 0.035f, 0.13f);
                DestroyObject(connector.GetComponent<Collider>());
                connector.GetComponent<Renderer>().sharedMaterial = connectorMaterial;
                connectors.Add(connector);
            }

            return connectors;
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
