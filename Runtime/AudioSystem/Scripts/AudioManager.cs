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
        [Foldout("Pool Settings")][SerializeField] private GameObject _audioPlayerPrefab;
        [Foldout("Pool Settings")][SerializeField] private int _defaultPoolSize = 10;

        [OnValueChanged("SetVolume")]
        [Foldout("Volume Settings")]
        [SerializeField][Range(0, 1)] private float _sfxVolume = 0.5f;

        [OnValueChanged("SetVolume")]
        [Foldout("Volume Settings")]
        [SerializeField][Range(0, 1)] private float _musicVolume = 0.5f;

        private ObjectPool<AudioPlayer> _audioPlayerPool;
        private List<AudioPlayer> _audioPlayersInUse = new();
        private uint _nextUsablePlayerId = 0;

        void Reset()
        {
            if (_audioPlayerPrefab == null)
            {
                _audioPlayerPrefab = Resources.Load<GameObject>("Prefabs/pref_CriaathAudioPlayer");
                CriaathDebugger.Log("AudioManager", "Audio Player Prefab sets automatically.");
            }
        }
        new void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            _audioPlayerPool = new ObjectPool<AudioPlayer>(_audioPlayerPrefab, transform, _defaultPoolSize, false);
            _audioPlayerPool.OnNewItemSpawn += InitializeOnSpawn;
            _audioPlayerPool.GeneratePool();
        }
        private void InitializeOnSpawn(AudioPlayer audioPlayer)
        {
            audioPlayer.OnAudioStop += () => StopAudioPlayer(audioPlayer);
        }
        private void StopAudioPlayer(AudioPlayer audioPlayer)
        {
            if (_audioPlayersInUse.Contains(audioPlayer) is not true) return;

            _audioPlayersInUse.Remove(audioPlayer);
            _audioPlayerPool.PushItem(audioPlayer);
        }

        private AudioPlayer PrepareAudioPlayer(Criaath.Audio.AudioClip clip)
        {
            if (clip == null || clip.Clip == null)
            {
                CriaathDebugger.LogError("AudioManager", Color.yellow, "AudioClip or Clip is null! It cannot playable!");
                return null;
            }

            AudioPlayer audioPlayer = _audioPlayerPool.Pull();
            audioPlayer.gameObject.SetActive(true);
            audioPlayer.SetClipSettings(clip);
            audioPlayer.Id = GetNextPlayerId();
            UpdatePlayerVolume(audioPlayer);
            _audioPlayersInUse.Add(audioPlayer);
            return audioPlayer;
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

            foreach (var audioPlayer in _audioPlayersInUse)
            {
                if (audioPlayer.CheckType(type))
                    audioPlayer.SetVolume(volume);
            }
            OnVolumeUpdate?.Invoke(type, volume);
        }

        public void UpdatePlayerVolume(AudioPlayer player)
        {
            if (player.CheckType(AudioType.Music))
                player.SetVolume(_musicVolume);
            else
                player.SetVolume(_sfxVolume);
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

        #region Player Functions
        public uint Play(Criaath.Audio.AudioClip audioClip)
        {
            AudioPlayer audioPlayer = PrepareAudioPlayer(audioClip);
            audioPlayer.Play();

            return audioPlayer.Id;
        }
        public void Stop(uint playerId)
        {
            GetPlayerWithId(playerId)?.Stop();
        }
        public void Pause(uint playerId)
        {
            GetPlayerWithId(playerId)?.Pause();
        }
        public void Resume(uint playerId)
        {
            GetPlayerWithId(playerId)?.Resume();
        }
        public uint FadeIn(Criaath.Audio.AudioClip audioClip, float fadeDuration)
        {
            AudioPlayer audioPlayer = PrepareAudioPlayer(audioClip);
            audioPlayer.FadeIn(fadeDuration);

            return audioPlayer.Id;
        }
        public void FadeOut(uint playerId, float fadeDuration)
        {
            GetPlayerWithId(playerId)?.FadeOut(fadeDuration);
        }
        public uint PlayRandomPitch(Criaath.Audio.AudioClip audioClip, float range)
        {
            AudioPlayer audioPlayer = PrepareAudioPlayer(audioClip);

            audioPlayer.PlayRandomPitch(range);
            return audioPlayer.Id;
        }
        public void SetRandomPitch(uint playerId, float range)
        {
            GetPlayerWithId(playerId)?.PlayRandomPitch(range);
        }
        #endregion
        private uint GetNextPlayerId()
        {
            return ++_nextUsablePlayerId;
        }
        private AudioPlayer GetPlayerWithId(uint playerId)
        {
            foreach (AudioPlayer player in _audioPlayersInUse)
            {
                if (player.Id == playerId)
                    return player;
            }
            return null;
        }
    }
}
