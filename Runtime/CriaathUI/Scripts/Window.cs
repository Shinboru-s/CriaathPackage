using UnityEngine;
using UnityEngine.UI;
using Criaath.MiniTools;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using Criaath.Extensions;
using UnityEngine.Events;
using System.Threading.Tasks;

namespace Criaath.UI
{
#if DOTWEEN_ENABLED
    [DefaultExecutionOrder(-20)]
    public class Window : MonoBehaviour
    {
        [ValidateInput("IsNameNullOrEmpty", "Window name cannot be empty!")]
        [SerializeField, BoxGroup("Settings")] string m_name;
        public string Name
        {
            private set => m_name = value;
            get => m_name;
        }
        bool IsNameNullOrEmpty(string name) { return !string.IsNullOrEmpty(name); }
        [SerializeField, BoxGroup("Settings")] GraphicRaycaster _graphicRaycaster;
        [SerializeField, BoxGroup("Settings")] Page[] _pages;
        [SerializeField, BoxGroup("Settings")] CollaborativeGroup[] CollaborativePageGroups;

        [Space(5)]
        [SerializeField, BoxGroup("Initialize")] bool _playAnimOnInitializeOpen = true;
        [SerializeField, BoxGroup("Initialize")][DropdownOption("_pageNames")] string[] _initializePages;

        [Space(5)]
        [SerializeField, ReadOnly, BoxGroup("Dev")] private List<Page> _openedPages = new();


        [Foldout("Events")] public UnityEvent OnCommandStarted, OnCommandEnded;


        #region Prepare Page Names
        private string[] _pageNames;
        public string[] GetPageNames()
        {
            if (_pages == null || _pages.Length == 0)
                return new string[] { "Missing Page!" };

            string[] pageNames = new string[_pages.Length];
            for (int i = 0; i < _pages.Length; i++)
            {
                pageNames[i] = _pages[i] != null ? _pages[i].Name : "Missing Page!";
            }
            return pageNames;
        }
        #endregion

        #region Commands
        private void Awake()
        {
            OnCommandStarted.AddListener(() => _graphicRaycaster.enabled = false);
            OnCommandEnded.AddListener(() => _graphicRaycaster.enabled = true);
            PreparePages();
            Initialize();
        }
        private void PreparePages()
        {
            foreach (Page page in _pages)
            {
                page.PreparePage();
            }
        }
        private void Reset()
        {
            m_name = gameObject.name;
            _graphicRaycaster = GetComponent<GraphicRaycaster>();
        }
        private void OnValidate()
        {
            _pageNames = GetPageNames();
            SetCollaborativeGroupIds();
            _openedPages = _pages.ToList();
        }
        private async void Initialize()
        {
            CloseAll(false);
            await Task.Delay(1000);
            foreach (string pageName in _initializePages)
            {
                Open(pageName, _playAnimOnInitializeOpen, false);
            }
        }
        public async void Open(string pageName, bool playAnimations, bool waitForCollaborativePages)
        {
            OnCommandStarted?.Invoke();
            Page page = GetPage(pageName);


            if (waitForCollaborativePages)
            {
                await CloseNonCollaborativePages(page);
                await page.Open(playAnimations);
            }
            else
            {
                Task pageOpenTask = page.Open(playAnimations);
                Task closeNonCollabTask = CloseNonCollaborativePages(page);
                await Task.WhenAll(pageOpenTask, closeNonCollabTask);
            }

            _openedPages.Add(page);
            OnCommandEnded?.Invoke();
        }
        public async void OpenAll(bool playAnimations)
        {
            OnCommandStarted?.Invoke();
            List<Task> pageTasks = new();

            foreach (var page in _pages)
            {
                if (page.IsOpen) continue;

                pageTasks.Add(page.Open(playAnimations));
                _openedPages.Add(page);
            }

            await Task.WhenAll(pageTasks);
            OnCommandEnded?.Invoke();
        }
        public async void Close(string pageName, bool playAnimations)
        {
            OnCommandStarted?.Invoke();
            Page page = GetPage(pageName);
            await page.Close(playAnimations);
            _openedPages.Remove(page);
            OnCommandEnded?.Invoke();
        }
        public async void CloseAll(bool playAnimations)
        {
            OnCommandStarted?.Invoke();
            List<Task> pageTasks = new();

            foreach (var page in _pages)
            {
                if (page.IsOpen is not true) continue;

                pageTasks.Add(page.Close(playAnimations));
                _openedPages.Remove(page);
            }

            await Task.WhenAll(pageTasks);
            OnCommandEnded?.Invoke();
        }
        #endregion

        public Page GetPage(string pageName)
        {
            if (pageName == null) return null;
            foreach (var page in _pages)
            {
                if (page.Name == pageName)
                    return page;
            }
            CriaathDebugger.LogError("Criaath UI Window", Color.magenta, $"{pageName} named page cannot found in {Name} window!");
            return null;
        }

        private void SetCollaborativeGroupIds()
        {
            if (_pages.IsNullOrEmpty()) return;

            foreach (var page in _pages)
            {
                if (page == null) continue;
                page.CollaborativeGroupIds.Clear();
            }

            for (int i = 0; i < CollaborativePageGroups.Length; i++)
            {
                foreach (var pageName in CollaborativePageGroups[i].CollaborativePages)
                {
                    if (string.IsNullOrEmpty(pageName)) continue;

                    Page page = GetPage(pageName);
                    if (page.CollaborativeGroupIds.Contains(i)) continue;
                    CriaathDebugger.Log("Criaath UI", $"{pageName} added to {i} group.");
                    page.CollaborativeGroupIds.Add(i);
                }
            }
        }
        private async Task CloseNonCollaborativePages(Page pageToOpen)
        {
            List<Task> closeTasks = new List<Task>();

            Page[] tempOpenedPages = _openedPages.ToArray();
            foreach (var page in tempOpenedPages)
            {
                if (page.IsCollaborative(pageToOpen) is not true)
                {
                    closeTasks.Add(page.Close(true));
                    _openedPages.Remove(page);
                }
            }

            await Task.WhenAll(closeTasks);
        }
    }

    [System.Serializable]
    public class CollaborativeGroup
    {
        [DropdownOption("_pageNames")]
        public string[] CollaborativePages;
    }
#endif
}