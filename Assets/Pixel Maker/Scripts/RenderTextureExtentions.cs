using UnityEngine;

namespace PixelMaker
{
    public static class RenderTextureExtentions
    {
        public static Texture2D ToTexture2D(this RenderTexture renderTexture, FilterMode filterMode, TextureFormat textureFormat)
        {            
            Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, textureFormat, false);
            var activeRenderTexture = RenderTexture.active;
            
            RenderTexture.active = renderTexture;

            tex.filterMode = filterMode;
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();

            RenderTexture.active = null;

            return tex;
        }
    }
}