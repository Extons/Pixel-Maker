#if UNITY_EDITOR

using Sirenix.OdinInspector;
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
        private RenderTextureFormat _renderTextureFormat = default;

        [SerializeField, Required]
        private Shader _normalShader = default;

        [SerializeField, Required]
        private Material _bitColorMaterial = default;

        [SerializeField, Required]
        private PixelMakerPreviewConfig _previewConfig = null;

        #endregion Serialized members

        #region Public members

        public bool RealTimeUpdate => _realTimeUpdate;

        public SceneAsset Studio => _studio;

        public RenderTexture Source => _source;

        public RenderTexture Destination => _destination;

        public RenderTexture Normal => _normal;

        public RenderTextureFormat RenderTextureFormat => _renderTextureFormat;

        public Shader NormalShader => _normalShader;

        public PixelMakerPreviewConfig PreviewConfig => _previewConfig;

        public Material BitColorMaterial => _bitColorMaterial;

        #endregion Public members
    }
}
