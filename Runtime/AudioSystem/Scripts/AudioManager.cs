using Criaath.MiniTools;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Criaath.Audio
{
    [DefaultExecutionOrder(-5)]
    public class AudioManager : CriaathSingleton<AudioManager>
    {
        [Foldout("Pool Settings")][SerializeField] private GameObject _audioSourcePrefab;
        [Foldout("Pool Settings")][SerializeField] private int _defaultPoolSize = 10;

        [OnValueChanged("SetVolume")]
        [Foldout("Volume Settings")]
        [SerializeField][Range(0, 1)] private float _sfxVolume = 0.5f;

        [OnValueChanged("SetVolume")]
        [Foldout("Volume Settings")]
        [SerializeField][Range(0, 1)] private float _musicVolume = 0.5f;

        private ObjectPool<AudioSource> _audioSourcePool;
        private List<AudioSource> _audioSourcesInUse = new();

        new void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            _audioSourcePool = new ObjectPool<AudioSource>(_audioSourcePrefab, transform, _defaultPoolSize, false);
            _audioSourcePool.OnNewItemSpawn += Test;
            _audioSourcePool.GeneratePool();
        }
        private void Test(AudioSource audioSource)
        {
            audioSource.ManagerInitialize();
        }

        public void Play(UnityEngine.AudioClip audioClip)
        {
            if (audioClip == null) return;

            AudioSource audioSource = PrepareAudioSource();
            audioSource.SetClipSettings(audioClip);

            StartAudio(audioSource);
        }
        public void Play(AudioClip audioClip)
        {
            if (audioClip == null) return;
            if (audioClip.Clip == null) return;

            AudioSource audioSource = PrepareAudioSource();
            audioSource.SetClipSettings(audioClip);

            StartAudio(audioSource);
        }

        private void StartAudio(AudioSource audioSource)
        {
            UpdateSourceVolume(audioSource);
            audioSource.Play();
            _audioSourcesInUse.Add(audioSource);
        }

        private AudioSource PrepareAudioSource()
        {
            AudioSource audioSource = _audioSourcePool.Pull();
            audioSource.gameObject.SetActive(true);

            return audioSource;
        }

        public void StopAudioSource(AudioSource audioSource)
        {
            if (_audioSourcesInUse.Contains(audioSource) is not true) return;

            _audioSourcesInUse.Remove(audioSource);
            _audioSourcePool.PushItem(audioSource);
        }

        private void SetVolume()
        {
            SetVolume(AudioType.SFX, _sfxVolume);
            SetVolume(AudioType.Music, _musicVolume);
        }
        public Action<AudioType, float> OnVolumeUpdate;
        public void SetVolume(AudioType type, float volume)
        {
            volume = Math.Clamp(volume, 0f, 1f);

            if (type == AudioType.Music)
                _musicVolume = volume;
            else if (type == AudioType.SFX)
                _sfxVolume = volume;

            foreach (var audioSource in _audioSourcesInUse)
            {
                if (audioSource.CheckType(type))
                    audioSource.SetVolume(volume);
            }
            OnVolumeUpdate?.Invoke(type, volume);
        }

        public void UpdateSourceVolume(AudioSource source)
        {
            if (source.CheckType(AudioType.Music))
                source.SetVolume(_musicVolume);
            else
                source.SetVolume(_sfxVolume);
        }
        public float GetVolume(AudioType type)
        {
            if (type == AudioType.Music)
                return _musicVolume;
            else if (type == AudioType.SFX)
                return _sfxVolume;
            else
                return 1;
        }
    }
}
