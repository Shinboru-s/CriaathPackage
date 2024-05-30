using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Criaath.Inputs
{
    [RequireComponent(typeof(EventTrigger))]
    public class TapHoldWindow : MonoBehaviour
    {
        [SerializeField] private EventTrigger _eventTrigger;
        private Camera _mainCamera;
        [SerializeField, ReadOnly] private bool _interactable = true;

        #region Built-in
        void Awake()
        {
            AddEvent();
        }
        #endregion

        private void AddEvent()
        {
            EventTrigger.Entry entryPointerDown = new()
            {
                eventID = EventTriggerType.PointerDown
            };
            entryPointerDown.callback.AddListener((data) => OnPointerDown((PointerEventData)data));
            _eventTrigger.triggers.Add(entryPointerDown);

            EventTrigger.Entry entryPointerUp = new()
            {
                eventID = EventTriggerType.PointerUp,
            };
            entryPointerUp.callback.AddListener((data) => OnPointerUp((PointerEventData)data));
            _eventTrigger.triggers.Add(entryPointerUp);
        }

        public void OnPointerDown(PointerEventData pointerEventData)
        {
            if (_interactable is not true) return;

            Vector3 worldPos = GetWorldPosition(pointerEventData);
            TapHoldManager.Instance.TryToInteract(worldPos);
        }
        public void OnPointerUp(PointerEventData pointerEventData)
        {
            if (_interactable is not true) return;

            Vector3 worldPos = GetWorldPosition(pointerEventData);
            TapHoldManager.Instance.EndInteraction(worldPos);
        }

        private Vector3 GetWorldPosition(PointerEventData pointerEventData)
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;

            return _mainCamera.ScreenToWorldPoint(pointerEventData.position);
        }
        public void ToggleInteract(bool toggle) => _interactable = toggle;
    }
}