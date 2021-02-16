namespace Zigurous.Importer.CC0Textures
{
    public enum TextureResolution
    {
        _1K,
        _2K,
        _4K,
        _8K,
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
                default:
                    return "2K";
            }
        }

    }

}
