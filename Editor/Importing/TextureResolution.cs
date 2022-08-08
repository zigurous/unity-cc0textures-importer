namespace Zigurous.AssetDownloader
{
    public enum TextureResolution
    {
        _1K,
        _2K,
        _4K,
        _8K,
        _16K,
    }

    public static class TextureResolutionExtensions
    {
        public static string GetName(this TextureResolution resolution)
        {
            switch (resolution)
            {
                case TextureResolution._1K:
                    return "1K";
                case TextureResolution._2K:
                    return "2K";
                case TextureResolution._4K:
                    return "4K";
                case TextureResolution._8K:
                    return "8K";
                case TextureResolution._16K:
                    return "16K";
                default:
                    return "2K";
            }
        }

        public static int GetMaxTextureSize(this TextureResolution resolution)
        {
            switch (resolution)
            {
                case TextureResolution._1K:
                    return 1024;
                case TextureResolution._2K:
                    return 2048;
                case TextureResolution._4K:
                    return 4096;
                case TextureResolution._8K:
                    return 8192;
                case TextureResolution._16K:
                    return 16384;
                default:
                    return 2048;
            }
        }

    }

}
