using UnityEngine;
using Criaath.MiniTools;
using NaughtyAttributes;
using UnityEngine.Events;

namespace Criaath.UI
{
#if DOTWEEN_ENABLED
    [DefaultExecutionOrder(50)]
    public class UICommander : MonoBehaviour
    {
        [Space(5)]
        [SerializeField][BoxGroup("Command Settings")] PlayOnThis _autoCommand;
        [SerializeField][BoxGroup("Command Settings")] CommandType _commandType;
        [SerializeField][BoxGroup("Command Settings")] bool _playAnimations = true;
        [SerializeField][BoxGroup("Command Settings")] bool _waitForCollaborativePages = true;

        [Space(5)]
        [SerializeField][BoxGroup("Command")] Window _window;
        [SerializeField][BoxGroup("Command")][HideIf("IsPageSelectable")][Dropdown("_pageNames")] string _page;

        [Foldout("Events")] public UnityEvent OnCommandStarted, OnCommandEnded;


        #region Prepare Dropdown Options

        private string[] _pageNames => GetPageNames();
        private string[] GetPageNames()
        {
            if (_window == null)
                return new string[] { "Missing Window!" };
            return _window.GetPageNames();
        }
        private bool IsPageSelectable() { return _commandType is CommandType.CloseAll or CommandType.OpenAll || _window == null; }
        #endregion

        #region Auto Command
        void Awake()
        {
            if (_autoCommand == PlayOnThis.Awake) GiveCommand();
        }
        void Start()
        {
            if (_autoCommand == PlayOnThis.Start) GiveCommand();
        }
        void OnEnable()
        {
            if (_autoCommand == PlayOnThis.OnEnable) GiveCommand();
        }
        void OnDisable()
        {
            if (_autoCommand == PlayOnThis.OnDisable) GiveCommand();
        }
        void OnDestroy()
        {
            if (_autoCommand == PlayOnThis.OnDestroy) GiveCommand();
        }
        #endregion

        [ContextMenu("Give Command")]
        public void GiveCommand()
        {
            GiveCommand(_commandType, _window, _page, _playAnimations, _waitForCollaborativePages);
        }
        private void GiveCommand(CommandType commandType, Window window, string pageName, bool playAnimations, bool waitForCollaborativePages)
        {
            switch (commandType)
            {
                case CommandType.Open:
                    window.Open(pageName, playAnimations, waitForCollaborativePages); break;
                case CommandType.OpenAll:
                    window.OpenAll(playAnimations); break;
                case CommandType.Close:
                    window.Close(pageName, playAnimations); break;
                case CommandType.CloseAll:
                    window.CloseAll(playAnimations); break;
                case CommandType.Toggle:
                    window.Toggle(pageName, playAnimations, waitForCollaborativePages); break;
            }
        }
    }
#endif

    public enum CommandType
    {
        Open, OpenAll, Close, CloseAll, Toggle
    }
}