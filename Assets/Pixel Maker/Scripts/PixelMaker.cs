#if UNITY_EDITOR

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
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

        private AnimationBuffer _spritesBuffer = null;

        private AnimationBuffer _normalsBuffer = null;

        private bool _inBuildMode = false;

        private bool _inPreviewAnimMode = false;

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

            if (_inPreviewAnimMode)
            {
                EditorGUILayout.HelpBox("In Preview Animation Mode ... ", MessageType.Info);
                if(GUILayout.Button("Stop Preview Animation"))
                {
                    _inPreviewAnimMode = false;
                }
                Repaint();
                return;
            }

            if (_inBuildMode)
            {
                EditorGUILayout.HelpBox("In Build Mode ... ", MessageType.None);
                return;
            }

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
            _spritesBuffer = new AnimationBuffer();
            _normalsBuffer = new AnimationBuffer();
            _inBuildMode = false;
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
            if (GUILayout.Button("Update Model and AnimationClip"))
            {
                var clip = _settings.PreviewConfig.Clip;

                _controller.UpdateModel(_settings.PreviewConfig.Model, out var animation);
                animation.AddClip(clip, clip.name);

                _animation = animation;
            }

            if (_preview != null
                && GUILayout.Button("Apply Material"))
            {
                _preview.UpdateTextureMaterial(_settings);
            }

            if (_preview != null
                && _animation != null
                && GUILayout.Button("Build Spritesheet"))
            {
                _controller.StartCoroutine(Build());
            }

            if (_preview != null
                && _animation != null
                && GUILayout.Button("Preview Animation"))
            {
                _controller.StartCoroutine(PreviewAnim());
            }

            if (_preview != null
                && _animation != null
                && GUILayout.Button("Dump/Save Spritesheet"))
            {
                _controller.StartCoroutine(Dump());
            }
        }

        private IEnumerator Build(bool normal = true)
        {
            _inBuildMode = true;

            int frames = Mathf.RoundToInt(_settings.PreviewConfig.Clip.GetFrames());
            _spritesBuffer.Clear();
            _normalsBuffer.Clear();

            _preview.ClearTextures();

            for (int i = 0, length = frames; i <= length; i++)
            {
                _controller.SetAnimationAtFrame(_animation, _settings.PreviewConfig.Clip, i);
                yield return null;

                _preview.UpdatePreviews(_controller, _settings, _camera);
                _preview.UpdateTextureMaterial(_settings);

                if (_preview.TryGetRenderFrame(out var finalRender))
                {
                    _spritesBuffer.AddBuffer(finalRender);
                }

                if (_preview.TryGetNormalFrame(out var normalRender))
                {
                    _normalsBuffer.AddBuffer(normalRender);
                }
            }

            _inBuildMode = false;
        }

        private IEnumerator Dump()
        {
            yield return Build();

            int scale = _settings.PreviewConfig.RenderScale * 2;

            string prefix = _settings.PreviewConfig.Prefix;

            int columns = _settings.PreviewConfig.SpriteSheetColumns;
            int rows = Mathf.RoundToInt((float)_spritesBuffer.Length / (float)columns);

            if (_settings.PreviewConfig.SingleRow)
            {
                columns = _spritesBuffer.Length;
                rows = 1;
            }

            _spritesBuffer.GetSpriteSheet(out var albedo, new Color(0,0,0,0), scale, scale, FilterMode.Point, rows, columns);
            _normalsBuffer.GetSpriteSheet(out var normal, new Color(0.5f, 0.5f, 1f, 1f), scale, scale, FilterMode.Point, rows, columns);

            DumpSpriteSheet(albedo, $"albedo{prefix}");
            DumpSpriteSheet(normal, $"normal{prefix}");
        }

        private IEnumerator PreviewAnim()
        {
            yield return Build();

            _inPreviewAnimMode = true;

            var sprites = new List<Texture2D>();
            _spritesBuffer.FillTextures(sprites);

            while (_inPreviewAnimMode)
            {
                foreach (var sprite in sprites)
                {
                    if (!_inPreviewAnimMode)
                    {
                        continue;
                    }

                    _preview.PreviewTexture(sprite);
                    yield return new WaitForSecondsRealtime(0.04f);
                }
            }

            _preview.PreviewTexture(null);
        }

        private void DumpSpriteSheet(Texture2D spritesheet, string prefix)
        {
            byte[] bytes = spritesheet.EncodeToPNG();

            string name = _settings.PreviewConfig.Clip.name;
            name = name.Replace(" ", "_").Replace("|", "_");

            string fileName = $"Spritesheet_{prefix}_[{name}].png";

            string directory = _settings.PreviewConfig.DumpDirectory + "/" + fileName;
            System.IO.File.WriteAllBytes(directory, bytes);

            Debug.Log($"Dump of {fileName} \nDirectory : {directory}");
        }

        #endregion Private methods

        #region Statics

        [MenuItem("Tools/Pixel Maker")]
        static protected void OpenWindow()
        {
            var window = GetWindow<PixelMaker>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1230, 1000);
        }

        #endregion Statics
    }
}
