using NaughtyAttributes;
using System;
using UnityEngine;

namespace Criaath.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip m_AudioClip;
        [SerializeField] private AudioSource _audioSource;

        public AudioClip AudioClip
        {
            private set => m_AudioClip = value;
            get => m_AudioClip;
        }

        [Tooltip("Play audio in the selected event")]
        [Foldout("Settings")][SerializeField] private bool _autoPlay = false;
        [Tooltip("Select which event to play at")]
        [ShowIf("_autoPlay")][Foldout("Settings")][SerializeField] private PlayOnThis _playOn;

        private Action _autoPlayAction;

        #region Built-in
        private void Awake()
        {
            Initialize();

            if (!_autoPlay) return;
            _autoPlayAction += Play;

            if (_playOn == PlayOnThis.Awake)
                _autoPlayAction?.Invoke();
        }

        private void OnEnable()
        {
            if (_playOn == PlayOnThis.OnEnable)
                _autoPlayAction?.Invoke();
        }
        private void Start()
        {
            if (_playOn == PlayOnThis.Start)
                _autoPlayAction?.Invoke();
        }
        private void OnDisable()
        {
            if (_playOn == PlayOnThis.OnDisable)
                _autoPlayAction?.Invoke();
        }
        private void OnDestroy()
        {
            if (_playOn == PlayOnThis.OnDestroy)
                _autoPlayAction?.Invoke();
        }
        #endregion

        private void Initialize()
        {
            _audioSource.SetClipSettings(m_AudioClip);
        }
        public void SetClip(AudioClip clip)
        {
            AudioClip = clip;
            Initialize();
        }
        public void Play()
        {
            _audioSource.Play();
        }
        public void Stop()
        {
            _audioSource.Stop();
        }
        public void PlayRandomPitch(float range)
        {
            _audioSource.PlayRandomPitch(range);
        }
        public void SetVolume(float volume)
        {
            _audioSource.SetVolume(volume);
        }
    }
}
