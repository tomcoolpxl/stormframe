using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stormframe.Construction.Persistence
{
    public static class ConstructionSaveSerializer
    {
        private const int CurrentVersion = 2;

        public static string Serialize(ConstructionWorld world)
        {
            var data = new ConstructionSaveData { version = CurrentVersion };
            foreach (PlacedPiece piece in world.Pieces.OrderBy(piece => piece.Id))
            {
                data.pieces.Add(new PieceSaveData
                {
                    id = piece.Id.ToString("N"),
                    kind = (int)piece.Kind,
                    x = piece.Anchor.x,
                    y = piece.Anchor.y,
                    z = piece.Anchor.z,
                    quarterTurns = piece.QuarterTurns
                });
            }

            return JsonUtility.ToJson(data, true);
        }

        public static bool TryRestore(string json, ConstructionWorld world, out string error)
        {
            error = null;
            ConstructionSaveData data;
            try
            {
                data = JsonUtility.FromJson<ConstructionSaveData>(json);
            }
            catch (Exception exception)
            {
                error = exception.Message;
                return false;
            }

            if (data == null || data.version < 1 || data.version > CurrentVersion || data.pieces == null)
            {
                error = "Unsupported or incomplete construction save.";
                return false;
            }

            var restored = new ConstructionWorld();
            foreach (PieceSaveData savedPiece in data.pieces)
            {
                int migratedY = data.version == 1 ? savedPiece.y * 2 : savedPiece.y;
                if (!Guid.TryParseExact(savedPiece.id, "N", out Guid id)
                    || !Enum.IsDefined(typeof(PieceKind), savedPiece.kind)
                    || !restored.TryPlace(
                        id,
                        (PieceKind)savedPiece.kind,
                        new Vector3Int(savedPiece.x, migratedY, savedPiece.z),
                        savedPiece.quarterTurns,
                        out _))
                {
                    error = "Save contains an invalid or overlapping piece.";
                    return false;
                }
            }

            world.Clear();
            foreach (PlacedPiece piece in restored.Pieces)
            {
                world.TryPlace(piece.Id, piece.Kind, piece.Anchor, piece.QuarterTurns, out _);
            }

            return true;
        }

        [Serializable]
        private sealed class ConstructionSaveData
        {
            public int version;
            public List<PieceSaveData> pieces = new();
        }

        [Serializable]
        private sealed class PieceSaveData
        {
            public string id;
            public int kind;
            public int x;
            public int y;
            public int z;
            public int quarterTurns;
        }
    }
}
