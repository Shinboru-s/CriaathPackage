using UnityEngine;

namespace Criaath
{
    [CreateAssetMenu(fileName = "_EntityData", menuName = "Criaath/Data/EntityData")]
    public class EntityData : ScriptableObject
    {
        public int MaxHealth;
        public float Speed;
    }
}