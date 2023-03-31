#if UNITY_EDITOR

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#endif //UNITY_EDITOR

namespace PixelMaker
{
    public class PixelMaker : OdinMenuEditorWindow
    {
        #region Private members

        private PixelMakerSettings _settings = null;

        private Camera _camera = null;

        private PixelMakerController _controller = null;

        private PixelMakerPreview _preview = null;

        private PixelMakerAnimator _animator = null;

        private Animation _animation = null;

        private Editor _previewEditor = null;

        private Editor _animatorEditor = null;

        #endregion Private members

        #region Implementation

        protected override void Initialize()
        {
            BeforeInitialize();
            base.Initialize();
        }

        protected override void DrawEditors()
        {
            base.DrawEditors();

            if (_settings == null)
            {
                EditorGUILayout.HelpBox("There is no configuration asset for Pixel Maker", MessageType.Error);
                return;
            }

            if (StudioSceneUnloaded())
            {
                EditorGUILayout.HelpBox("You need to load a studio scene", MessageType.Error);

                if (GUILayout.Button("Load Studio Scene"))
                {
                    LoadStudioScene();
                    Initialize();
                }

                return;
            }

            var selected = CurrentDrawingTargets[0] is PixelMakerPreview;
            if (!selected)
            {
                return;
            }
            else
            {
                _preview.UpdatePreviews(_controller, _settings, _camera);
                _animator.UpdateAnimator(_controller, _animation, _settings.PreviewConfig.Clip);
            }

            if (_previewEditor == null)
            {
                EditorGUILayout.HelpBox("Error on Preview Config Editor", MessageType.Error);
                return;
            }

            _previewEditor.OnInspectorGUI();
            _animatorEditor.OnInspectorGUI();
            ControllerGUI();

            if (_settings.RealTimeUpdate)
            {
                EditorApplication.delayCall += Repaint;
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                { "Pixel Maker", _preview, EditorIcons.Play},
                { "Configuration", _settings, EditorIcons.SettingsCog},
            };

            return tree;
        }

        #endregion Implementation

        #region Private methods

        private void BeforeInitialize()
        {
            _settings = AssetDatabase.LoadAssetAtPath<PixelMakerSettings>("Assets/Pixel Maker/Pixel Maker.config.asset");
            _controller = FindObjectOfType<PixelMakerController>();
            _camera = FindObjectOfType<Camera>();
            _preview = new PixelMakerPreview();
            _animator = new PixelMakerAnimator();
            _previewEditor = Editor.CreateEditor(_settings.PreviewConfig);
            _animatorEditor = Editor.CreateEditor(_animator);
        }

        private void LoadStudioScene()
        {
            var path = AssetDatabase.GetAssetPath(_settings.Studio);
            EditorSceneManager.OpenScene(path);
        }

        private bool StudioSceneUnloaded()
        {
            if (_settings == null)
            {
                return false;
            }

            var activeScene = EditorSceneManager.GetActiveScene();
            var path = AssetDatabase.GetAssetPath(_settings.Studio);
            return activeScene.path != path;
        }

        private bool StudioSceneLoaded() => !StudioSceneUnloaded();

        private void ControllerGUI()
        {
            if (_animation != null
                && GUILayout.Button("Play Animation Frame"))
            {
                _controller.SetAnimationAtFrame(_animation, _settings.PreviewConfig.Clip, _animator.AnimationFrame);
            }

            if (GUILayout.Button("Update Model and AnimationClip"))
            {
                var clip = _settings.PreviewConfig.Clip;

                _controller.UpdateModel(_settings.PreviewConfig.Model, out var animation);
                animation.AddClip(clip, clip.name);

                _animation = animation;
            }
        }

        #endregion Private methods

        #region Statics

        [MenuItem("Tools/Pixel Maker")]
        static protected void OpenWindow()
        {
            var window = GetWindow<PixelMaker>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 1000);
        }

        #endregion Statics
    }
}
