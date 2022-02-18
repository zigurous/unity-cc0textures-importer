using UnityEngine;

namespace Zigurous.Importer.CC0Textures
{
    internal static class Log
    {
        internal static Object context;

        internal static void Message(string message)
        {
            Debug.Log("[CC0TexturesImporter] " + message, context);
        }

        internal static void Warning(string warning)
        {
            Debug.LogWarning("[CC0TexturesImporter] " + warning, context);
        }

        internal static void Error(string error)
        {
            Debug.LogError("[CC0TexturesImporter] " + error, context);
        }

    }

}
