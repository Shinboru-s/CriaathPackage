using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Criaath.OOS
{
    public abstract class OosBase : MonoBehaviour
    {
        #region SerializeField Variables

        [BoxGroup("Movement Settings")]
        [SerializeField] private OosPositions _targetPosition;

        [BoxGroup("Movement Settings")]
        [ShowIf("_targetPosition", OosPositions.CustomPosition)]
        [SerializeField] private Vector3 _customPosition;

        [BoxGroup("Movement Settings")]
        [SerializeField] protected float _delay;

        [BoxGroup("Movement Settings")]
        [SerializeField] protected float _travelTime;

        [BoxGroup("Movement Settings")]
        [SerializeField] protected Ease _ease;

        [Tooltip("To calculate target position using start position")]
        [Foldout("Special Settings")][SerializeField] private bool _fixedToStartPosition;

        [Foldout("Events")] public UnityEvent OnMovementStated;
        [Foldout("Events")] public UnityEvent OnMovementFinished;

        #endregion



        #region Protected Variables

        protected Vector3 _movePosition;
        protected Vector3 _screenHeight;
        protected Vector3 _screenWidth;
        protected IEnumerator _moveCoroutine;
        protected float _realTravelTime;
        protected Vector3 _startPosition;
        protected Tween _moveTween;

        #endregion

        private void Update()
        {
            if (_moveTween == null)
                Debug.Log("move tween is null");
            else
                Debug.Log("there is tween here");
        }

        #region Built-in

        void OnEnable()
        {
            OnMovementFinished.AddListener(() => _moveTween = null);

            _startPosition = GetObjectPosition();
            CalculateScreenSize();
        }

        #endregion



        #region protected virtual Funcs

        protected virtual void CalculateScreenSize()
        {

        }

        protected virtual void MoveIt()
        {

        }

        protected virtual Vector3 GetObjectPosition()
        {
            return new Vector3(0, 0, 0);
        }

        #endregion



        #region Public Funcs

        public void MoveToTarget()
        {
            _realTravelTime = _travelTime;
            MoveToPosition(_targetPosition);
        }

        public void MoveToTargetImmediately()
        {
            _realTravelTime = 0;
            MoveToPosition(_targetPosition);
        }

        public void StopMovement(bool complete)
        {
            if (_moveTween == null) return;

            if (complete)
                _moveTween.Complete();
            else
                _moveTween.Kill();

            _moveTween = null;
        }

        public void MoveToPosition(OosPositions targetPosition)
        {
            Vector3 objectPosition = GetObjectPosition();

            if (_fixedToStartPosition)
                objectPosition = _startPosition;

            switch (targetPosition)
            {
                case OosPositions.Up:
                    _movePosition = objectPosition + _screenHeight;
                    break;

                case OosPositions.Down:
                    _movePosition = objectPosition - _screenHeight;
                    break;

                case OosPositions.Left:
                    _movePosition = objectPosition - _screenWidth;
                    break;

                case OosPositions.Right:
                    _movePosition = objectPosition + _screenWidth;
                    break;

                case OosPositions.Center:
                    _movePosition = new Vector3(0, 0, 0);
                    break;

                case OosPositions.CustomPosition:
                    _movePosition = _customPosition;
                    break;
                case OosPositions.StartPosition:
                    _movePosition = _startPosition;
                    break;

                default:
                    _movePosition = objectPosition;
                    break;
            }

            MoveIt();
        }

        #endregion
    }
}
