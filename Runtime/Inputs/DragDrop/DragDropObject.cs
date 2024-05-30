using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Criaath.Inputs
{
    public class DragDropObject : MonoBehaviour
    {
        public int DraggableIndex = 0;
        [SerializeField][ReadOnly] private bool _canDraggable = true;

        [Foldout("Settings"), SerializeField] private TapHoldObject _tapHoldScript;
        [Foldout("Settings"), SerializeField] private bool _returnStartOnFail = true;
        [Foldout("Settings"), SerializeField] private bool _snapToContainer = true;

        [Foldout("Movement")] public float MoveSpeed = 5;

        [Foldout("Events")] public UnityEvent OnDragBegin, OnDrag, OnDragEnd, OnDropFail, OnDropSuccess, OnReturnedToStart, OnContainerAssigned;

        private Vector2 _dragStartPosition;
        private bool _dragging;
        [SerializeField, ReadOnly] private ContainerObject _assignedContainer;
        private Coroutine _moveCoroutine;

        #region UnityFunctions
        void Awake()
        {
            _tapHoldScript.OnHoldBegin.AddListener(BeginDrag);
            _tapHoldScript.OnHoldCompleted.AddListener(EndDrag);
        }
        void OnEnable() => OnDrag.AddListener(FollowMouse);
        void OnDisable() => OnDrag.RemoveListener(FollowMouse);
        void Update()
        {
            Drag();
        }
        #endregion

        public void BeginDrag()
        {
            if (_canDraggable is not true) return;

            if (_assignedContainer != null)
            {
                _assignedContainer.OnDraggablePick?.Invoke(this);
            }

            _dragStartPosition = transform.position;
            _dragging = true;
            OnDragBegin?.Invoke();
        }
        private void Drag()
        {
            if (_dragging is not true) return;
            OnDrag?.Invoke();
        }

        public void EndDrag()
        {
            if (_dragging is not true) return;

            _dragging = false;
            ContainerObject container = DragDropManager.Instance.GetContainer();
            Drop(container);
            OnDragEnd?.Invoke();
        }

        public virtual void Drop(ContainerObject container)
        {
            if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);

            if (container != null && container.CheckIndex(DraggableIndex))
            {
                if (_snapToContainer)
                    MovePosition(container.transform.position, false);

                AssignToContainer(container);
                OnDropSuccess?.Invoke();
            }
            else
            {
                if (_returnStartOnFail || _assignedContainer != null)
                {
                    ReturnToStart();
                }

                OnDropFail?.Invoke();
            }
        }

        private void FollowMouse()
        {
            Vector2 mousePosition = DragDropManager.Instance.GetMousePosition();
            transform.position = mousePosition;
        }

        private void ReturnToStart() => MovePosition(_dragStartPosition, true);

        private void MovePosition(Vector2 position, bool invokeEvent)
        {
            _moveCoroutine = StartCoroutine(MoveToPosition(position, invokeEvent));
        }
        private IEnumerator MoveToPosition(Vector2 targetPosition, bool invokeEvent)
        {
            ToggleDraggable(false);

            Vector2 startPosition = transform.position;
            float travelDistance = Vector2.Distance(startPosition, targetPosition);
            float travelTime = travelDistance / MoveSpeed;
            float elapsedTime = 0f;

            while (elapsedTime < travelTime)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / travelTime);
                transform.position = Vector2.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
            transform.position = targetPosition;

            ToggleDraggable(true);

            if (invokeEvent)
                OnReturnedToStart?.Invoke();
        }

        private void AssignToContainer(ContainerObject newContainer)
        {
            _assignedContainer = newContainer;
            OnContainerAssigned?.Invoke();
            _assignedContainer.OnDraggableDrop?.Invoke(this);
        }

        public ContainerObject GetAssignContainer() { return _assignedContainer; }
        public void ToggleDraggable(bool state) => _canDraggable = state;
    }
}
