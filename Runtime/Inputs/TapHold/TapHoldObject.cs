using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Criaath.Inputs
{
    [RequireComponent(typeof(Collider2D))]
    public class TapHoldObject : MonoBehaviour
    {
        [SerializeField] private Collider2D _collider;

        [Foldout("Settings"), SerializeField] private InputType _inputType;
        [Foldout("Settings"), SerializeField] private bool _checkPositionOnRelease;

        #region Tap Variables
        [Foldout("Events")][ShowIf("_inputType", InputType.Tap)] public UnityEvent OnTapBegin, OnTapCompleted;
        #endregion

        #region Hold Variables
        [Foldout("Events")][ShowIf("_inputType", InputType.Hold)] public UnityEvent OnHoldBegin, OnHolding, OnHoldCompleted;

        [Foldout("Settings")][ShowIf("_inputType", InputType.Hold)][SerializeField] private bool _checkHoldDuration = false;
        [Foldout("Settings")][ShowIf("ShowHoldDuration")][SerializeField] private float _holdDuration = 1.0f;
        private float _holdDurationCounter;
        private bool _isHolding = false;
        private bool _isHoldEnded = false;
        #endregion

        [Foldout("Events")] public UnityEvent OnInteractionFail;

        bool ShowHoldDuration() { return _checkHoldDuration && (_inputType == InputType.Hold); }

        #region Built-in
        void Start()
        {
            if (_inputType == InputType.Hold)
            {
                OnHoldBegin.AddListener(() => ToggleIsHolding(true));
                OnHoldCompleted.AddListener(() => ToggleIsHolding(false));
                OnInteractionFail.AddListener(() => ToggleIsHolding(false));
            }
        }
        void OnEnable() => TapHoldManager.Instance.AddTapHoldObject(this);
        void OnDisable() => TapHoldManager.Instance.RemoveTapHoldObject(this);
        #endregion
        void Update()
        {
            CheckHold();
        }

        public bool CheckTouchPosition(Vector2 touchPosition) { return _collider.bounds.Contains(touchPosition); }
        private void ToggleIsHolding(bool state) => _isHolding = state;
        private bool IsHoldCompleted() => _holdDurationCounter >= _holdDuration;

        public void Interact()
        {
            if (_inputType == InputType.Tap)
            {
                OnTapBegin?.Invoke();
            }
            else
            {
                _isHoldEnded = false;
                _holdDurationCounter = 0;
                OnHoldBegin?.Invoke();
            }
        }
        public void EndInteract(Vector2 mouseWorldPosition)
        {
            if (_checkPositionOnRelease && CheckTouchPosition(mouseWorldPosition) is not true)
            {
                OnInteractionFail?.Invoke();
                return;
            }

            if (_inputType == InputType.Tap)
            {
                OnTapCompleted?.Invoke();
            }
            else
            {
                if (_isHoldEnded) return;
                if (IsHoldCompleted() || _checkHoldDuration is not true)
                    OnHoldCompleted?.Invoke();
                else
                    OnInteractionFail?.Invoke();

                _isHoldEnded = true;
                _holdDurationCounter = 0;
            }
        }
        private void CheckHold()
        {
            if (_isHolding == false) return;

            if (_checkHoldDuration)
            {
                _holdDurationCounter += Time.deltaTime;

                if (_holdDurationCounter >= _holdDuration)
                    EndInteract(Vector2.zero);
            }

            OnHolding?.Invoke();
        }

    }
}