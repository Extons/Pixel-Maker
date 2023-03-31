using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PixelMaker
{
    [CreateAssetMenu(menuName = "Tools/PixelMaker/" + nameof(PixelMakerPreviewConfig))]
    public class PixelMakerPreviewConfig : ScriptableObject
    {
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
        private Color _toonColor = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        [ColorUsage(true, true)]
        private Color _toonAmbiantColor = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        [ColorUsage(true, true)]
        private Color _toonSpecularColor = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        [ColorUsage(true, true)]
        private Color _toonRimColor = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        [Range(0f, 1f)]
        private float _toonRimAmount = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        [Range(0f, 1f)]
        private float _toonRimThreshold = default;

        [FoldoutGroup("Toon Shader")]
        [SerializeField, Required]
        private float _toonGlossiness = default;

        [FoldoutGroup("Animator")]
        [SerializeField, Required]
        [ValueDropdown(nameof(GetAllModels), NumberOfItemsBeforeEnablingSearch = 0)]
        private GameObject _model = default;

        [FoldoutGroup("Animator")]
        [SerializeField, Required]
        [ValueDropdown(nameof(GetAnimationClips), NumberOfItemsBeforeEnablingSearch = 0)]
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

        #endregion Serialized members

        #region Public members

        public Color Background => _background;

        public Color ToonColor => _toonColor;

        public Color ToonAmbiantColor => _toonAmbiantColor;

        public Color ToonSpecularColor => _toonSpecularColor;

        public Color ToonRimColor => _toonRimColor;

        public float ToonRimAmount => _toonRimAmount;

        public float ToonRimThreshold => _toonRimThreshold;

        public float ToonGlossiness => _toonGlossiness;

        public int SourceRenderScale => _sourceRenderScale;

        public int RenderScale => _renderScale;

        public GameObject Model => _model;

        public AnimationClip Clip => _clip;

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

        #endregion Public members

        #region Private methods

        private IEnumerable<AnimationClip> GetAnimationClips()
        {
            List<AnimationClip> clips = new List<AnimationClip>();

            var guids = AssetDatabase.FindAssets("t:AnimationClip");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var animClip = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;

                if (animClip != null)
                {
                    clips.Add(animClip);
                }
            }

            return clips;
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
