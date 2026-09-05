using System;
using System.Collections.Generic;
using NUnit.Framework;
using Stormframe.Construction;
using UnityEngine;

namespace Stormframe.Tests
{
    public sealed class PieceViewFactoryTests
    {
        [Test]
        public void CreateSlope_ProducesClosedMesh()
        {
            var piece = new PlacedPiece(Guid.NewGuid(), PieceKind.Slope, Vector3Int.zero, 0);
            GameObject view = PieceViewFactory.Create(piece, false);
            Mesh mesh = view.GetComponentInChildren<MeshFilter>().sharedMesh;
            var edgeUseCounts = new Dictionary<(int, int), int>();

            for (int index = 0; index < mesh.triangles.Length; index += 3)
            {
                CountEdge(edgeUseCounts, mesh.triangles[index], mesh.triangles[index + 1]);
                CountEdge(edgeUseCounts, mesh.triangles[index + 1], mesh.triangles[index + 2]);
                CountEdge(edgeUseCounts, mesh.triangles[index + 2], mesh.triangles[index]);
            }

            Assert.That(mesh.triangles.Length / 3, Is.EqualTo(8));
            Assert.That(edgeUseCounts.Values, Is.All.EqualTo(2));
            UnityEngine.Object.DestroyImmediate(view);
        }

        private static void CountEdge(Dictionary<(int, int), int> counts, int first, int second)
        {
            var edge = first < second ? (first, second) : (second, first);
            counts.TryGetValue(edge, out int count);
            counts[edge] = count + 1;
        }
    }
}
