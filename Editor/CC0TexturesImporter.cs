using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
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
        public enum Resolution
        {
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
        private string outputPath = "/";

        [MenuItem("Window/CC0Textures Importer")]
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
            if (GUILayout.Button("Import")) {
                Import();
            }
        }

        private void Import()
        {
            if (this.assetId == null || this.assetId.Length == 0)
            {
                LogError("Invalid asset id");
                return;
            }

            string downloadLink = String.Format(
                format: "https://cc0textures.com/get?file={0}_{1}-{2}.zip",
                arg0: this.assetId,
                arg1: ResolutionToString(this.resolution),
                arg2: FormatToString(this.format));

            DownloadZip(downloadLink);
        }

        private string ResolutionToString(Resolution resolution)
        {
            switch (resolution)
            {
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

        private async void DownloadZip(string link)
        {
            Log("Downloading asset...");

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
            Log("Importing asset...");

            string zipFile = Application.dataPath;
            string pathToRemove = "/Assets";
            int removeIndex = zipFile.LastIndexOf(pathToRemove);
            zipFile = zipFile.Remove(removeIndex, pathToRemove.Length);
            zipFile += "/CC0Textures.zip";

            if (!this.outputPath.EndsWith("/")) {
                this.outputPath += "/";
            }

            UnzipFiles(zipFile, Application.dataPath);
            AssetDatabase.Refresh();
            Log("Finished");
        }

        private void UnzipFiles(string filePath, string outputDirectory)
        {
            if (!System.IO.File.Exists(filePath))
            {
                LogError("Zip file does not exist");
                return;
            }

            FileStream fs = null;

            try {
                fs = new FileStream(filePath, FileMode.Open);
            } catch (Exception e) {
                LogError(e.Message);
            }

            if (fs != null)
            {
                try
                {
                    ZipFile zipFile = new ZipFile(fs);
                    int numFiles = 0;

                    if (!zipFile.TestArchive(true))
                    {
                        LogError("Zip file failed integrity check");
                        zipFile.IsStreamOwner = false;
                        zipFile.Close();
                        fs.Close();
                    }
                    else
                    {
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
                            string fullFilePath = outputDirectory + this.outputPath + Path.GetFileName(RenameFile(entryFileName));

                            // Unzip file in buffered chunks. This is just as
                            // fast as unpacking to a buffer the full size of
                            // the file, but does not waste memory. The "using"
                            // will close the stream even if an exception occurs.
                            using (FileStream streamWriter = File.Create(fullFilePath)) {
                                StreamUtils.Copy(zipStream, streamWriter, buffer);
                            }

                            numFiles++;
                        }

                        zipFile.IsStreamOwner = false;
                        zipFile.Close();
                        fs.Close();
                    }
                }
                catch (Exception e) {
                    LogError(e.Message);
                }
            }
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
