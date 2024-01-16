using UnityEngine;

namespace Criaath.MiniTools
{
    public static class CriaathDebugger
    {
        public static void Log(string message, Color color)
        {
            if (!ShouldLog()) return;
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{message}</color>");
        }

        public static void Log(string header, Color headerColor, string message, Color messageColor)
        {
            if (!ShouldLog()) return;
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(headerColor)}>{header}</color>: <color=#{ColorUtility.ToHtmlStringRGBA(messageColor)}>{message}</color>");
        }

        public static void LogWarning(string message, Color color)
        {
            if (!ShouldLog()) return;
            Debug.LogWarning($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{message}</color>");
        }
        public static void LogWarning(string header, Color headerColor, string message, Color messageColor)
        {
            if (!ShouldLog()) return;
            Debug.LogWarning($"<color=#{ColorUtility.ToHtmlStringRGBA(headerColor)}>{header}</color>: <color=#{ColorUtility.ToHtmlStringRGBA(messageColor)}>{message}</color>");
        }
        public static void LogError(string message, Color color)
        {
            if (!ShouldLog()) return;
            Debug.LogError($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{message}</color>");
        }
        public static void LogError(string header, Color headerColor, string message, Color messageColor)
        {
            if (!ShouldLog()) return;
            Debug.LogError($"<color=#{ColorUtility.ToHtmlStringRGBA(headerColor)}>{header}</color>: <color=#{ColorUtility.ToHtmlStringRGBA(messageColor)}>{message}</color>");
        }


        private static bool ShouldLog()
        {
            return Debug.isDebugBuild;
        }
    }
}
