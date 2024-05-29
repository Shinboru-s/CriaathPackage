using UnityEngine;

namespace Criaath.MiniTools
{
    public static class CriaathDebugger
    {
        #region Log
        public static void Log(object message) => Log(message, Color.white);
        public static void Log(object message, Color color)
        {
            if (!ShouldLog()) return;
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{message}</color>");
        }

        public static void Log(object header, object message) => Log(header, Color.green, message, Color.white);
        public static void Log(object header, Color headerColor, object message) => Log(header, headerColor, message, Color.white);
        public static void Log(object header, Color headerColor, object message, Color messageColor)
        {
            if (!ShouldLog()) return;
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(headerColor)}>{header}</color> \n<color=#{ColorUtility.ToHtmlStringRGBA(messageColor)}>{message}</color>");
        }
        #endregion
        #region LogWarning
        public static void LogWarning(object message) => LogWarning(message, Color.white);
        public static void LogWarning(object message, Color color)
        {
            if (!ShouldLog()) return;
            Debug.LogWarning($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{message}</color>");
        }
        public static void LogWarning(object header, object message) => LogWarning(header, Color.green, message, Color.white);
        public static void LogWarning(object header, Color headerColor, object message) => LogWarning(header, headerColor, message, Color.white);
        public static void LogWarning(object header, Color headerColor, object message, Color messageColor)
        {
            if (!ShouldLog()) return;
            Debug.LogWarning($"<color=#{ColorUtility.ToHtmlStringRGBA(headerColor)}>{header}</color> \n<color=#{ColorUtility.ToHtmlStringRGBA(messageColor)}>{message}</color>");
        }
        #endregion
        #region LogError
        public static void LogError(object message) => LogError(message, Color.white);
        public static void LogError(object message, Color color)
        {
            if (!ShouldLog()) return;
            Debug.LogError($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{message}</color>");
        }
        public static void LogError(object header, object message) => LogError(header, Color.green, message, Color.white);
        public static void LogError(object header, Color headerColor, object message) => LogError(header, headerColor, message, Color.white);
        public static void LogError(object header, Color headerColor, object message, Color messageColor)
        {
            if (!ShouldLog()) return;
            Debug.LogError($"<color=#{ColorUtility.ToHtmlStringRGBA(headerColor)}>{header}</color> \n<color=#{ColorUtility.ToHtmlStringRGBA(messageColor)}>{message}</color>");
        }
        #endregion

        private static bool ShouldLog() { return Debug.isDebugBuild; }
    }
}
