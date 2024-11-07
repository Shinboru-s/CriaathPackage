#if NETCODE_ENABLED
using UnityEngine;
using Unity.Netcode;

namespace Criaath.MiniTools
{
    public class CriaathNetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
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
}
#endif