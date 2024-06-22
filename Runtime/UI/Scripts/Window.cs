using UnityEngine;
using UnityEngine.UI;
using Criaath.MiniTools;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using Criaath.Extensions;
using UnityEngine.Events;

namespace Criaath.UI
{
    [DefaultExecutionOrder(-20)]
    public class Window : MonoBehaviour
    {
        [ValidateInput("IsNameNullOrEmpty", "Window name cannot be empty!")]
        [SerializeField] string m_name;
        [SerializeField] GraphicRaycaster _graphicRaycaster; // TODO: use _graphicRaycaster
        [SerializeField] Page[] _pages;
        [SerializeField] CollaborativeGroup[] CollaborativePageGroups;

        [SerializeField, ReadOnly] private List<Page> _openedPages = new();

        [Foldout("Events")] public UnityEvent OnCommandStarted, OnCommandEnded;

        private int _pagesInProcessCount = 0;
        private bool _allProcessStated = false;

        public string Name
        {
            private set => m_name = value;
            get => m_name;
        }

        #region Prepare Page Names
        bool IsNameNullOrEmpty(string name) { return !string.IsNullOrEmpty(name); }

        [HideInInspector] public string[] PageNames;

        private string[] GetPageNames()
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
            foreach (var page in _pages)
            {
                page.OnOpenStarted.AddListener(() => ProcessPage(+1));
                page.OnOpenEnded.AddListener(() => ProcessPage(-1));
                page.OnCloseStarted.AddListener(() => ProcessPage(+1));
                page.OnCloseEnded.AddListener(() => ProcessPage(-1));
            }
        }
        public void ProcessPage(int state)
        {
            return; //! its disabled
            _pagesInProcessCount += state;
            if (_pagesInProcessCount == 0 && _allProcessStated)
            {
                OnCommandEnded?.Invoke();
                _allProcessStated = false;
            }
        }
        private void OnValidate()
        {
            PageNames = GetPageNames();
            SetCollaborativeGroupIds();
            _openedPages = _pages.ToList();
        }
        public void Open(string pageName, bool playAnimations)
        {
            OnCommandStarted?.Invoke();
            Page page = GetPage(pageName);
            page.Open(playAnimations);
            CloseNonCollaborativePages(page);
            _openedPages.Add(page);
        }
        public void OpenAll(bool playAnimations)
        {
            OnCommandStarted?.Invoke();

            foreach (var page in _pages)
            {
                if (page.IsOpen) continue;

                page.Open(playAnimations);
                _openedPages.Add(page);
            }
            _allProcessStated = true;
        }
        public void Close(string pageName, bool playAnimations)
        {
            OnCommandStarted?.Invoke();
            Page page = GetPage(pageName);
            page.Close(playAnimations);
            _openedPages.Remove(page);
        }
        public void CloseAll(bool playAnimations)
        {
            OnCommandStarted?.Invoke();

            foreach (var page in _pages)
            {
                if (page.IsOpen is not true) continue;

                page.Close(playAnimations);
                _openedPages.Remove(page);
            }
            _allProcessStated = true;
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
        private void CloseNonCollaborativePages(Page pageToOpen)
        {
            foreach (var page in _openedPages)
            {
                if (page.IsCollaborative(pageToOpen) is not true)
                    page.Close(true);
            }
        }
    }

    [System.Serializable]
    public class CollaborativeGroup
    {
        [DropdownOption("PageNames")]
        public string[] CollaborativePages;
    }

}