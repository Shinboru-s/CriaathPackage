using UnityEngine;

namespace Criaath.Audio
{
    [CreateAssetMenu(fileName = "_CriaathClip", menuName = "Criaath/Audio Clip")]
    public class AudioClip : ScriptableObject
    {
        [SerializeField] private UnityEngine.AudioClip m_Clip;
        public UnityEngine.AudioClip Clip
        {
            private set => m_Clip = value;
            get => m_Clip;
        }

        public AudioType Type;
        [Range(0, 1)] public float Volume = 0.5f;
        [Range(-3, 3)] public float Pitch = 1f;
        public bool Loop = false;

        public void Play()
        {
            AudioManager.Instance.Play(this);
        }

    }
}
