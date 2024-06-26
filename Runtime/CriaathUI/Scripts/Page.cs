using UnityEngine;
using UnityEngine.Events;
using Criaath.MiniTools;
using NaughtyAttributes;
using BrunoMikoski.AnimationSequencer;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Criaath.UI
{
#if DOTWEEN_ENABLED
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

        public async Task Open(bool playAnimations)
        {
            if (IsOpen == true)
            {
                CriaathDebugger.LogWarning("Criaath UI", $"{Name} named page already opened!");
                return;
            }

            var tcs = new TaskCompletionSource<bool>();
            _openAnimation.Play(() => tcs.SetResult(true));

            if (playAnimations == false) _openAnimation.Complete();

            await tcs.Task;

            OnOpenEnded?.Invoke();
            IsOpen = true;
        }

        public async Task Close(bool playAnimations)
        {
            if (IsOpen == false)
            {
                CriaathDebugger.LogWarning("Criaath UI", $"{Name} named page already closed!");
                return;
            }

            var tcs = new TaskCompletionSource<bool>();
            _closeAnimation.Play(() => tcs.SetResult(true));

            if (playAnimations == false) _closeAnimation.Complete();

            await tcs.Task;

            OnCloseEnded?.Invoke();
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
#endif
}