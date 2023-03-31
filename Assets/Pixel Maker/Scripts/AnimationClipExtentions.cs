using UnityEngine;

namespace PixelMaker
{
    static public class AnimationClipExtentions
    {
        static public float GetFrames(this AnimationClip clip)
        {
            return clip.length * clip.frameRate;
        }
    }
}
