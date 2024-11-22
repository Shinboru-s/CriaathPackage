using UnityEngine;
using UnityEngine.UI;

namespace Criaath.Audio
{
    public class AudioSlider : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private AudioType _type;

        private void Reset()
        {
            _slider = GetComponent<Slider>();
        }
        private void Start()
        {
            _slider.value = AudioManager.Instance.GetVolume(_type);
            AudioManager.Instance.OnVolumeUpdate += (AudioType type, float volume) =>
            {
                if (_type == type)
                    _slider.value = volume;
            };

            _slider.onValueChanged.AddListener((value) =>
            {
                AudioManager.Instance.SetVolume(_type, (float)value);
            });
        }
    }
}
