using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using UnityEngine;

namespace Zigurous.Importer.CC0Textures
{
    public static class WebClient
    {
        private static readonly System.Net.WebClient client = new System.Net.WebClient();

        public static async void DownloadAsset(string assetId, TextureResolution resolution, ImageFormat format, string outputPath, string outputName, Action<List<string>> onComplete)
        {
            try
            {
                // Construct the full download link to the asset
                string downloadLink = String.Format(
                    format: "https://ambientcg.com/get?file={0}_{1}-{2}.zip",
                    arg0: assetId,
                    arg1: resolution.GetName(),
                    arg2: format.GetName());

                // Wait for the zip file to be downloaded
                Log.Message("Downloading asset: " + assetId);
                await client.DownloadFileTaskAsync(downloadLink, "textures.zip");

                // Extract the files from the zip
                string zip = GetZipFilePath();
                Log.Message("Importing files from " + zip);
                List<string> files = UnzipFiles(zip, outputPath, outputName);

                // Cleanup resources and complete download
                System.IO.File.Delete(zip);
                onComplete(files);
            }
            catch (HttpRequestException e)
            {
                Log.Error(e.Message);
            }
        }

        private static List<string> UnzipFiles(string zip, string outputPath, string outputName)
        {
            if (!System.IO.File.Exists(zip))
            {
                Log.Error("Zip file does not exist");
                return null;
            }

            FileStream fs = null;

            try {
                fs = new FileStream(zip, FileMode.Open);
            } catch (Exception e) {
                Log.Error(e.Message);
                return null;
            }

            if (fs == null) {
                return null;
            }

            try
            {
                ZipFile zipFile = new ZipFile(fs);

                if (!zipFile.TestArchive(true))
                {
                    Log.Error("Zip file failed integrity check");
                    zipFile.IsStreamOwner = false;
                    zipFile.Close();
                    fs.Close();
                    return null;
                }
                else
                {
                    List<string> unzippedFiles = new List<string>();

                    string dataPath = outputPath.StartsWith("/") ? outputPath.Remove(0, 1) : outputPath;
                    dataPath = dataPath.Replace("Assets/", "");
                    dataPath = Application.dataPath + "/" + dataPath;

                    foreach (ZipEntry zipEntry in zipFile)
                    {
                        if (!zipEntry.IsFile) {
                            continue;
                        }

                        String entryFileName = zipEntry.Name;

                        if (entryFileName.Contains("DS_Store")) {
                            continue;
                        }

                        Log.Message("Unpacking zip file entry: " + entryFileName);

                        byte[] buffer = new byte[4096];
                        Stream zipStream = zipFile.GetInputStream(zipEntry);

                        string filePath = GetFilePath(entryFileName, outputName);
                        string fullFilePath = dataPath + filePath;

                        // Create the file directory if it does not exist
                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(fullFilePath);
                        fileInfo.Directory.Create();

                        // Unzip file in buffered chunks. This is just as fast
                        // as unpacking to a buffer the full size of the file,
                        // but does not waste memory. The "using" will close the
                        // stream even if an exception occurs.
                        using (FileStream streamWriter = File.Create(fullFilePath)) {
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                        }

                        unzippedFiles.Add(filePath);
                    }

                    zipFile.IsStreamOwner = false;
                    zipFile.Close();
                    fs.Close();

                    return unzippedFiles;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        private static string GetFilePath(string fileName, string outputName)
        {
            string[] components = fileName.Split('_'); // <AssetID>_<Resolution>_<Map>.<FileExtension>
            string assetName = outputName != null && outputName.Length > 0 ? outputName : components[0];
            string mapName = components.Length >= 3 ? components[2] : "";
            mapName = mapName.Replace("Color", "Albedo");
            mapName = mapName.Replace("Metalness", "Metallic");
            mapName = mapName.Replace("AmbientOcclusion", "Occlusion");
            return Path.GetFileName(assetName + "_" + mapName);
        }

        private static string GetZipFilePath()
        {
            string path = Application.dataPath;
            int removeIndex = path.LastIndexOf("/Assets");
            path = path.Remove(removeIndex, "/Assets".Length);
            path += "/textures.zip";
            return path;
        }

    }

}
