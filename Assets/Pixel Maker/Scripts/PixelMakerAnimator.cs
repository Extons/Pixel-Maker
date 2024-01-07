using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace PixelMaker
{
    public class PixelMakerAnimator : ScriptableObject
    {
        #region Serialized members

        [BoxGroup("Preview Animation")]
        [ProgressBar(0, nameof(_maxFrame), 0, 1, 0, Segmented = true)]
        [ShowInInspector]
        private int _animationFrame = 0;

        [BoxGroup("Preview Animation")]
        [SerializeField]
        private bool _autoUpdate = true;

        #endregion Serialized members

        #region Private members

        private int _maxFrame = 0;

        private Action _onAnimationUpdate = null;

        #endregion Private members

        #region Public members

        public int AnimationFrame => _animationFrame;

        #endregion Public members

        #region API

        public void OnAnimationUpdateRegister(Action callback, bool toggle)
        {
            if (toggle)
            {
                _onAnimationUpdate += callback;
            }
            else
            {
                _onAnimationUpdate -= callback;
            }
        }

        public void UpdateAnimator(PixelMakerController controller, Animation animation, AnimationClip clip)
        {
            if (controller == null
                || animation == null
                || clip == null)
            {
                return;
            }

            _maxFrame = Mathf.RoundToInt(clip.GetFrames());
            _animationFrame = Mathf.Clamp(_animationFrame, 0, _maxFrame);

            if (_autoUpdate)
            {
                controller.SetAnimationAtFrame(animation, clip, _animationFrame);
                
                if(_onAnimationUpdate != null)
                {
                    _onAnimationUpdate.Invoke();
                }
            }
        }

        #endregion API
    }
}
