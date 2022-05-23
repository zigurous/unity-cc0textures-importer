using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Zigurous.AssetDownloader
{
    public class ImportUtils
    {
        public static void UpdateTextureImportSettings(List<string> textureAssets, ImportContext context)
        {
            Log.Message("Updating texture import settings");

            AssetDatabase.Refresh();

            foreach (string asset in textureAssets)
            {
                string assetPath = context.GetAssetPath(asset);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                if (importer == null) {
                    continue;
                }

                importer.maxTextureSize = context.resolution.GetMaxTextureSize();
                importer.textureType = asset.Contains("Normal") ?
                    TextureImporterType.NormalMap :
                    TextureImporterType.Default;

                AssetDatabase.WriteImportSettingsIfDirty(assetPath);
            }

            AssetDatabase.Refresh();
        }

        public static Material CreateMaterial(List<string> textureAssets, ImportContext context)
        {
            Log.Message("Creating new material");

            Material material = context.materialPreset ?
                new Material(context.materialPreset) :
                new Material(GetDefaultShader());

            foreach (string asset in textureAssets)
            {
                TextureMapType mapType = asset.GetTextureMapType();

                if (mapType == TextureMapType.None) {
                    continue;
                }

                string mapName = mapType.GetTextureMapName();
                string assetPath = context.GetAssetPath(asset);

                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

                if (texture != null) {
                    material.SetTexture(mapName, texture);
                }
            }

            string materialName = context.GetOutputName($"{context.assetId}.mat");
            string materialPath = context.GetAssetPath(materialName);

            AssetDatabase.CreateAsset(material, materialPath);
            AssetDatabase.Refresh();

            return material;
        }

        public static Shader GetDefaultShader()
        {
            if (GraphicsSettings.currentRenderPipeline != null) {
                return GraphicsSettings.currentRenderPipeline.defaultShader;
            } else {
                return Shader.Find("Standard");
            }
        }

    }

}
