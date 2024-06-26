using NaughtyAttributes;
using System;
using UnityEngine;

namespace Criaath.OOS
{
#if DOTWEEN_ENABLED
    public class OosController : MonoBehaviour
    {
        [SerializeField] private OosBase _open;
        [SerializeField] private OosBase _close;

        private bool _isOpened;

        [Tooltip("To prevent the object cannot be opened again if it is already open")]
        [Foldout("Settings")][SerializeField] private bool _checkIsOpened;
        [Tooltip("Select which state it is in when in editor mode (before play mode)")]
        [ShowIf("_checkIsOpened")][Foldout("Settings")][SerializeField] private OosState _startState;

        [Tooltip("To move the object immediately in the selected event")]
        [Foldout("Settings")][SerializeField, Space(5)] private bool _moveImmediately;
        [Tooltip("Select which event to play at")]
        [ShowIf("_moveImmediately")][Foldout("Settings")][SerializeField] private PlayOnThis _playOn;
        [Tooltip("Select which state to play")]
        [ShowIf("_moveImmediately")][Foldout("Settings")][SerializeField] private OosState _playThis;
        private Action PlayAnimation;

        #region Built-in

        private void Awake()
        {
            if (_checkIsOpened)
            {
                switch (_startState)
                {
                    case OosState.Open: _isOpened = true; break;
                    case OosState.Close: _isOpened = false; break;
                }
            }
            if (!_moveImmediately) return;
            if (_playThis == OosState.Open) PlayAnimation += OpenImmediately;
            else PlayAnimation += CloseImmediately;

            if (_playOn == PlayOnThis.Awake)
                PlayAnimation?.Invoke();
        }
        private void OnEnable()
        {
            if (_playOn == PlayOnThis.OnEnable)
                PlayAnimation?.Invoke();
        }
        private void Start()
        {
            if (_playOn == PlayOnThis.Start)
                PlayAnimation?.Invoke();
        }
        private void OnDisable()
        {
            if (_playOn == PlayOnThis.OnDisable)
                PlayAnimation?.Invoke();
        }
        private void OnDestroy()
        {
            if (_playOn == PlayOnThis.OnDestroy)
                PlayAnimation?.Invoke();
        }

        #endregion

        #region Public Funcs
        public void Open()
        {
            if (!CanMoveToTarget(true)) return;
            _open.MoveToTarget();
            ChangeIsOpened();
        }
        public void OpenImmediately()
        {
            if (!CanMoveToTarget(true)) return;
            _open.MoveToTargetImmediately();
            ChangeIsOpened();
        }

        public void Close()
        {
            if (!CanMoveToTarget(false)) return;
            _close.MoveToTarget();
            ChangeIsOpened();
        }
        public void CloseImmediately()
        {
            if (!CanMoveToTarget(false)) return;
            _close.MoveToTargetImmediately();
            ChangeIsOpened();
        }
        #endregion

        #region Settings

        private bool CanMoveToTarget(bool isTryingToOpen)
        {
            if (!_checkIsOpened) return true;
            if (isTryingToOpen == _isOpened) return false;
            else return true;
        }

        private void ChangeIsOpened()
        {
            _isOpened = !_isOpened;
        }

        public enum OosState
        {
            Open,
            Close
        }

        #endregion
    }
#endif
}
