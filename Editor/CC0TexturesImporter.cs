using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Zigurous.Importer
{
    public sealed class CC0TexturesImporter : EditorWindow
    {
        public enum MapType
        {
            Albedo,
            Normal,
            Displacement,
            Roughness,
            Metallic,
            Occlusion,
            Emission,
            None
        }

        public enum Resolution
        {
            _1K,
            _2K,
            _4K,
            _8K
        }

        public enum Format
        {
            JPG,
            PNG
        }

        private static readonly WebClient client = new WebClient();
        private string assetId;
        private Resolution resolution = Resolution._2K;
        private Format format = Format.JPG;
        private string outputName;
        private string outputPath = "Assets/";
        private Material outputMaterial;
        private List<string> unzippedFiles = new List<string>();

        [MenuItem("Window/CC0 Textures Importer")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CC0TexturesImporter));
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Asset Settings", EditorStyles.boldLabel);
            this.assetId = EditorGUILayout.TextField("Asset ID", this.assetId);
            this.resolution = (Resolution)EditorGUILayout.EnumPopup("Resolution", this.resolution);
            this.format = (Format)EditorGUILayout.EnumPopup("Format", this.format);

            EditorGUILayout.Space();
            GUILayout.Label("Import Settings", EditorStyles.boldLabel);
            this.outputName = EditorGUILayout.TextField("Output Name", this.outputName);
            this.outputPath = EditorGUILayout.TextField("Output Path", this.outputPath);

            EditorGUILayout.Space();
            if (GUILayout.Button("Import Textures")) {
                Import();
            }

            if (GUILayout.Button("Import as Material")) {
                Import(createMaterial: true);
            }
        }

        private void Import(bool createMaterial = false)
        {
            if (this.assetId == null || this.assetId.Length == 0)
            {
                LogError("Invalid asset id");
                return;
            }

            if (createMaterial) {
                this.outputMaterial = new Material(Shader.Find("Standard"));
            } else {
                this.outputMaterial = null;
            }

            string downloadLink = String.Format(
                format: "https://cc0textures.com/get?file={0}_{1}-{2}.zip",
                arg0: this.assetId,
                arg1: ResolutionToString(this.resolution),
                arg2: FormatToString(this.format));

            DownloadZip(downloadLink);
        }

        private async void DownloadZip(string link)
        {
            Log("Downloading asset: " + this.assetId);

            await DownloadFile(
                uri: link,
                fileName: "CC0Textures.zip",
                onSuccess: DownloadSuccess,
                onError: LogError);
        }

        private static async Task DownloadFile(string uri, string fileName, Action onSuccess, Action<string> onError)
        {
            try
            {
                await CC0TexturesImporter.client.DownloadFileTaskAsync(uri, fileName);
                onSuccess();
            }
            catch (HttpRequestException e)
            {
                onError(e.Message);
            }
        }

        private void DownloadSuccess()
        {
            if (!this.outputPath.EndsWith("/")) {
                this.outputPath += "/";
            }

            string zipFilePath = Application.dataPath;
            int removeIndex = zipFilePath.LastIndexOf("/Assets");
            zipFilePath = zipFilePath.Remove(removeIndex, "/Assets".Length);
            zipFilePath += "/CC0Textures.zip";

            Log("Importing files from " + zipFilePath);

            if (UnzipFiles(zipFilePath))
            {
                AssetDatabase.Refresh();

                if (this.outputMaterial != null) {
                    CreateMaterialAsset();
                }
            }

            Log("Finished");
        }

        private bool UnzipFiles(string filePath)
        {
            this.unzippedFiles.Clear();

            if (!System.IO.File.Exists(filePath))
            {
                LogError("Zip file does not exist");
                return false;
            }

            FileStream fs = null;

            try {
                fs = new FileStream(filePath, FileMode.Open);
            } catch (Exception e) {
                LogError(e.Message);
                return false;
            }

            if (fs == null) {
                return false;
            }

            try
            {
                ZipFile zipFile = new ZipFile(fs);

                if (!zipFile.TestArchive(true))
                {
                    LogError("Zip file failed integrity check");
                    zipFile.IsStreamOwner = false;
                    zipFile.Close();
                    fs.Close();
                    return false;
                }
                else
                {
                    string outputPath = this.outputPath.StartsWith("/") ? this.outputPath.Remove(0, 1) : this.outputPath;
                    outputPath = outputPath.Replace("Assets/", "");
                    outputPath = Application.dataPath + "/" + outputPath;

                    foreach (ZipEntry zipEntry in zipFile)
                    {
                        if (!zipEntry.IsFile) {
                            continue;
                        }

                        String entryFileName = zipEntry.Name;

                        if (entryFileName.Contains("DS_Store")) {
                            continue;
                        }

                        Log("Unpacking zip file entry: " + entryFileName);

                        byte[] buffer = new byte[4096];
                        Stream zipStream = zipFile.GetInputStream(zipEntry);
                        string fileName = Path.GetFileName(RenameFile(entryFileName));
                        string fullFilePath = outputPath + fileName;
                        Debug.Log(fullFilePath);

                        // Unzip file in buffered chunks. This is just as
                        // fast as unpacking to a buffer the full size of
                        // the file, but does not waste memory. The "using"
                        // will close the stream even if an exception occurs.
                        using (FileStream streamWriter = File.Create(fullFilePath)) {
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                        }

                        this.unzippedFiles.Add(fileName);
                    }

                    zipFile.IsStreamOwner = false;
                    zipFile.Close();
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                LogError(e.Message);
                return false;
            }

            return true;
        }

        private string RenameFile(string fileName)
        {
            string[] components = fileName.Split('_'); // <AssetID>_<Resolution>_<Map>.<FileExtension>
            string assetName = this.outputName != null && this.outputName.Length > 0 ? this.outputName : components[0];
            string mapName = components.Length >= 3 ? components[2] : "";
            mapName = mapName.Replace("Color", "Albedo");
            mapName = mapName.Replace("Metalness", "Metallic");
            mapName = mapName.Replace("AmbientOcclusion", "Occlusion");
            return assetName + "-" + mapName;
        }

        private void CreateMaterialAsset()
        {
            if (this.outputMaterial == null) {
                return;
            }

            Log("Creating new material");

            string outputPath = this.outputPath.StartsWith("/") ? this.outputPath.Remove(0, 1) : this.outputPath;
            outputPath = outputPath.StartsWith("Assets") ? outputPath : "Assets/" + outputPath;
            outputPath = outputPath.Replace("//", "/");

            if (this.unzippedFiles != null)
            {
                foreach (string fileName in this.unzippedFiles)
                {
                    string filePath = outputPath + fileName;
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                    MapType mapType = MapTypeFromFileName(fileName);
                    string mapName = MapTypeToTextureName(mapType);

                    if (mapName != null) {
                        this.outputMaterial.SetTexture(mapName, texture);
                    }
                }
            }

            string outputName = this.outputName != null && this.outputName.Length > 0 ? this.outputName : this.assetId;
            string assetName = outputPath + outputName + ".mat";

            AssetDatabase.CreateAsset(this.outputMaterial, assetName);
            AssetDatabase.Refresh();
        }

        private MapType MapTypeFromFileName(string fileName)
        {
            if (fileName.Contains("Albedo")) {
                return MapType.Albedo;
            } else if (fileName.Contains("Normal")) {
                return MapType.Normal;
            } else if (fileName.Contains("Displacement")) {
                return MapType.Displacement;
            } else if (fileName.Contains("Roughness")) {
                return MapType.Roughness;
            } else if (fileName.Contains("Metallic")) {
                return MapType.Metallic;
            } else if (fileName.Contains("Occlusion")) {
                return MapType.Occlusion;
            } else if (fileName.Contains("Emission")) {
                return MapType.Emission;
            } else {
                return MapType.None;
            }
        }

        private string MapTypeToTextureName(MapType mapType)
        {
            switch (mapType)
            {
                case MapType.Albedo:
                    return "_MainTex";
                case MapType.Normal:
                    return "_BumpMap";
                case MapType.Displacement:
                    return "_ParallaxMap";
                case MapType.Roughness:
                    return "_SpecGlossMap";
                case MapType.Metallic:
                    return "_MetallicGlossMap";
                case MapType.Occlusion:
                    return "_OcclusionMap";
                case MapType.Emission:
                    return "_EmissionMap";
                default:
                    return null;
            }
        }

        private string ResolutionToString(Resolution resolution)
        {
            switch (resolution)
            {
                case Resolution._1K:
                    return "1K";
                case Resolution._2K:
                    return "2K";
                case Resolution._4K:
                    return "4K";
                case Resolution._8K:
                    return "8K";
                default:
                    return "2K";
            }
        }

        private string FormatToString(Format format)
        {
            switch (format)
            {
                case Format.JPG:
                    return "JPG";
                case Format.PNG:
                    return "PNG";
                default:
                    return "JPG";
            }
        }

        private void Log(string message)
        {
            Debug.Log("[CC0TexturesImporter]: " + message, this);
        }

        private void LogError(string message)
        {
            Debug.LogError("[CC0TexturesImporter]: " + message, this);
        }

    }

}
