using Sirenix.OdinInspector;
using UnityEngine;

namespace PixelMaker
{
    public class PixelMakerPreview
    {
        #region Constants members

        private const int _previewSize = 128 * 2;

        #endregion Constants members

        #region Serialized members

        [HorizontalGroup("Split")]
        [VerticalGroup("Split/Source")]
        [TexturePreview(_previewSize, _previewSize, FilterMode.Bilinear)]
        [SerializeField]
        [ReadOnly, HideLabel]
        private Texture2D _source = null;

        [VerticalGroup("Split/Source")]
        [SerializeField, ReadOnly]
        [DisplayAsString, HideLabel]
        private string _srcDisplay = "120x120";

        [VerticalGroup("Split/Destination")]
        [TexturePreview(_previewSize, _previewSize, FilterMode.Point)]
        [SerializeField]
        [ReadOnly, HideLabel]
        private Texture2D _destination = null;

        [VerticalGroup("Split/Destination")]
        [SerializeField, ReadOnly]
        [DisplayAsString, HideLabel]
        private string _destDisplay = "120x120";

        [VerticalGroup("Split/Normal")]
        [TexturePreview(_previewSize, _previewSize, FilterMode.Point)]
        [ShowInInspector]
        [ReadOnly, HideLabel]
        private Texture2D _normal = null;

        [VerticalGroup("Split/Normal")]
        [SerializeField, ReadOnly]
        [DisplayAsString, HideLabel]
        private string _nrmDisplay = "120x120";

        [VerticalGroup("Split/UV")]
        [TexturePreview(_previewSize, _previewSize, FilterMode.Point)]
        [ShowInInspector]
        [ReadOnly, HideLabel]
        private Texture2D _uv = null;

        [VerticalGroup("Split/UV")]
        [SerializeField, ReadOnly]
        [DisplayAsString, HideLabel]
        private string _uvDisplay = "120x120";

        [VerticalGroup("Split/Final")]
        [TexturePreview(_previewSize, _previewSize, FilterMode.Point, false)]
        [ShowInInspector]
        [ReadOnly, HideLabel]
        private Texture2D _final = null;

        [VerticalGroup("Split/Final")]
        [SerializeField, ReadOnly]
        [DisplayAsString, HideLabel]
        private string _finalDisplay = "120x120";

        [HorizontalGroup("Preview")]
        [VerticalGroup("Preview/Large")]
        [TexturePreview(512, 512, FilterMode.Point, false)]
        [ShowInInspector]
        [ReadOnly, HideLabel]
        [ShowIf(nameof(_inAnimPreview))]
        private Texture2D _previewAnimLarge = null;

        [HorizontalGroup("Preview")]
        [VerticalGroup("Preview/Medium")]
        [TexturePreview(256, 256, FilterMode.Point, false)]
        [ShowInInspector]
        [ReadOnly, HideLabel]
        [ShowIf(nameof(_inAnimPreview))]
        private Texture2D _previewAnimMedium = null;

        [HorizontalGroup("Preview")]
        [VerticalGroup("Preview/Small")]
        [TexturePreview(128, 128, FilterMode.Point, false)]
        [ShowInInspector]
        [ReadOnly, HideLabel]
        [ShowIf(nameof(_inAnimPreview))]
        private Texture2D _previewAnimSmall = null;

        #endregion Serialized members

        #region Private members

        private bool _inAnimPreview = default;

        #endregion Private members

        #region API

        public void UpdatePreviews(PixelMakerController controller, PixelMakerSettings settings, Camera sourceCamera, Camera normalCamera, Camera uvCamera)
        {
            UpdateRenderScale(settings);

            if (TryDrawPreview(settings.Source, sourceCamera, FilterMode.Bilinear, TextureFormat.RGBA32, out var sourceTex))
            {
                _source = sourceTex;
            }

            if (TryDrawPreview(settings.Destination, sourceCamera, FilterMode.Point, TextureFormat.RGBA32, out var destinationTex))
            {
                _destination = destinationTex;
            }

            if (TryDrawPreview(settings.Normal, normalCamera, FilterMode.Point, TextureFormat.RGB24, out var normalTex))
            {
                _normal = normalTex;
            }

            if (TryDrawPreview(settings.UV, uvCamera, FilterMode.Point, TextureFormat.RGB24, out var uvTex))
            {
                _uv = uvTex;
            }

            UpdateMaterial(settings);

            UpdateModelTransform(controller, settings);
            UpdateCamera(sourceCamera, settings);
        }

        public void UpdateTextureMaterial(PixelMakerSettings settings)
        {
            var renderTemp = RenderTexture.GetTemporary(settings.Destination.width, settings.Destination.height, 32);
            renderTemp.filterMode = FilterMode.Point;

            Graphics.Blit(settings.Destination, renderTemp, settings.BitColorMaterial);
            _final = renderTemp.ToTexture2D(FilterMode.Point, TextureFormat.RGBA32);
            RenderTexture.ReleaseTemporary(renderTemp);
        }

        public void ClearTextures()
        {
            _normal = null;
            _final = null;
        }

        public bool TryGetRenderFrame(out Texture2D destination)
        {
            if (_destination != null)
            {
                destination = _final;
                return true;
            }

            destination = null;
            return false;
        }

        public bool TryGetNormalFrame(out Texture2D normal)
        {
            if (_normal != null)
            {
                normal = _normal;
                return true;
            }

            normal = null;
            return false;
        }

        public bool TryGetUVFrame(out Texture2D uv)
        {
            if (_uv != null)
            {
                uv = _uv;
                return true;
            }

            uv = null;
            return false;
        }

        public void PreviewTexture(Texture2D texture)
        {
            _inAnimPreview = (texture != null);

            _previewAnimLarge = texture;
            _previewAnimMedium = texture;
            _previewAnimSmall = texture;
        }

        #endregion API

        #region Private methods

        private bool TryDrawPreview(RenderTexture renderTexture, Camera camera, FilterMode filterMode, TextureFormat textureFormat, out Texture2D texture)
        {
            if (camera != null)
            {
                camera.targetTexture = renderTexture;
                camera.Render();
            }

            texture = renderTexture.ToTexture2D(filterMode, textureFormat);

            if (texture != null)
            {
                return true;
            }

            return false;
        }

        private void UpdateRenderScale(PixelMakerSettings settings)
        {            
            UpdateRenderScale(settings.Source, settings.PreviewConfig.SourceRenderScale, "Source", out _srcDisplay);
            UpdateRenderScale(settings.Destination, settings.PreviewConfig.RenderScale, "Low Resolution", out _destDisplay);
            UpdateRenderScale(settings.Normal, settings.PreviewConfig.RenderScale, "Normal", out _nrmDisplay);
            UpdateRenderScale(settings.UV, settings.PreviewConfig.RenderScale, "UV", out _uvDisplay);
            UpdateRenderScale(settings.Destination, settings.PreviewConfig.RenderScale, "Final Result", out _finalDisplay);
        }

        private void UpdateRenderScale(RenderTexture renderTexture, int scale, string prefix, out string display)
        {
            renderTexture.Release();
            renderTexture.height = 2 * scale;
            renderTexture.width = 2 * scale;
            display = $"{prefix} : {renderTexture.height}x{renderTexture.width}";
        }

        private void UpdateCamera(Camera camera, PixelMakerSettings settings)
        {
            camera.orthographicSize = settings.PreviewConfig.CameraScale;
            camera.transform.localPosition =
                new Vector3(
                settings.PreviewConfig.CameraPositionX,
                settings.PreviewConfig.CameraPositionY,
                camera.transform.localPosition.z
                );
        }

        private void UpdateModelTransform(PixelMakerController controller, PixelMakerSettings settings)
        {
            var preview = settings.PreviewConfig;

            controller.Position(preview.ModelPositionX, preview.ModelPositionY, preview.ModelPositionZ);
            controller.Rotate(preview.ModelRotationX, preview.ModelRotationY, preview.ModelRotationZ);
            controller.Scale(preview.ModelScale);
        }

        private void UpdateMaterial(PixelMakerSettings settings)
        {
            if (settings.PreviewConfig == null
                || settings.PreviewConfig.Palette == null)
            {
                return;
            }

            settings.BitColorMaterial.SetTexture("_PaletteTex", settings.PreviewConfig.Palette.toTexture());
            settings.BitColorMaterial.SetColor("_OutlineColor", settings.PreviewConfig.OutlineColor);
            settings.BitColorMaterial.SetFloat("_OutlineWidth", settings.PreviewConfig.OutlineWidth);
            settings.BitColorMaterial.SetFloat("_Saturation", settings.PreviewConfig.Saturation);
            settings.BitColorMaterial.SetFloat("_Contrast", settings.PreviewConfig.Contrast);
            settings.BitColorMaterial.SetFloat("_BitValue", settings.PreviewConfig.BitValue);
            settings.BitColorMaterial.SetFloat("_Bit", settings.PreviewConfig.ConvertToBit ? 1 : 0);
            settings.BitColorMaterial.SetFloat("_Palette", settings.PreviewConfig.UsePalette ? 1 : 0);
        }

        #endregion Private methods
    }
}
