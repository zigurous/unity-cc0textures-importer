namespace Zigurous.AssetDownloader
{
    public enum ImageFormat
    {
        JPG,
        PNG,
    }

    public static class ImageFormatExtensions
    {
        public static string GetName(this ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.JPG:
                    return "JPG";
                case ImageFormat.PNG:
                    return "PNG";
                default:
                    return "JPG";
            }
        }

    }

}
