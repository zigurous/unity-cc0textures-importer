namespace Zigurous.Importer.CC0Textures
{
    public enum TextureMapType
    {
        None,
        Albedo,
        Normal,
        Displacement,
        Roughness,
        Metallic,
        Occlusion,
        Emission,
    }

    public static class TextureMapTypeExtensions
    {
        public static TextureMapType ToTextureMapType(this string fileName)
        {
            if (fileName.Contains("Albedo")) {
                return TextureMapType.Albedo;
            } else if (fileName.Contains("Normal")) {
                return TextureMapType.Normal;
            } else if (fileName.Contains("Displacement")) {
                return TextureMapType.Displacement;
            } else if (fileName.Contains("Roughness")) {
                return TextureMapType.Roughness;
            } else if (fileName.Contains("Metallic") || fileName.Contains("Metalness")) {
                return TextureMapType.Metallic;
            } else if (fileName.Contains("Occlusion")) {
                return TextureMapType.Occlusion;
            } else if (fileName.Contains("Emission")) {
                return TextureMapType.Emission;
            } else if (fileName.Contains("Color")) {
                return TextureMapType.Albedo;
            } else {
                return TextureMapType.None;
            }
        }

        public static string ToTextureName(this TextureMapType mapType)
        {
            switch (mapType)
            {
                case TextureMapType.Albedo:
                    return "_MainTex";
                case TextureMapType.Normal:
                    return "_BumpMap";
                case TextureMapType.Displacement:
                    return "_ParallaxMap";
                case TextureMapType.Roughness:
                    return "_SpecGlossMap";
                case TextureMapType.Metallic:
                    return "_MetallicGlossMap";
                case TextureMapType.Occlusion:
                    return "_OcclusionMap";
                case TextureMapType.Emission:
                    return "_EmissionMap";
                default:
                    return null;
            }
        }

    }

}
