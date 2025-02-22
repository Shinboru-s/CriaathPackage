using UnityEngine;

namespace Criaath.MiniTools
{
    public class CriaathSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }
        protected virtual PersistenceOption Persistence => PersistenceOption.None;

        protected virtual void Awake()
        {
            SetInstance();
            if (Persistence == PersistenceOption.DontDestroyOnLoad)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        protected virtual void SetInstance()
        {
            if (Instance != null) Destroy(this.gameObject);
            else Instance = this as T;
        }
    }
    public enum PersistenceOption
    {
        None,
        DontDestroyOnLoad
    }
}
