using System;
using System.Collections;
using UnityEngine;


namespace Criaath.Audio
{
    [RequireComponent(typeof(UnityEngine.AudioSource))]
    public class AudioSource : MonoBehaviour
    {
        [SerializeField] private UnityEngine.AudioSource _audioSource;
        private AudioType _type;
        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }
        private float _defaultVolume;
        private float _defaultPitch;
        private float _volumeMultiplier = 1;
        private IEnumerator _stopCoroutine;

        public Action OnAudioPlay;
        public Action OnAudioStop;

        void Awake()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.OnVolumeUpdate += CheckVolumeUpdate;
        }
        public void SetClipSettings(AudioClip audioClip)
        {
            _audioSource.clip = audioClip.Clip;
            _audioSource.pitch = audioClip.Pitch;
            _audioSource.loop = audioClip.Loop;
            _audioSource.volume = audioClip.Volume;

            _defaultVolume = audioClip.Volume;
            _defaultPitch = audioClip.Pitch;
            _type = audioClip.Type;
        }
        public void SetClipSettings(UnityEngine.AudioClip audioClip)
        {
            _audioSource.clip = audioClip;
            _audioSource.pitch = 1f;
            _audioSource.loop = false;
            _audioSource.volume = 1f;

            _defaultVolume = 1f;
            _defaultPitch = 1f;
            _type = AudioType.SFX;
        }
        private void CheckVolumeUpdate(AudioType type, float volume)
        {
            if (CheckType(type))
                SetVolume(volume);
        }
        public void SetVolume(float volume)
        {
            _volumeMultiplier = volume;
            _audioSource.volume = _defaultVolume * _volumeMultiplier;
        }

        public bool CheckType(AudioType type) { return _type == type; }

        public void Play()
        {
            if (IsPlaying)
                _audioSource.Stop();

            OnAudioPlay?.Invoke();
            _audioSource.Play();
            IsPlaying = true;
            IsPaused = false;

            if (_stopCoroutine != null) StopCoroutine(_stopCoroutine);

            if (_audioSource.loop == true) return;

            _stopCoroutine = CheckAudioFinished();
            StartCoroutine(_stopCoroutine);
        }

        public void Stop()
        {
            if (IsPlaying is not true) return;

            if (_stopCoroutine != null)
                StopCoroutine(_stopCoroutine);
            _audioSource.Stop();
            IsPlaying = false;
            IsPaused = false;
            OnAudioStop?.Invoke();
        }
        public void Pause()
        {
            if (IsPaused) return;
            _audioSource.Pause();
            IsPaused = true;
        }
        public void Resume()
        {
            if (IsPaused is not true) return;
            _audioSource.UnPause();
            IsPaused = false;
        }
        public void SetRandomPitch(float range)
        {
            float pitch = _defaultPitch;
            _audioSource.pitch = UnityEngine.Random.Range(pitch - range, pitch + range);
        }
        public void PlayRandomPitch(float range)
        {
            SetRandomPitch(range);
            Play();
        }

        public void ManagerInitialize()
        {
            OnAudioStop += () => AudioManager.Instance.StopAudioSource(this);
        }

        private IEnumerator CheckAudioFinished()
        {
            yield return new WaitUntil(() => !_audioSource.isPlaying && !IsPaused);
            Stop();
        }
        private void OnDestroy()
        {
            OnAudioStop -= () => AudioManager.Instance.StopAudioSource(this);
            AudioManager.Instance.OnVolumeUpdate -= CheckVolumeUpdate;
        }
    }
}
