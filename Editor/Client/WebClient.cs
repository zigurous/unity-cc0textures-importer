using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Zigurous.AssetDownloader
{
    public static class WebClient
    {
        private static readonly System.Net.WebClient client;
        private const string zipFileName = "download.zip";

        static WebClient()
        {
            client = new System.Net.WebClient();
            client.Headers.Add("Accept-Language", "en-US");
            client.Headers.Add("Content-Type", "application/zip");
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.67 Safari/537.36");
        }

        public static async void Download(AssetSource source, ImportContext context)
        {
            try
            {
                string url = source.GetDownloadUrl(context);
                Log.Message("Downloading: " + url);

                await client.DownloadFileTaskAsync(url, zipFileName);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return;
            }

            List<string> files = new List<string>();
            string zipPath = GetZipPath();

            FileStream fs = new FileStream(zipPath, FileMode.Open);
            ZipFile zipFile = new ZipFile(fs);

            Log.Message("Importing files from " + zipPath);

            if (zipFile.TestArchive(true)) {
                Unzip(zipFile, source, context, ref files);
            } else {
                Log.Error("Zip file failed integrity check");
            }

            zipFile.IsStreamOwner = false;
            zipFile.Close();
            fs.Close();

            System.IO.File.Delete(zipPath);

            if (files.Count > 0) {
                source.ImportComplete(context, files);
            }
        }

        private static void Unzip(ZipFile zip, AssetSource source, ImportContext context, ref List<string> files)
        {
            try
            {
                foreach (ZipEntry entry in zip)
                {
                    string entryName = entry.Name;

                    if (!entry.IsFile || !source.Filter(entryName)) {
                        continue;
                    }

                    Log.Message("Unpacking zip file entry: " + entryName);

                    entryName = source.Rename(entryName);
                    entryName = context.GetOutputName(entryName);

                    string filePath = context.GetFilePath(entryName);
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
                    fileInfo.Directory.Create();

                    byte[] buffer = new byte[4096];
                    Stream stream = zip.GetInputStream(entry);

                    using (FileStream streamWriter = File.Create(filePath)) {
                        StreamUtils.Copy(stream, streamWriter, buffer);
                    }

                    files.Add(entryName);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        private static string GetZipPath()
        {
            string path = Application.dataPath;
            path = path.Remove(path.Length - "/Assets".Length);
            return $"{path}/{zipFileName}";
        }

    }

}
