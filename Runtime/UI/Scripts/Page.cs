using UnityEngine;
using UnityEngine.Events;
using Criaath.MiniTools;
using NaughtyAttributes;
using BrunoMikoski.AnimationSequencer;
using System.Collections.Generic;
using System;

namespace Criaath.UI
{
    [DefaultExecutionOrder(10)]
    public class Page : MonoBehaviour
    {
        [ValidateInput("IsNameNullOrEmpty", "Page name cannot be empty!")]
        [SerializeField] private string m_name;
        public string Name
        {
            private set => m_name = value;
            get => m_name;
        }
        bool IsNameNullOrEmpty(string name) { return !string.IsNullOrEmpty(name); }

        public bool IsOpen { get; private set; } = true;

        [HideInInspector] public List<int> CollaborativeGroupIds = new();

        [BoxGroup("Animations"), SerializeField] AnimationSequencerController _openAnimation, _closeAnimation;
        [Foldout("Events")] public UnityEvent OnOpenStarted, OnOpenEnded, OnCloseStarted, OnCloseEnded;

        void Awake()
        {
            AnimationTest();

            PrepareEvents();
        }
        void PrepareEvents()
        {
            // Prepare Open animation events
            _openAnimation.OnStartEvent.AddListener(() => OnOpenStarted?.Invoke());
            //_openAnimation.OnFinishedEvent.AddListener(() => OnOpenEnded?.Invoke());
            // Prepare Close animation events
            _closeAnimation.OnStartEvent.AddListener(() => OnCloseStarted?.Invoke());
            //_closeAnimation.OnFinishedEvent.AddListener(() => OnCloseEnded?.Invoke());

            // Prepare page' active state
            OnOpenStarted.AddListener(() => gameObject.SetActive(true));
            OnCloseEnded.AddListener(() => gameObject.SetActive(false));
        }

        void AnimationTest()
        {
            OnOpenStarted.AddListener(() => CriaathDebugger.Log($"{Name}", "Open Started"));
            OnOpenEnded.AddListener(() => CriaathDebugger.Log($"{Name}", "Open Ended"));

            OnCloseStarted.AddListener(() => CriaathDebugger.Log($"{Name}", "Close Started"));
            OnCloseEnded.AddListener(() => CriaathDebugger.Log($"{Name}", "Close Ended"));
        }

        public void Open(bool playAnimations, UnityEvent callback = null)
        {
            if (IsOpen == true)
            {
                CriaathDebugger.LogWarning("Criaath UI", $"{Name} named page already opened!");
                return;
            }

            _openAnimation.Play(() => { OnOpenEnded?.Invoke(); callback?.Invoke(); });
            if (playAnimations == false) _openAnimation.Complete();

            IsOpen = true;
        }
        public void Close(bool playAnimations, UnityEvent callback = null)
        {
            if (IsOpen == false)
            {
                CriaathDebugger.LogWarning("Criaath UI", $"{Name} named page already closed!");
                return;
            }

            _closeAnimation.Play(() => { OnCloseEnded?.Invoke(); callback?.Invoke(); });
            if (playAnimations == false) _closeAnimation.Complete();

            IsOpen = false;
        }

        public bool IsCollaborative(Page page)
        {
            foreach (var groupId in page.CollaborativeGroupIds)
            {
                if (CollaborativeGroupIds.Contains(groupId))
                    return true;
            }

            return false;
        }
    }
}