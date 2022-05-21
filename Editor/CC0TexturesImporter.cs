using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Zigurous.Importer.CC0Textures
{
    public sealed class CC0TexturesImporter : EditorWindow
    {
        private string assetId;
        private TextureResolution resolution = TextureResolution._2K;
        private ImageFormat format = ImageFormat.JPG;
        private Material materialPreset;

        private string outputName;
        private string outputPath = "Assets/";
        private bool outputMaterial = false;

        private bool showRequired = true;
        private bool showOptional = true;

        [MenuItem("Tools/CC0 Textures Importer")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CC0TexturesImporter), false, "CC0 Textures Importer");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("CC0 Textures Importer", EditorStyles.largeLabel);
            EditorGUILayout.LabelField("Enter an Asset ID from the website, choose the desired Texture Resolution and Format, then click \"Import Textures\" or \"Import Material\".", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            using (new EditorGUI.IndentLevelScope(1))
            {
                showRequired = EditorGUILayout.BeginFoldoutHeaderGroup(showRequired, "Required");

                if (showRequired)
                {
                    assetId = EditorGUILayout.TextField("Asset ID", assetId);
                    resolution = (TextureResolution)EditorGUILayout.EnumPopup("Resolution", resolution);
                    format = (ImageFormat)EditorGUILayout.EnumPopup("Format", format);
                    EditorGUILayout.Space();
                }

                EditorGUILayout.EndFoldoutHeaderGroup();

                showOptional = EditorGUILayout.BeginFoldoutHeaderGroup(showOptional, "Optional");

                if (showOptional)
                {
                    outputName = EditorGUILayout.TextField("Output Name", outputName);
                    outputPath = EditorGUILayout.TextField("Output Path", outputPath);
                    materialPreset = (Material)EditorGUILayout.ObjectField("Material Preset", materialPreset, typeof(Material), true);
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            EditorGUILayout.Space(20f);

            if (DrawCenteredButton("Import Textures"))
            {
                outputMaterial = false;
                Import();
            }

            if (DrawCenteredButton("Import Material"))
            {
                outputMaterial = true;
                Import();
            }

            if (DrawCenteredButton("Browse Materials")) {
                Application.OpenURL("https://ambientcg.com");
            }
        }

        private bool DrawCenteredButton(string label, float width = 200f, float height = 25f)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            bool clicked = GUILayout.Button(label, GUILayout.Width(width), GUILayout.Height(height));

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(1f);

            return clicked;
        }

        private void Import()
        {
            Log.context = this;

            if (assetId == null || assetId.Length == 0)
            {
                Log.Error("Invalid asset id");
                return;
            }

            if (!outputPath.EndsWith("/")) {
                outputPath += "/";
            }

            WebClient.DownloadAsset(assetId, resolution, format, outputPath, outputName, ImportComplete);
        }

        private void ImportComplete(List<string> files)
        {
            if (files == null) {
                return;
            }

            // Refresh the assets so we can update any import settings and/or
            // assign textures to a new material
            AssetDatabase.Refresh();

            // Find the normal map and change the texture type import settings
            foreach (string fileName in files)
            {
                if (fileName.Contains("Normal"))
                {
                    string filePath = GetAssetPath(fileName);
                    TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
                    importer.textureType = TextureImporterType.NormalMap;
                    AssetDatabase.WriteImportSettingsIfDirty(filePath);
                    AssetDatabase.Refresh();
                    break;
                }
            }

            // Create a new material with the imported texture files
            if (outputMaterial) {
                CreateMaterialAsset(files);
            }

            Log.Message("Finished");
        }

        private void CreateMaterialAsset(List<string> files)
        {
            Log.Message("Creating new material");

            // Create a new material from the prefab or use Unity's default
            // standard shader material
            Material material = materialPreset ?
                new Material(materialPreset) :
                new Material(GetDefaultShader());

            // Assign each texture map to the right slot in the material
            foreach (string fileName in files)
            {
                Texture2D texture = LoadTexture(fileName);
                TextureMapType mapType = fileName.ToTextureMapType();
                string mapName = mapType.ToTextureName();

                if (mapName != null) {
                    material.SetTexture(mapName, texture);
                }
            }

            // Save the material as an asset and refresh
            string name = outputName != null && outputName.Length > 0 ? outputName : assetId;
            string assetName = outputPath + name + ".mat";
            AssetDatabase.CreateAsset(material, assetName);
            AssetDatabase.Refresh();
        }

        private Shader GetDefaultShader()
        {
            if (GraphicsSettings.currentRenderPipeline != null) {
                return GraphicsSettings.currentRenderPipeline.defaultShader;
            } else {
                return Shader.Find("Standard");
            }
        }

        private Texture2D LoadTexture(string fileName)
        {
            string filePath = GetAssetPath(fileName);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
        }

        private string GetAssetPath(string fileName)
        {
            string path = outputPath.StartsWith("/") ? outputPath.Remove(0, 1) : outputPath;
            path = path.StartsWith("Assets") ? path : "Assets/" + path;
            path = path.Replace("//", "/");
            return path + fileName;
        }

    }

}
