using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zigurous.AssetDownloader
{
    public class AmbientCG : AssetSource
    {
        public override AssetSourceType sourceType => AssetSourceType.AmbientCG;
        public override string sourceUrl => "https://ambientcg.com";

        private static bool createMaterial;

        public override void OnSettingsGUI(ImportContext context)
        {
            context.resolution = (TextureResolution)EditorGUILayout.EnumPopup("Resolution", context.resolution);
            context.format = (ImageFormat)EditorGUILayout.EnumPopup("Format", context.format);
        }

        public override void OnOutputGUI(ImportContext context)
        {
            context.materialPreset = (Material)EditorGUILayout.ObjectField("Material Preset", context.materialPreset, typeof(Material), true);
        }

        public override void OnImportGUI(ImportContext context)
        {
            if (CustomGUI.CenteredButton("Import Textures"))
            {
                createMaterial = false;
                Import(context);
            }

            if (CustomGUI.CenteredButton("Import Material"))
            {
                createMaterial = true;
                Import(context);
            }
        }

        public override string GetDownloadUrl(ImportContext context)
        {
            return $"{sourceUrl}/get?file={context.assetId}_{context.resolution.GetName()}-{context.format.GetName()}.zip";
        }

        public override bool Filter(string fileName)
        {
            return fileName.GetTextureMapType() != TextureMapType.None;
        }

        public override string Rename(string fileName)
        {
            return fileName.Replace("Color", "Albedo")
                           .Replace("Metalness", "Metallic")
                           .Replace("AmbientOcclusion", "Occlusion");
        }

        public override void ImportComplete(ImportContext context, List<string> files)
        {
            ImportUtils.UpdateTextureImportSettings(files, context);

            if (createMaterial) {
                ImportUtils.CreateMaterial(files, context);
            }

            base.ImportComplete(context, files);
        }

    }

}
