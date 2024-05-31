using UnityEngine;

namespace Criaath.MiniTools
{
    public class CriaathSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            SetInstance();
        }
        protected virtual void SetInstance()
        {
            if (Instance != null) Destroy(this.gameObject);
            else Instance = this as T;
        }
    }
}
