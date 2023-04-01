#if UNITY_EDITOR

using UnityEngine;

#endif //UNITY_EDITOR


namespace PixelMaker
{
    public class TexturePreviewAttribute : PropertyAttribute
    {
        public int width;
        public int height;
        public FilterMode filterMode;
        public bool background;

        public TexturePreviewAttribute(int width = 64, int height = 64, FilterMode filterMode = FilterMode.Bilinear, bool background = true)
        {
            this.width = width;
            this.height = height;
            this.filterMode = filterMode;
            this.background = background;
        }
    }
}
