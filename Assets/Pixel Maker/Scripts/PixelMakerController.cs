using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace PixelMaker
{
    public class PixelMakerController : MonoBehaviour
    {
        #region Serialized members

        [SerializeField, Required]
        private Material _uvMaterial = null;

        [SerializeField, Required]
        private LayerMask _layerMask = default;

        [SerializeField, Required]
        private ShadowCastingMode _shadowCastingMode = default;

        [SerializeField, Required]
        private bool _receiveShadows = default;

        #endregion Serialized members

        #region Private members

        private Animation _uvAnimation = null;

        private GameObject _parent = null;

        #endregion Private members

        #region API

        public void Rotate(float x, float y, float z)
        {
            transform.localRotation = Quaternion.Euler(x, y, z);
        }

        public void Position(float x, float y, float z)
        {
            transform.localPosition = new Vector3(x, y, z);
        }

        public void Scale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public void UpdateModel(GameObject instance, out Animation animation)
        {
            DestroyParent();

            InstantiateModel(instance, out animation, out _);
            InstantiateModel(instance, out _uvAnimation, out var uvModel);
            ReplaceModelMaterials(uvModel, _uvMaterial, _shadowCastingMode, _receiveShadows, 3);
        }

        public void GetClipFrames(AnimationClip clip, out float clipFrames)
        {
            float frameRate = clip.frameRate;
            float duration = clip.length;

            clipFrames = duration * frameRate;
        }

        public void SetAnimationAtFrame(Animation animation, AnimationClip clip, int targetFrame)
        {
            SetAnimationAtFrameModel(animation, clip, targetFrame);
            SetAnimationAtFrameModel(_uvAnimation, clip, targetFrame);
        }

        #endregion API

        #region Private methods

        private void InstantiateModel(GameObject instance, out Animation animation, out GameObject gameObject)
        {
            if (_parent == null)
            {
                _parent = new GameObject("Parent");
                _parent.transform.SetParent(transform);
            }

            var model = Instantiate(instance, transform) as GameObject;

            model.transform.localRotation = Quaternion.identity;
            model.transform.localPosition = Vector3.zero;

            model.transform.SetParent(_parent.transform);

            if (!model.TryGetComponent<Animation>(out animation))
            {
                animation = model.AddComponent<Animation>();
                animation.cullingType = AnimationCullingType.AlwaysAnimate;
                animation.playAutomatically = true;
            }

            gameObject = model;
        }

        private void DestroyParent()
        {
            if (_parent != null)
            {
                DestroyImmediate(_parent);
            }
        }

        private void SetAnimationAtFrameModel(Animation animation, AnimationClip clip, int targetFrame)
        {
            string name = clip.name;
            float frameRate = clip.frameRate;
            float duration = clip.length;

            float frames = duration * frameRate;

            float targetTime = (targetFrame * duration) / frames;

            if (animation[name] == null)
            {
                animation.AddClip(clip, name);
            }

            var state = animation[name];

            if (state != null)
            {
                state.enabled = true;
                state.weight = 1;
                state.normalizedTime = 0;
                state.time = targetTime;

                animation.Sample();

                state.enabled = false;
            }
        }

        private void ReplaceModelMaterials(GameObject model, Material material, ShadowCastingMode castingShadowMode, bool receiveShadows, int layer = 0)
        {
            var renderers = model.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                int length = renderer.sharedMaterials.Length;
                Material[] newMaterials = new Material[length];

                for (int i = 0; i < length; i++)
                {
                    newMaterials[i] = material;
                }

                renderer.sharedMaterials = newMaterials;
                renderer.gameObject.layer = layer;
                renderer.shadowCastingMode = castingShadowMode;
                renderer.receiveShadows = receiveShadows;
            }
        }

        #endregion Private methods
    }
}
