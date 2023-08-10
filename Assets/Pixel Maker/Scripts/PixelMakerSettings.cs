#if UNITY_EDITOR

using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#endif //UNITY_EDITOR


namespace PixelMaker
{
    [CreateAssetMenu(menuName = "Tools/PixelMaker/" + nameof(PixelMakerSettings))]
    public class PixelMakerSettings : ScriptableObject
    {
        #region Serialized members

        [SerializeField, Required]
        private bool _realTimeUpdate = false;

        [SerializeField, Required]
        private SceneAsset _studio = null;

        [SerializeField, Required]
        private RenderTexture _source = null;

        [SerializeField, Required]
        private RenderTexture _destination = null;

        [SerializeField, Required]
        private RenderTexture _normal = null;

        [SerializeField, Required]
        private RenderTexture _uv = null;

        [SerializeField, Required]
        private RenderTextureFormat _renderTextureFormat = default;

        [SerializeField, Required]
        private Shader _normalShader = default;

        [SerializeField, Required]
        private Material _bitColorMaterial = default;

        [ValueDropdown(nameof(GetAllPreviewConfig), NumberOfItemsBeforeEnablingSearch = 0)]
        [SerializeField, Required]
        private PixelMakerPreviewConfig _previewConfig = null;

        #endregion Serialized members

        #region Public members

        public bool RealTimeUpdate => _realTimeUpdate;

        public SceneAsset Studio => _studio;

        public RenderTexture Source => _source;

        public RenderTexture Destination => _destination;

        public RenderTexture Normal => _normal;

        public RenderTexture UV => _uv;

        public RenderTextureFormat RenderTextureFormat => _renderTextureFormat;

        public Shader NormalShader => _normalShader;

        public PixelMakerPreviewConfig PreviewConfig => _previewConfig;

        public Material BitColorMaterial => _bitColorMaterial;

        #endregion Public members

        #region Private methods

        private IEnumerable<PixelMakerPreviewConfig> GetAllPreviewConfig()
        {
            var previewConfigs = new List<PixelMakerPreviewConfig>();

            var guids = AssetDatabase.FindAssets("t:PixelMakerPreviewConfig");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var previewConfig = AssetDatabase.LoadAssetAtPath(path, typeof(PixelMakerPreviewConfig)) as PixelMakerPreviewConfig;

                if (previewConfig != null)
                {
                    previewConfigs.Add(previewConfig);
                }
            }

            return previewConfigs;
        }

        #endregion Private methods
    }
}
