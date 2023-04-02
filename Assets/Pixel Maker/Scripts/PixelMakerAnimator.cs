using Sirenix.OdinInspector;
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

        public int AnimationFrame => _animationFrame;

        #endregion Private members

        #region API

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
            }
        }

        #endregion API
    }
}
