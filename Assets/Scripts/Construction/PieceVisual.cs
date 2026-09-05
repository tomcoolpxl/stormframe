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
                    PieceKind.Plate => new Color(0.58f, 0.51f, 0.36f),
                    PieceKind.Slope => new Color(0.32f, 0.39f, 0.43f),
                    _ => new Color(0.54f, 0.27f, 0.12f)
                };
            }

            return kind switch
            {
                PieceKind.Beam => new Color(0.48f, 0.24f, 0.09f),
                PieceKind.Plate => new Color(0.74f, 0.65f, 0.43f),
                PieceKind.Slope => new Color(0.34f, 0.5f, 0.62f),
                _ => new Color(0.7f, 0.34f, 0.13f)
            };
        }
    }
}
