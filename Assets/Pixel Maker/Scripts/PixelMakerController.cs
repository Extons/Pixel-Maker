using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace PixelMaker
{
    public class PixelMakerController : MonoBehaviour
    {
        [SerializeField, Required]
        private PixelMakerSettings _settings = null;

        [SerializeField, Required]
        private Texture2D _texture;

        #region Private members

        private GameObject _model = null;

        #endregion Private members

        private void Update()
        {
        }

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

        public void UpdateModel(GameObject model, out Animation animation)
        {
            if(_model != null)
            {
                DestroyImmediate(_model);
            }
            else
            {
                foreach(Transform item in transform)
                {
                    DestroyImmediate(item.gameObject);
                }
            }

            _model = Instantiate(model, transform) as GameObject;

            _model.transform.localRotation = Quaternion.identity;
            _model.transform.localPosition = Vector3.zero;

            if(!_model.TryGetComponent<Animation>(out animation))
            {
                animation = _model.AddComponent<Animation>();
                animation.cullingType = AnimationCullingType.AlwaysAnimate;
                animation.playAutomatically = true;
            }
        }

        public void GetClipFrames(AnimationClip clip, out float clipFrames)
        {
            float frameRate = clip.frameRate;
            float duration = clip.length;

            clipFrames = duration * frameRate;
        }

        public void SetAnimationAtFrame(Animation animation, AnimationClip clip, int targetFrame)
        {
            string name = clip.name;
            float frameRate = clip.frameRate;
            float duration = clip.length;

            float frames = duration * frameRate;

            float targetTime = (targetFrame * duration) / frames;
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

        #endregion API
    }
}
