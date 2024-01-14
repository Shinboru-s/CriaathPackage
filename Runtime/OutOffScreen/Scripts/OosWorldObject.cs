using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Criaath.OOS
{
    public class OosWorldObject : OosBase
    {
        [SerializeField] private Transform _objectToMove;

        protected override void MoveIt()
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = WaitAndMove();
            StartCoroutine(_moveCoroutine);
        }

        private IEnumerator WaitAndMove()
        {
            yield return new WaitForSeconds(_delay);

            OnMovementStated?.Invoke();
            _moveTween = _objectToMove.DOMove(_movePosition, _realTravelTime).SetEase(_ease).OnComplete(() => OnMovementFinished?.Invoke());
        }

        protected override void CalculateScreenSize()
        {
            Vector2 pixelScreenSize = new Vector2(Screen.width, Screen.height);
            Vector2 screenSize = Camera.main.ScreenToWorldPoint(pixelScreenSize);
            _screenHeight = new Vector3(0f, screenSize.y * 2, 0f);
            _screenWidth = new Vector3(screenSize.x * 2, 0f, 0f);
        }

        protected override Vector3 GetObjectPosition()
        {
            return _objectToMove.position;
        }
    }
}
