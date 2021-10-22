﻿using System.Collections.Generic;
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

        private bool showRequired = true;
        private bool showOptional = true;

        [MenuItem("Tools/CC0 Textures Importer")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CC0TexturesImporter), false, "CC0 Textures Importer");
        }

        private void OnGUI()
        {
            EditorGUI.indentLevel = 1;

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
                materialPrefab = (Material)EditorGUILayout.ObjectField("Material Prefab", materialPrefab, typeof(Material), true);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space(20f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Import Textures", GUILayout.Width(200f), GUILayout.Height(25f)))
            {
                outputMaterial = false;
                Import();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Import Material", GUILayout.Width(200f), GUILayout.Height(25f)))
            {
                outputMaterial = true;
                Import();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
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
            Material material = materialPrefab ?
                new Material(materialPrefab) :
                new Material(Shader.Find("Standard"));

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
