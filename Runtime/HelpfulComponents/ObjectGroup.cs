using NaughtyAttributes;
using UnityEngine;

namespace Criaath.MiniComponents
{
    [DefaultExecutionOrder(-5)]
    public class ObjectGroup : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float _alpha = 1;
        private SpriteRenderer[] _allChildSpriteRenderers;
        private float[] _initialAlphas;



        private void GetChildSpriteRenderers()
        {
            _allChildSpriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
            _initialAlphas = new float[_allChildSpriteRenderers.Length];

            for (int i = 0; i < _allChildSpriteRenderers.Length; i++)
            {
                _initialAlphas[i] = _allChildSpriteRenderers[i].color.a;
            }
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_allChildSpriteRenderers == null) GetChildSpriteRenderers();
            SetAlpha(_alpha);
        }

        [Button]
        public void SetAlphaValues()
        {
            GetChildSpriteRenderers();
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
                color.a = _initialAlphas[i] * alpha;
                _allChildSpriteRenderers[i].color = color;
            }

            _alpha = alpha;
        }
    }
}
