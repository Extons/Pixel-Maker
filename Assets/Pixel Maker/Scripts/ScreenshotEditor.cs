using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PixelMaker
{
    [CustomEditor(typeof(Camera))]
    public class ScreenshotEditor : Editor
    {
        #region Implementation

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            if (GUILayout.Button("RGBA32 Screenshot"))
            {
                string folder = "Assets/Screenshots";
                string fileNamePrefix = "rgba32";

                CaptureScreenshot(folder, fileNamePrefix, true);
            }

            if (GUILayout.Button("RGB32 Screenshot"))
            {
                string folder = "Assets/Screenshots";
                string fileNamePrefix = "rgb32";

                CaptureScreenshot(folder, fileNamePrefix, false);
            }
        }

        #endregion Implementation

        #region Private methods

        private void CaptureScreenshot(string folder, string prefix, bool rgba)
        {
            Camera camera = (Camera)target;

            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }

            string fileName = $"{prefix}_{System.DateTime.Now:yyyyMMddHHmmss}.png";
            string filePath = System.IO.Path.Combine(folder, fileName);

            if (rgba)
            {
                Capture(filePath, camera);
            }
            else
            {
                ScreenCapture.CaptureScreenshot(filePath);
            }

            Debug.Log($"Capture d'écran enregistrée : {filePath}");
            AssetDatabase.Refresh();
        }

        private void Capture(string path, Camera camera)
        {
            int w = camera.pixelWidth;
            int h = camera.pixelHeight;
            var rt = new RenderTexture(w, h, 32);
            camera.targetTexture = rt;
            var screenShot = new Texture2D(w, h, TextureFormat.ARGB32, false);

            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            screenShot.Apply();
            camera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(rt);

            File.WriteAllBytes(path, screenShot.EncodeToPNG());

            Debug.Log("Directory: " + path);
            AssetDatabase.Refresh();
        } 

        #endregion Private methods
    }
}