﻿using Sirenix.OdinInspector;
using System;
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

        [VerticalGroup("Split/Final")]
        [TexturePreview(256, 256, FilterMode.Point, false)]
        [ShowInInspector]
        [ReadOnly, HideLabel]
        private Texture2D _final = null;

        [VerticalGroup("Split/Final")]
        [SerializeField, ReadOnly]
        [DisplayAsString, HideLabel]
        private string _finalDisplay = "120x120";

        #endregion Serialized members

        private int _index = 0;
        #region API

        public void UpdatePreviews(PixelMakerController controller, PixelMakerSettings settings, Camera camera)
        {
            UpdateRenderScale(settings);
            ResetCamera(camera);

            if (TryDrawPreview(settings.Source, camera, FilterMode.Bilinear, out var sourceTex))
            {
                _source = sourceTex;
            }

            if (TryDrawPreview(settings.Destination, camera, FilterMode.Point, out var destinationTex, null, settings.PreviewConfig.Background))
            {
                _destination = destinationTex;
            }

            var normalBackground = new Color(0.5f, 0.5f, 1f, 1f);
            if (TryDrawPreview(settings.Normal, camera, FilterMode.Point, out var normalTex, settings.NormalShader, normalBackground))
            {
                _normal = normalTex;
            }

            UpdateMaterial(settings);

            UpdateModelTransform(controller, settings);
            UpdateCamera(camera, settings);
        }

        internal void UpdateTextureMaterial(PixelMakerSettings settings)
        {
            var renderTemp = RenderTexture.GetTemporary(settings.Destination.width, settings.Destination.height, 24);
            renderTemp.filterMode = FilterMode.Point;

            Graphics.Blit(settings.Destination, renderTemp, settings.BitColorMaterial);
            _final = renderTemp.ToTexture2D(FilterMode.Point);
            RenderTexture.ReleaseTemporary(renderTemp);
        }

        #endregion API

        #region Private methods

        private bool TryDrawPreview(RenderTexture renderTexture, Camera camera, FilterMode filterMode, out Texture2D texture, Shader shader = null, Color background = default)
        {
            camera.backgroundColor = background;

            if (shader == null)
            {
                camera.targetTexture = renderTexture;
                camera.ResetReplacementShader();
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

        private void UpdateRenderScale(PixelMakerSettings settings)
        {
            var sourceRenderTexture = settings.Source;
            var destinationRenderTexture = settings.Destination;
            var normalRenderTexture = settings.Normal;

            sourceRenderTexture.Release();
            sourceRenderTexture.height = 2 * settings.PreviewConfig.SourceRenderScale;
            sourceRenderTexture.width = 2 * settings.PreviewConfig.SourceRenderScale;

            destinationRenderTexture.Release();
            destinationRenderTexture.height = 2 * settings.PreviewConfig.RenderScale;
            destinationRenderTexture.width = 2 * settings.PreviewConfig.RenderScale;

            normalRenderTexture.Release();
            normalRenderTexture.height = 2 * settings.PreviewConfig.RenderScale;
            normalRenderTexture.width = 2 * settings.PreviewConfig.RenderScale;

            _srcDisplay = $"Source : {settings.Source.height}x{settings.Source.width}";
            _destDisplay = $"Low Resolution : {destinationRenderTexture.height}x{destinationRenderTexture.width}";
            _nrmDisplay = $"Normal : {normalRenderTexture.height}x{normalRenderTexture.width}";
            _finalDisplay = $"Final Result : {destinationRenderTexture.height}x{destinationRenderTexture.width}";
        }

        private void UpdateCamera(Camera camera, PixelMakerSettings settings)
        {
            camera.orthographicSize = settings.PreviewConfig.CameraScale;
            camera.transform.localPosition = new Vector3(
                settings.PreviewConfig.CameraPositionX,
                settings.PreviewConfig.CameraPositionY,
                camera.transform.localPosition.z);
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
