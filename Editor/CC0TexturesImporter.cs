using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zigurous.Importer.CC0Textures
{
    public sealed class CC0TexturesImporter : EditorWindow
    {
        private string assetId;
        private TextureResolution resolution = TextureResolution._2K;
        private ImageFormat format = ImageFormat.JPG;
        private Material materialPrefab;

        private string outputName;
        private string outputPath = "Assets/";
        private bool outputMaterial = false;

        private bool fold = true;

        [MenuItem("Window/CC0Textures Importer")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CC0TexturesImporter), false, "CC0Textures Importer");
        }

        private void OnGUI()
        {
            this.fold = EditorGUILayout.BeginFoldoutHeaderGroup(this.fold, "Import Settings");

            if (this.fold)
            {
                this.assetId = EditorGUILayout.TextField("Asset ID", this.assetId);
                this.resolution = (TextureResolution)EditorGUILayout.EnumPopup("Resolution", this.resolution);
                this.format = (ImageFormat)EditorGUILayout.EnumPopup("Format", this.format);
                this.outputName = EditorGUILayout.TextField("Output Name", this.outputName);
                this.outputPath = EditorGUILayout.TextField("Output Path", this.outputPath);
                this.materialPrefab = (Material)EditorGUILayout.ObjectField("Material Prefab", this.materialPrefab, typeof(Material), true);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Import Textures", GUILayout.Width(200.0f), GUILayout.Height(25.0f)))
            {
                this.outputMaterial = false;
                Import();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Import Material", GUILayout.Width(200.0f), GUILayout.Height(25.0f)))
            {
                this.outputMaterial = true;
                Import();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void Import()
        {
            Log.context = this;

            if (this.assetId == null || this.assetId.Length == 0)
            {
                Log.Error("Invalid asset id");
                return;
            }

            if (!this.outputPath.EndsWith("/")) {
                this.outputPath += "/";
            }

            WebClient.DownloadAsset(this.assetId, this.resolution, this.format, this.outputPath, this.outputName, ImportComplete);
        }

        private void ImportComplete(List<string> files)
        {
            if (files == null) {
                return;
            }

            AssetDatabase.Refresh();

            if (this.outputMaterial) {
                CreateMaterialAsset(files);
            }

            Log.Message("Finished");
        }

        private void CreateMaterialAsset(List<string> files)
        {
            Log.Message("Creating new material");

            Material material = this.materialPrefab ?
                new Material(this.materialPrefab) :
                new Material(Shader.Find("Standard"));

            string outputPath = this.outputPath.StartsWith("/") ? this.outputPath.Remove(0, 1) : this.outputPath;
            outputPath = outputPath.StartsWith("Assets") ? outputPath : "Assets/" + outputPath;
            outputPath = outputPath.Replace("//", "/");

            foreach (string fileName in files)
            {
                string filePath = outputPath + fileName;
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                TextureMapType mapType = fileName.ToTextureMapType();
                string mapName = mapType.ToTextureName();

                if (mapName != null) {
                    material.SetTexture(mapName, texture);
                }
            }

            string outputName = this.outputName != null && this.outputName.Length > 0 ? this.outputName : this.assetId;
            string assetName = outputPath + outputName + ".mat";

            AssetDatabase.CreateAsset(material, assetName);
            AssetDatabase.Refresh();
        }

    }

}
