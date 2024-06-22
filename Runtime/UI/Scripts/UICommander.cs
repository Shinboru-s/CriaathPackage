using UnityEngine;
using Criaath.MiniTools;
using NaughtyAttributes;
using UnityEngine.Events;

namespace Criaath.UI
{
    [DefaultExecutionOrder(50)]
    public class UICommander : MonoBehaviour
    {
        [Space(5)]
        [SerializeField][BoxGroup("Command Settings")] PlayOnThis _autoCommand;
        [SerializeField][BoxGroup("Command Settings")] CommandType _commandType;
        [SerializeField][BoxGroup("Command Settings")] bool _playAnimations = true;
        [SerializeField][BoxGroup("Command Settings")] bool _waitForCollaborativePages = true;
        [Space(5)]
        [SerializeField][BoxGroup("Command")][Dropdown("_windowNames")] string _window;
        [SerializeField][BoxGroup("Command")][HideIf("IsPageSelectable")][Dropdown("_pageNames")] string _page;

        [Foldout("Events")] public UnityEvent OnCommandStarted, OnCommandEnded;


        #region Prepare Dropdown Options
        private string[] _windowNames => GetWindowNames();
        private string[] GetWindowNames()
        {
            UIManager uIManager = FindObjectOfType<UIManager>();
            if (uIManager == null)
            {
                CriaathDebugger.LogError("UIManager missing!", "You need to add a UI Manager to use Criaath UI components.");
                return null;
            }
            if (uIManager != null) return uIManager.WindowNames;
            return new string[] { "None" };
        }

        private string[] _pageNames => GetPageNames();
        private string[] GetPageNames()
        {
            UIManager uIManager = FindObjectOfType<UIManager>();
            return uIManager.GetWindow(_window).PageNames;
        }
        private bool IsPageSelectable() { return _commandType is CommandType.CloseAll or CommandType.OpenAll; }
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
            UIManager.Instance.GiveCommand(_commandType, _window, _page, _playAnimations, _waitForCollaborativePages);
        }
    }

    public enum CommandType
    {
        Open,
        OpenAll,
        Close,
        CloseAll
    }
}