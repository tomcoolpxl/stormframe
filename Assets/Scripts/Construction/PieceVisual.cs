using UnityEngine;

namespace Stormframe.Construction
{
    public sealed class PieceVisual : MonoBehaviour
    {
        private PieceKind _kind;
        private bool _ghost;
        private Transform _geometry;
        private Renderer _renderer;
        private Vector3 _baseScale;
        private Material _material;
        private ConstructionVisualStyle _style;

        public ConstructionVisualStyle Style => _style;
        public void Initialize(
            PieceKind kind,
            bool ghost,
            Transform geometry,
            Renderer renderer,
            Vector3 baseScale)
        {
            _kind = kind;
            _ghost = ghost;
            _geometry = geometry;
            _renderer = renderer;
            _baseScale = baseScale;
            _material = CreateMaterial();
            _renderer.sharedMaterial = _material;
        }

        public void ApplyStyle(ConstructionVisualStyle style)
        {
            _style = style;
            float seamScale = style switch
            {
                ConstructionVisualStyle.Natural => 0.985f,
                ConstructionVisualStyle.Blockout => 0.86f,
                _ => 0.94f
            };
            _geometry.localScale = _baseScale * seamScale;

            if (!_ghost)
            {
                _material.color = ColorFor(_kind, style);
                _material.SetFloat("_Glossiness", style == ConstructionVisualStyle.Natural ? 0.12f : 0.28f);
            }
        }

        public void SetPlacementValidity(bool valid)
        {
            if (!_ghost) return;
            _material.color = valid
                ? new Color(0.2f, 0.95f, 0.42f, 0.48f)
                : new Color(1f, 0.16f, 0.14f, 0.48f);
        }

        private Material CreateMaterial()
        {
            var material = new Material(Shader.Find("Standard"));
            if (_ghost)
            {
                material.SetFloat("_Mode", 3f);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.renderQueue = 3000;
            }

            return material;
        }

        private static Color ColorFor(PieceKind kind, ConstructionVisualStyle style)
        {
            if (style == ConstructionVisualStyle.Blockout)
            {
                return kind switch
                {
                    PieceKind.Beam => new Color(0.95f, 0.33f, 0.16f),
                    PieceKind.LongBlock => new Color(0.95f, 0.33f, 0.16f),
                    PieceKind.Pillar => new Color(0.7f, 0.25f, 0.85f),
                    PieceKind.WallPanel => new Color(0.98f, 0.62f, 0.14f),
                    PieceKind.Cylinder => new Color(0.12f, 0.78f, 0.82f),
                    PieceKind.Rod => new Color(0.88f, 0.88f, 0.92f),
                    PieceKind.Plate => new Color(0.98f, 0.82f, 0.18f),
                    PieceKind.Slope => new Color(0.22f, 0.64f, 0.95f),
                    _ => new Color(0.86f, 0.24f, 0.18f)
                };
            }

            if (style == ConstructionVisualStyle.Natural)
            {
                return kind switch
                {
                    PieceKind.Beam => new Color(0.34f, 0.17f, 0.07f),
                    PieceKind.LongBlock => new Color(0.38f, 0.2f, 0.08f),
                    PieceKind.Pillar => new Color(0.3f, 0.28f, 0.25f),
                    PieceKind.WallPanel => new Color(0.48f, 0.43f, 0.34f),
                    PieceKind.Cylinder => new Color(0.27f, 0.35f, 0.36f),
                    PieceKind.Rod => new Color(0.38f, 0.4f, 0.42f),
                    PieceKind.Plate => new Color(0.58f, 0.51f, 0.36f),
                    PieceKind.Slope => new Color(0.32f, 0.39f, 0.43f),
                    _ => new Color(0.54f, 0.27f, 0.12f)
                };
            }

            return kind switch
            {
                PieceKind.Beam => new Color(0.48f, 0.24f, 0.09f),
                PieceKind.LongBlock => new Color(0.52f, 0.27f, 0.1f),
                PieceKind.Pillar => new Color(0.38f, 0.34f, 0.3f),
                PieceKind.WallPanel => new Color(0.65f, 0.56f, 0.42f),
                PieceKind.Cylinder => new Color(0.3f, 0.48f, 0.5f),
                PieceKind.Rod => new Color(0.55f, 0.57f, 0.6f),
                PieceKind.Plate => new Color(0.74f, 0.65f, 0.43f),
                PieceKind.Slope => new Color(0.34f, 0.5f, 0.62f),
                _ => new Color(0.7f, 0.34f, 0.13f)
            };
        }
    }
}
