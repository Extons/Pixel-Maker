#if UNITY_EDITOR

using UnityEngine;

#endif //UNITY_EDITOR

namespace PixelMaker
{
    public static class RenderTextureExtentions
    {
        public static Texture2D ToTexture2D(this RenderTexture renderTexture, FilterMode filterMode, TextureFormat textureFormat)
        {
            Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, textureFormat, false);

            var old_rt = RenderTexture.active;
            RenderTexture.active = renderTexture;

            tex.filterMode = filterMode;
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();

            RenderTexture.active = old_rt;
            return tex;
        }
    }

}
