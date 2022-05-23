using UnityEditor;
using UnityEngine;

namespace Zigurous.AssetDownloader
{
    public static class CustomGUI
    {
        public static bool CenteredButton(string label, float width = 200f, float height = 25f)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            bool clicked = GUILayout.Button(label, GUILayout.Width(width), GUILayout.Height(height));

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(1f);

            return clicked;
        }

    }

}
