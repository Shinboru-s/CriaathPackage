using System;
using System.Linq;
using UnityEngine;

namespace Criaath.Inputs
{
    public class ContainerObject : MonoBehaviour
    {
        [SerializeField] private Collider2D _collider;
        public Action<DragDropObject> OnDraggableDrop, OnDraggablePick;

        [SerializeField] int[] _containableIndexes = new int[] { 0 };

        void OnEnable() => DragDropManager.Instance.AddContainerObject(this);
        void OnDisable() => DragDropManager.Instance.RemoveContainerObject(this);

        public bool CheckContainer(Vector2 mousePosition) => _collider.bounds.Contains(mousePosition);
        public bool CheckIndex(int draggableIndex) => _containableIndexes.Contains(draggableIndex);
        public void SetContainableIndexes(int[] containableIndexes) => _containableIndexes = containableIndexes;

    }
}
