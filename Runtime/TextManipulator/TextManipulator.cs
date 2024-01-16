using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Criaath.Text
{
    public class TextManipulator : MonoBehaviour
    {
        [Required][SerializeField] private TextMeshProUGUI _textMesh;
        public Action OnValueChanged;

        private List<Action> onValueChangedActions = new List<Action>();

        // Text change
        public void SetText(string text)
        {
            _textMesh.text = text;
            OnValueChanged?.Invoke();
        }
        public void SetValue(float value) => SetText(value.ToString());
        public void SetValue(int value) => SetText(value.ToString());

        public void AddValue(float value) => SetValue(GetFloatValue() + value);
        public void AddValue(int value) => SetValue(GetIntValue() + value);

        // Add event

        public void WaitValueAndInvoke(string value, Action action)
        {
            Action checkAndInvoke = () => CheckAndInvokeAction(value, action);
            OnValueChanged += checkAndInvoke;
            onValueChangedActions.Add(checkAndInvoke);
        }

        public void CheckAndInvokeAction(string value, Action action)
        {
            if (GetValue() != value) return;

            action?.Invoke();

            Action checkAndInvoke = () => CheckAndInvokeAction(value, action);
            if (onValueChangedActions.Contains(checkAndInvoke))
            {
                OnValueChanged -= checkAndInvoke;
                onValueChangedActions.Remove(checkAndInvoke);
            }

        }

        // Font change
        public void SetFont(TMP_FontAsset font)
        {
            _textMesh.font = font;
        }

        // Color change
        public void SetColor(Color color)
        {
            _textMesh.color = color;
        }

        public void SetAlpha(float value)
        {
            Math.Clamp(value, 0.0f, 1.0f);
            _textMesh.alpha = value;
        }

        // Get funcs

        public TextMeshProUGUI GetTextMesh() => _textMesh;
        public string GetValue() => _textMesh.text;

        public float GetFloatValue()
        {
            float value;

            if (float.TryParse(_textMesh.text, out value)) return value;

            else
            {
                Debug.LogError($"{_textMesh.text} failed to convert to int");
                return 0;
            }
        }

        public int GetIntValue()
        {
            int value;

            if (int.TryParse(_textMesh.text, out value)) return value;

            else
            {
                Debug.LogError($"{_textMesh.text} failed to convert to int");
                return 0;
            }
        }
    }
}
