using UnityEngine;

namespace Zigurous.AssetDownloader
{
    public sealed class ImportContext
    {
        public string assetId = "";
        public string outputName = "";
        public string outputPath = "";

        public TextureResolution resolution = TextureResolution._2K;
        public ImageFormat format = ImageFormat.JPG;
        public Material materialPreset;

        public bool HasAssetId()
        {
            return assetId != null && assetId.Length > 0;
        }

        public string GetOutputName(string fileName)
        {
            if (outputName != null && outputName.Length > 0) {
                return fileName.Replace(assetId, outputName);
            } else {
                return fileName;
            }
        }

        public string GetFilePath(string assetName)
        {
            string outputPath = GetOutputPath();
            string filePath = $"{Application.dataPath}/{outputPath}/{assetName}";
            return filePath.Replace("//", "/");
        }

        public string GetAssetPath(string assetName)
        {
            string outputPath = GetOutputPath();
            string assetPath = $"Assets/{outputPath}/{assetName}";
            return assetPath.Replace("//", "/");
        }

        private string GetOutputPath()
        {
            string path = outputPath != null ? outputPath : "";

            if (path.StartsWith("/")) {
                path = path.Remove(0, 1);
            }

            if (path.StartsWith("Assets/")) {
                path = path.Remove(0, 7);
            }

            if (path.EndsWith("/")) {
                path = path.Remove(path.Length - 1);
            }

            return path;
        }

    }

}
