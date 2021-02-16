using UnityEngine;

namespace Zigurous.Importer.CC0Textures
{
    internal static class Log
    {
        public static Object context;

        public static void Message(string message)
        {
            Debug.Log("[CC0TexturesImporter]: " + message, context);
        }

        public static void Warning(string warning)
        {
            Debug.LogWarning("[CC0TexturesImporter]: " + warning, context);
        }

        public static void Error(string error)
        {
            Debug.LogError("[CC0TexturesImporter]: " + error, context);
        }

    }

}
