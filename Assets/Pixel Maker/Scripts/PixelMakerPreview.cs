using Sirenix.OdinInspector;
using UnityEngine;

namespace PixelMaker
{
    public class PixelMakerPreview
    {
        #region Serialized members

        [HorizontalGroup("Split")]
        [VerticalGroup("Split/Source")]
        [TexturePreview(256, 256, FilterMode.Bilinear)]
        [SerializeField]
        [ReadOnly, HideLabel]
        private Texture2D _source = null;

        [VerticalGroup("Split/Source")]
        [SerializeField, ReadOnly]
        [DisplayAsString, HideLabel]
        private string _srcDisplay = "120x120";

        [VerticalGroup("Split/Destination")]
        [TexturePreview(256, 256, FilterMode.Point)]
        [SerializeField]
        [ReadOnly, HideLabel]
        private Texture2D _destination = null;

        [VerticalGroup("Split/Destination")]
        [SerializeField, ReadOnly]
        [DisplayAsString, HideLabel]
        private string _destDisplay = "120x120";

        [VerticalGroup("Split/Normal")]
        [TexturePreview(256, 256, FilterMode.Point)]
        [ShowInInspector]
        [ReadOnly, HideLabel]
        private Texture2D _normal = null;

        [VerticalGroup("Split/Normal")]
        [SerializeField, ReadOnly]
        [DisplayAsString, HideLabel]
        private string _nrmDisplay = "120x120";

        #endregion Serialized members

        #region API

        public void UpdatePreviews(PixelMakerController controller, PixelMakerSettings config, Camera camera)
        {
            UpdateRenderScale(config);
            UpdateToonMaterial(config);
            ResetCamera(camera);

            if (TryDrawPreview(config.Source, camera, FilterMode.Bilinear, out var sourceTex))
            {
                _source = sourceTex;
            }

            if (TryDrawPreview(config.Destination, camera, FilterMode.Point, out var destinationTex, null, config.PreviewConfig.Background))
            {
                _destination = destinationTex;
            }

            PreviewNormal(config.Destination, camera, config.NormalShader);

            UpdateModelTransform(controller, config);
            UpdateCamera(camera, config);
        }

        #endregion API

        #region Private methods

        private void PreviewNormal(RenderTexture renderTexture, Camera camera, Shader shader)
        {
            var background = new Color(0.5f, 0.5f, 1f, 1f);
            if (TryDrawPreview(renderTexture, camera, FilterMode.Point, out var normalTex, shader, background))
            {
                _normal = normalTex;
            }
        }

        private bool TryDrawPreview(RenderTexture renderTexture, Camera camera, FilterMode filterMode, out Texture2D texture, Shader shader = null, Color background = default)
        {
            camera.backgroundColor = background;

            if (shader == null)
            {
                camera.targetTexture = renderTexture;
                camera.Render();
            }
            else
            {
                camera.targetTexture = renderTexture;
                camera.RenderWithShader(shader, "");
            }

            texture = renderTexture.ToTexture2D(filterMode);

            if (texture != null)
            {
                return true;
            }

            return false;
        }

        private void ResetCamera(Camera camera)
        {
            camera.backgroundColor = new Color(0, 0, 0, 0);
            camera.ResetReplacementShader();
        }

        private void UpdateToonMaterial(PixelMakerSettings config)
        {
            var mat = config.ToonMaterial;

            mat.SetColor("_ColorToon", config.PreviewConfig.ToonColor);
            mat.SetColor("_AmbientColor", config.PreviewConfig.ToonAmbiantColor);
            mat.SetColor("_SpecularColor", config.PreviewConfig.ToonSpecularColor);
            mat.SetColor("_RimColor", config.PreviewConfig.ToonRimColor);
            mat.SetFloat("_Glossiness", config.PreviewConfig.ToonGlossiness);
            mat.SetFloat("_RimAmount", config.PreviewConfig.ToonRimAmount);
            mat.SetFloat("_RimThreshold", config.PreviewConfig.ToonRimThreshold);
        }

        private void UpdateRenderScale(PixelMakerSettings config)
        {
            var sourceRenderTexture = config.Source;
            var destinationRenderTexture = config.Destination;

            sourceRenderTexture.Release();
            sourceRenderTexture.height = 2 * config.PreviewConfig.SourceRenderScale;
            sourceRenderTexture.width = 2 * config.PreviewConfig.SourceRenderScale;

            destinationRenderTexture.Release();
            destinationRenderTexture.height = 2 * config.PreviewConfig.RenderScale;
            destinationRenderTexture.width = 2 * config.PreviewConfig.RenderScale;

            _srcDisplay = $"{config.Source.height}x{config.Source.width}";
            _destDisplay = $"{destinationRenderTexture.height}x{destinationRenderTexture.width}";
            _nrmDisplay = _destDisplay;
        }

        private void UpdateCamera(Camera camera, PixelMakerSettings config)
        {
            camera.orthographicSize = config.PreviewConfig.CameraScale;
            camera.transform.localPosition = new Vector3(
                config.PreviewConfig.CameraPositionX, 
                config.PreviewConfig.CameraPositionY, 
                camera.transform.localPosition.z);
        }

        private void UpdateModelTransform(PixelMakerController controller, PixelMakerSettings config)
        {
            var preview = config.PreviewConfig;
            
            controller.Position(preview.ModelPositionX, preview.ModelPositionY, preview.ModelPositionZ);
            controller.Rotate(preview.ModelRotationX, preview.ModelRotationY, preview.ModelRotationZ);
            controller.Scale(preview.ModelScale);
        }

        #endregion Private methods
    }
}
