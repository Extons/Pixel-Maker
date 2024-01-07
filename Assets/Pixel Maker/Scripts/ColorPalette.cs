using Sirenix.OdinInspector;
using UnityEngine;

namespace PixelMaker
{
    [CreateAssetMenu(menuName = "Tools/PixelMaker/ColorPalette/" + nameof(ColorPalette))]
    public class ColorPalette : ScriptableObject
    {
        [SerializeField]
        private Color[] _colors = default;

        private Texture2D _texture = null;

        public Texture2D toTexture()
        {
            if(_texture == null
                || _texture.width != _colors.Length)
            {
                _texture = new Texture2D(_colors.Length, 1, TextureFormat.RGBA32, false, true);
                _texture.filterMode = FilterMode.Point;
            }

            for (int i = 0; i < _colors.Length; i++)
            {
                _texture.SetPixel(i, 0, _colors[i]);
            }

            _texture.Apply();
            return _texture;
        }

        [Button("Clear Texture")]
        private void Clear()
        {
            DestroyImmediate(_texture);
        }
    }
}
