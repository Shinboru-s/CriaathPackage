using UnityEngine;
using System.Linq;
using Criaath.MiniTools;
using NaughtyAttributes;

namespace Criaath.UI
{
    public class UIManager : CriaathSingleton<UIManager>
    {
        [SerializeField] Window[] _windows;

        [HideInInspector] public string[] WindowNames;

        #region Prepare Names
        private void OnValidate()
        {
            WindowNames = GetPageNames();
        }

        private string[] GetPageNames()
        {
            if (_windows == null || _windows.Length == 0)
            {
                return new string[] { "Missing Window!" };
            }

            string[] windowNames = new string[_windows.Length];
            for (int i = 0; i < _windows.Length; i++)
            {
                windowNames[i] = _windows[i] != null ? _windows[i].Name : "Missing Window!";
            }
            return windowNames;
        }

        #endregion
        public void GiveCommand(CommandType commandType, string window, string pageName, bool playAnimations)
        {
            Window selectedWindow = GetWindow(window);

            switch (commandType)
            {
                case CommandType.Open:
                    selectedWindow.Open(pageName, playAnimations); break;
                case CommandType.OpenAll:
                    selectedWindow.OpenAll(playAnimations); break;
                case CommandType.Close:
                    selectedWindow.Close(pageName, playAnimations); break;
                case CommandType.CloseAll:
                    selectedWindow.CloseAll(playAnimations); break;
            }
        }

        public Window GetWindow(string name)
        {
            foreach (var window in _windows)
            {
                if (window.Name == name)
                    return window;
            }
            CriaathDebugger.LogError("Criaath UIManager", Color.magenta, $"{name} named window cannot found!");
            return null;
        }

    }
}