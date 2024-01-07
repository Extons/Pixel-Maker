using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PixelMaker
{
    [CreateAssetMenu(menuName = "Tools/PixelMaker/" + nameof(PixelMakerPreviewConfig))]
    public class PixelMakerPreviewConfig : ScriptableObject
    {
        #region Data

        private int[] _bitsValues = { 1, 2, 4, 8, 16, 32, 64, 128, 256 };

        #endregion Data

        #region Serialized members

        [FoldoutGroup("Rendering Settings")]
        [SerializeField, Required]
        [Range(1, 2048)]
        private int _sourceRenderScale = 1;

        [FoldoutGroup("Rendering Settings")]
        [SerializeField, Required]
        [Range(1, 2048)]
        private int _renderScale = 1;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        private Color _background = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        private bool _usePalette = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        private bool _convertToBit = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        [InlineEditor]
        [ShowIf(nameof(_usePalette))]
        private ColorPalette _palette = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        private Color _outlineColor = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        private float _outlineWidth = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        private float _saturation = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        private float _contrast = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        [ValueDropdown(nameof(_bitsValues))]
        [ShowIf(nameof(_convertToBit))]
        private int _bitValue = default;

        [FoldoutGroup("Animator")]
        [SerializeField, Required]
        [ValueDropdown(nameof(GetAllModels), NumberOfItemsBeforeEnablingSearch = 0)]
        private GameObject _model = default;

        [FoldoutGroup("Animator")]
        [SerializeField, Required]
        [ValueDropdown(nameof(GetAnimationClipsFromModels), NumberOfItemsBeforeEnablingSearch = 0)]
        private AnimationClip _clip = default;

        [BoxGroup("Model/Model Position", GroupName = "Position")]
        [HorizontalGroup("Model/Model Position/")]
        [LabelText("X :"), LabelWidth(17)]
        [SerializeField, Required]
        private float _modelPositionX = default;

        [HorizontalGroup("Model/Model Position/")]
        [LabelText("Y :"), LabelWidth(17)]
        [SerializeField, Required]
        private float _modelPositionY = default;

        [HorizontalGroup("Model/Model Position/")]
        [LabelText("Z :"), LabelWidth(17)]
        [SerializeField, Required]
        private float _modelPositionZ = default;

        [BoxGroup("Model/Model Rotation", GroupName = "Rotation")]
        [HorizontalGroup("Model/Model Rotation/")]
        [LabelText("X :"), LabelWidth(17)]
        [SerializeField, Required]
        private float _modelRotationX = default;

        [HorizontalGroup("Model/Model Rotation/")]
        [LabelText("Y :"), LabelWidth(17)]
        [SerializeField, Required]
        private float _modelRotationY = default;

        [HorizontalGroup("Model/Model Rotation/")]
        [LabelText("Z :"), LabelWidth(17)]
        [SerializeField, Required]
        private float _modelRotationZ = default;

        [FoldoutGroup("Model")]
        [LabelText("Scale :"), LabelWidth(45)]
        [SerializeField, Required]
        private float _modelScale = default;

        [BoxGroup("Camera/Camera Position", GroupName = "Position")]
        [HorizontalGroup("Camera/Camera Position/")]
        [LabelText("X :"), LabelWidth(17)]
        [SerializeField, Required]
        private float _cameraPositionX = default;

        [HorizontalGroup("Camera/Camera Position/")]
        [LabelText("Y :"), LabelWidth(17)]
        [SerializeField, Required]
        private float _cameraPositionY = default;

        [FoldoutGroup("Camera")]
        [LabelText("Scale :"), LabelWidth(45)]
        [SerializeField, Required]
        private float _cameraScale = default;

        [BoxGroup("Dump Settings")]
        [SerializeField, Required]
        private bool _singleRow = true;

        [BoxGroup("Dump Settings")]
        [SerializeField, Required]
        [Range(0f, 1f)]
        private float _animationReduice = default;

        [BoxGroup("Dump Settings")]
        [SerializeField, Required]
        [HideIf(nameof(_singleRow))]
        private int _spriteSheetColumns = default;

        [BoxGroup("Dump Settings")]
        [SerializeField]
        private string _prefix = default;

        [BoxGroup("Dump Settings")]
        [SerializeField]
        [FolderPath(AbsolutePath = true)]
        private string _dumpDirectory = default;

        #endregion Serialized members

        #region Public members

        public Color Background => _background;

        public bool ConvertToBit => _convertToBit;

        public bool UsePalette => _usePalette;

        public ColorPalette Palette => _palette;

        public Color OutlineColor => _outlineColor;

        public float OutlineWidth => _outlineWidth;

        public float Saturation => _saturation;

        public float Contrast => _contrast;

        public float BitValue => _bitValue;

        public int SourceRenderScale => _sourceRenderScale;

        public int RenderScale => _renderScale;

        public GameObject Model => _model;

        public AnimationClip Clip => _clip;

        public bool SingleRow => _singleRow;

        public int SpriteSheetColumns => _spriteSheetColumns;

        public float AnimationReduice => _animationReduice;

        public float ModelPositionX => _modelPositionX;

        public float ModelPositionY => _modelPositionY;

        public float ModelPositionZ => _modelPositionZ;

        public float ModelRotationX => _modelRotationX;

        public float ModelRotationY => _modelRotationY;

        public float ModelRotationZ => _modelRotationZ;

        public float ModelScale => _modelScale;

        public float CameraPositionX => _cameraPositionX;

        public float CameraPositionY => _cameraPositionY;

        public float CameraScale => _cameraScale;

        public string Prefix => _prefix;

        public string DumpDirectory => _dumpDirectory;

        #endregion Public members

        #region Private methods
        
        private IEnumerable<AnimationClip> GetAnimationClipsFromModels()
        {
            List<AnimationClip> animationClips = new List<AnimationClip>();

            var guids = AssetDatabase.FindAssets("t:animation");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var animClip = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;

                if (animClip != null)
                {
                    animationClips.Add(animClip);
                }
            }

            guids = AssetDatabase.FindAssets("t:model");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var subObjects = AssetDatabase.LoadAllAssetsAtPath(path);

                foreach (var subObject in subObjects)
                {
                    var clip = subObject as AnimationClip;
                    if (clip != null
                        && !animationClips.Contains(clip))
                    {
                        animationClips.Add(clip);
                    }
                }
            }

            return animationClips;
        }

        private IEnumerable<GameObject> GetAllModels()
        {
            var models = new List<GameObject>();

            var guids = AssetDatabase.FindAssets("t:model");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var model = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

                if (model != null)
                {
                    models.Add(model);
                }
            }

            return models;
        }

        #endregion Private methods
    }
}
