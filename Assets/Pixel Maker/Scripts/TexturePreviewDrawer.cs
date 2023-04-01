#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

#endif //UNITY_EDITOR


namespace PixelMaker
{
    [CustomPropertyDrawer(typeof(TexturePreviewAttribute))]
    public class TexturePreviewDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // If the property is not a texture, do nothing
            if (property.propertyType != SerializedPropertyType.ObjectReference 
                || property.objectReferenceValue == null 
                || !(property.objectReferenceValue is Texture2D))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            // Get the texture and the preview size
            Texture2D texture = property.objectReferenceValue as Texture2D;
            TexturePreviewAttribute previewAttribute = attribute as TexturePreviewAttribute;
            texture.filterMode = previewAttribute.filterMode;
            
            // Draw the texture preview
            Rect texturePreviewRect = new Rect(position.x, position.y, previewAttribute.width, previewAttribute.height);
            Rect backgroundRect = new Rect(position.x, position.y, previewAttribute.width, previewAttribute.height);

            if (previewAttribute.background)
            {
                var style = new GUIStyle(GUI.skin.box);

                style.normal.background = CreateColor(Color.black);
                style.stretchWidth = true;
                style.stretchHeight = true;

                EditorGUI.LabelField(backgroundRect, GUIContent.none, style);
            }

            GUI.DrawTexture(texturePreviewRect, texture, ScaleMode.ScaleToFit);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // If the property is not a texture, return the default height
            if (property.propertyType != SerializedPropertyType.ObjectReference 
                || property.objectReferenceValue == null 
                || !(property.objectReferenceValue is Texture2D))
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }

            TexturePreviewAttribute previewAttribute = (TexturePreviewAttribute)attribute;
            return Mathf.Max(previewAttribute.height, EditorGUI.GetPropertyHeight(property, label));
        }

        private Texture2D CreateColor(Color color)
        {
            var tex = new Texture2D(1,1, TextureFormat.ARGB32, false);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }
    }
}
