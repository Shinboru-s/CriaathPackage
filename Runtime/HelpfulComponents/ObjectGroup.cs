using UnityEngine;

namespace Criaath.MiniComponents
{
    [DefaultExecutionOrder(-5)]
    public class ObjectGroup : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float _alpha = 1;
        private SpriteRenderer[] _allChildSpriteRenderers;
        private void GetChildSpriteRenderers()
        {
            _allChildSpriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_allChildSpriteRenderers == null) GetChildSpriteRenderers();
            SetAlpha(_alpha);
        }

#else
    private void Awake()
    {
        if (_allChildSpriteRenderers == null) GetChildSpriteRenderers();
        SetAlpha(_alpha);
    }
#endif
        public void SetAlpha(float alpha)
        {
            alpha = Mathf.Clamp01(alpha);
            for (int i = 0; i < _allChildSpriteRenderers.Length; i++)
            {
                Color color = _allChildSpriteRenderers[i].color;
                color.a = alpha;
                _allChildSpriteRenderers[i].color = color;
            }

            // for itself
            if (gameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }


            _alpha = alpha;
        }
    }
}
