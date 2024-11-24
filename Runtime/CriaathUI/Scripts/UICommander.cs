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
        [SerializeField][BoxGroup("Command Settings")] bool _invokeEvents = true;
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
            GiveCommand(_commandType, _window, _page, _playAnimations, _waitForCollaborativePages, _invokeEvents);
        }
        private async void GiveCommand(CommandType commandType, Window window, string pageName, bool playAnimations, bool waitForCollaborativePages, bool invokeEvents)
        {
            OnCommandStarted?.Invoke();
            switch (commandType)
            {
                case CommandType.Open:
                    await window.Open(pageName, playAnimations, waitForCollaborativePages, invokeEvents); break;
                case CommandType.OpenAll:
                    await window.OpenAll(playAnimations, invokeEvents); break;
                case CommandType.Close:
                    await window.Close(pageName, playAnimations, invokeEvents); break;
                case CommandType.CloseAll:
                    await window.CloseAll(playAnimations, invokeEvents); break;
                case CommandType.Toggle:
                    await window.Toggle(pageName, playAnimations, waitForCollaborativePages, invokeEvents); break;
            }
            OnCommandEnded?.Invoke();
        }
    }
#endif

    public enum CommandType
    {
        Open, OpenAll, Close, CloseAll, Toggle
    }
}