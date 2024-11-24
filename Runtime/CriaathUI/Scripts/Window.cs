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
        [SerializeField, BoxGroup("Initialize")] bool _initializeOnAwake = true;
        [SerializeField, BoxGroup("Initialize")] bool _invokeEventsOnInitialize = true;
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
        private async void Awake()
        {
            OnCommandStarted.AddListener(() => _graphicRaycaster.enabled = false);
            OnCommandEnded.AddListener(() => _graphicRaycaster.enabled = true);
            PreparePages();
            if (_initializeOnAwake)
                await Initialize();
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
            // _openedPages = _pages.ToList();
            //CheckInitializePages();
        }
        public async Task Initialize()
        {
            await CloseAll(false, _invokeEventsOnInitialize);
            foreach (string pageName in _initializePages)
            {
                await Open(pageName, _playAnimOnInitializeOpen, false, _invokeEventsOnInitialize);
            }
        }
        public async Task Open(string pageName, bool playAnimations, bool waitForCollaborativePages, bool invokeEvents)
        {
            OnCommandStarted?.Invoke();
            Page page = GetPage(pageName);


            if (waitForCollaborativePages)
            {
                await CloseNonCollaborativePages(page, invokeEvents);
                await page.Open(playAnimations, invokeEvents);
            }
            else
            {
                Task pageOpenTask = page.Open(playAnimations, invokeEvents);
                Task closeNonCollabTask = CloseNonCollaborativePages(page, invokeEvents);
                await Task.WhenAll(pageOpenTask, closeNonCollabTask);
            }

            _openedPages.Add(page);
            OnCommandEnded?.Invoke();
        }
        public async Task OpenAll(bool playAnimations, bool invokeEvents)
        {
            OnCommandStarted?.Invoke();
            List<Task> pageTasks = new();

            foreach (var page in _pages)
            {
                if (page.IsOpen) continue;

                pageTasks.Add(page.Open(playAnimations, invokeEvents));
                _openedPages.Add(page);
            }

            await Task.WhenAll(pageTasks);
            OnCommandEnded?.Invoke();
        }
        public async Task Close(string pageName, bool playAnimations, bool invokeEvents)
        {
            OnCommandStarted?.Invoke();
            Page page = GetPage(pageName);
            await page.Close(playAnimations, invokeEvents);
            _openedPages.Remove(page);
            OnCommandEnded?.Invoke();
        }
        public async Task CloseAll(bool playAnimations, bool invokeEvents)
        {
            OnCommandStarted?.Invoke();
            List<Task> pageTasks = new();

            foreach (var page in _pages)
            {
                if (page.IsOpen is not true) continue;

                pageTasks.Add(page.Close(playAnimations, invokeEvents));
                _openedPages.Remove(page);
            }

            await Task.WhenAll(pageTasks);
            OnCommandEnded?.Invoke();
        }
        public async Task Toggle(string pageName, bool playAnimations, bool waitForCollaborativePages, bool invokeEvents)
        {
            if (GetPage(pageName).IsOpen)
                await Close(pageName, playAnimations, invokeEvents);
            else
                await Open(pageName, playAnimations, waitForCollaborativePages, invokeEvents);
        }
        private async Task CloseNonCollaborativePages(Page pageToOpen, bool invokeEvents)
        {
            List<Task> closeTasks = new List<Task>();

            Page[] tempOpenedPages = _openedPages.ToArray();
            foreach (var page in tempOpenedPages)
            {
                if (page.IsCollaborative(pageToOpen) is not true)
                {
                    closeTasks.Add(page.Close(true, invokeEvents));
                    _openedPages.Remove(page);
                }
            }

            await Task.WhenAll(closeTasks);
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

        // #if UNITY_EDITOR
        //         [ContextMenu("Run Close Method")]
        //         private void RunCloseInEditor()
        //         {
        //             Task.Run(() => CheckInitializePages()).ConfigureAwait(false);
        //         }
        //         private async void CheckInitializePages()
        //         {
        //             _openedPages = new();
        //             foreach (Page page in _openedPages)
        //             {
        //                 if (IsInitializePage(page))
        //                 {
        //                     if (page.IsOpen) continue;

        //                     await page.Open(false);
        //                     _openedPages.Add(page);
        //                 }
        //                 else
        //                 {
        //                     if (!page.IsOpen) continue;

        //                     await page.Close(false);
        //                     _openedPages.Remove(page);
        //                 }
        //             }
        //         }
        //         private bool IsInitializePage(Page page)
        //         {
        //             return _initializePages.Contains(page.name);
        //         }
        // #endif

    }

    [System.Serializable]
    public class CollaborativeGroup
    {
        [DropdownOption("_pageNames")]
        public string[] CollaborativePages;
    }
#endif
}