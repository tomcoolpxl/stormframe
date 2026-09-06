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
        private Color _modularColor;

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
            _geometry.localScale = _baseScale * 0.94f;

            if (!_ghost)
            {
                _modularColor = ModularColorFor(_kind);
                _material.color = _modularColor;
                _material.SetFloat("_Glossiness", 0.28f);
            }
        }

        public void SetStructuralHighlight(bool? supported)
        {
            if (_ghost) return;
            _material.color = supported switch
            {
                true => new Color(0.12f, 0.78f, 0.52f),
                false => new Color(1f, 0.16f, 0.12f),
                null => _modularColor
            };
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

        private static Color ModularColorFor(PieceKind kind)
        {
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
