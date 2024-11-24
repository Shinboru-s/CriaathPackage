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
        [SerializeField] private bool _logOnCommand = false;

        [BoxGroup("Animations"), SerializeField] AnimationSequencerController _openAnimation, _closeAnimation;
        [Foldout("Events")] public UnityEvent OnOpenStarted, OnOpenEnded, OnCloseStarted, OnCloseEnded;
        private void Reset()
        {
            m_name = gameObject.name;
        }
        public void PreparePage()
        {
            AddLogs();
        }

        void AddLogs()
        {
            if (!_logOnCommand) return;
            OnOpenStarted.AddListener(() => CriaathDebugger.Log($"{Name}", "Open Started"));
            OnOpenEnded.AddListener(() => CriaathDebugger.Log($"{Name}", "Open Ended"));

            OnCloseStarted.AddListener(() => CriaathDebugger.Log($"{Name}", "Close Started"));
            OnCloseEnded.AddListener(() => CriaathDebugger.Log($"{Name}", "Close Ended"));
        }

        public async Task Open(bool playAnimations, bool invokeEvents)
        {
            if (IsOpen == true)
            {
                CriaathDebugger.LogWarning("Criaath UI", $"{Name} named page already opened!");
                return;
            }
            gameObject.SetActive(true);
            if (invokeEvents) OnOpenStarted?.Invoke();
            var tcs = new TaskCompletionSource<bool>();
            _openAnimation.Play(() => tcs.SetResult(true));

            if (playAnimations == false) _openAnimation.Complete();

            await tcs.Task;

            if (invokeEvents) OnOpenEnded?.Invoke();
            IsOpen = true;
        }
        // #if UNITY_EDITOR
        //         [ContextMenu("Run Close Method")]
        //         public void CloseEditor()
        //         {
        //             if (IsOpen == false)
        //             {
        //                 CriaathDebugger.LogWarning("Criaath UI", $"{Name} named page already closed!");
        //                 return;
        //             }
        //             _closeAnimation.Play(() => gameObject.SetActive(false));
        //             IsOpen = false;
        //         }
        // #endif
        public async Task Close(bool playAnimations, bool invokeEvents)
        {
            if (IsOpen == false)
            {
                CriaathDebugger.LogWarning("Criaath UI", $"{Name} named page already closed!");
                return;
            }
            if (invokeEvents) OnCloseStarted?.Invoke();
            var tcs = new TaskCompletionSource<bool>();
            _closeAnimation.Play(() => tcs.SetResult(true));

            if (playAnimations == false) _closeAnimation.Complete();

            await tcs.Task;

            IsOpen = false;
            gameObject.SetActive(false);
            if (invokeEvents) OnCloseEnded?.Invoke();
        }
        public async Task Toggle(bool playAnimations, bool invokeEvents)
        {
            if (IsOpen)
                await Close(playAnimations, invokeEvents);
            else
                await Open(playAnimations, invokeEvents);
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