using Sirenix.OdinInspector;
using UnityEngine;

namespace PixelMaker
{
    [ExecuteInEditMode]
    public class CameraBlit : MonoBehaviour
    {
        #region Private members

        [SerializeField, Required]
        private Material _normalSpaceMaterial = null;

        [SerializeField, Required]
        private Camera _normalCamera = null;

        #endregion Private members

        #region Lifecycle

        private void OnEnable()
        {
            _normalCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
        }

        private void OnDisable()
        {
            _normalCamera.depthTextureMode = default;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_normalSpaceMaterial != null)
            {
                Graphics.Blit(source, destination, _normalSpaceMaterial);
            }
        }

        #endregion Lifecycle
    }
}