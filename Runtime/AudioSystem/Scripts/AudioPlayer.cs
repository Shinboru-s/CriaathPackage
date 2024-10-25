using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;


namespace Criaath.Audio
{
    [RequireComponent(typeof(UnityEngine.AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        [ReadOnly] public uint Id;
        [SerializeField] private Criaath.Audio.AudioClip m_AudioClip;

        public AudioClip AudioClip
        {
            private set => m_AudioClip = value;
            get => m_AudioClip;
        }

        [Tooltip("Play audio in the selected event")]
        [Foldout("Settings")][SerializeField] private bool _autoPlay = false;
        [Tooltip("Select which event to play at")]
        [ShowIf("_autoPlay")][Foldout("Settings")][SerializeField] private PlayOnThis _playOn;

        [SerializeField] private AudioSource _audioSource;
        private AudioType _type;
        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }
        private float _defaultVolume;
        private float _defaultPitch;
        private float _volumeMultiplier = 1;
        private IEnumerator _stopCoroutine;
        private Coroutine _fadeCoroutine;

        public Action OnAudioPlay;
        public Action OnAudioStop;

        private Action _autoPlayAction;



        #region Built-in
        private void Reset()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        private void Awake()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.OnVolumeUpdate += CheckVolumeUpdate;

            SetClipSettings(m_AudioClip);

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

            AudioManager.Instance.OnVolumeUpdate -= CheckVolumeUpdate;
        }
        #endregion

        public void SetClip(AudioClip clip)
        {
            AudioClip = clip;
            SetClipSettings(m_AudioClip);

        }

        public void SetClipSettings(AudioClip audioClip)
        {
            if (audioClip == null) return;

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
        public void ResumeOrPlay()
        {
            if (IsPaused) Resume();
            else Play();
        }
        public void FadeIn(float duration, bool tryResume = false)
        {
            StopFadeCoroutine();
            _fadeCoroutine = StartCoroutine(FadeAudioIn(duration, tryResume));
        }
        public void FadeOut(float duration, bool pauseOnEnd = false)
        {
            StopFadeCoroutine();
            if (IsPlaying)
                _fadeCoroutine = StartCoroutine(FadeAudioOut(duration, pauseOnEnd));
        }
        private void StopFadeCoroutine()
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
        }
        private IEnumerator FadeAudioIn(float duration, bool tryResume, float startVolume = 0f)
        {
            float endVolume = _defaultVolume * _volumeMultiplier;
            _audioSource.volume = startVolume;

            if (tryResume)
                ResumeOrPlay();
            else
                Play();

            while (_audioSource.volume < endVolume)
            {
                _audioSource.volume += Time.deltaTime / duration;
                yield return null;
            }

            _audioSource.volume = endVolume;
        }

        private IEnumerator FadeAudioOut(float duration, bool pauseOnEnd)
        {
            float startVolume = _audioSource.volume;

            while (_audioSource.volume > 0f)
            {
                _audioSource.volume -= startVolume * Time.deltaTime / duration;
                yield return null;
            }

            if (pauseOnEnd)
                Pause();
            else
                Stop();

            _audioSource.volume = startVolume;
        }
        public void PlayRandomPitch(float range)
        {
            SetRandomPitch(range);
            Play();
        }
        public void SetRandomPitch(float range)
        {
            float pitch = _defaultPitch;
            _audioSource.pitch = UnityEngine.Random.Range(pitch - range, pitch + range);
        }

        private IEnumerator CheckAudioFinished()
        {
            yield return new WaitUntil(() => !_audioSource.isPlaying && !IsPaused);
            Stop();
        }

    }
}
