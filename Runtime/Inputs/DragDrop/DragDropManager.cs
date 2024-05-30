using System.Collections.Generic;
using Criaath.MiniTools;
using UnityEngine;

namespace Criaath.Inputs
{
    [DefaultExecutionOrder(-5)]
    public class DragDropManager : CriaathSingleton<DragDropManager>
    {
        private Camera _mainCamera;
        private List<ContainerObject> _containerObjects = new();

        public void AddContainerObject(ContainerObject container) => _containerObjects.Add(container);
        public void RemoveContainerObject(ContainerObject container) => _containerObjects.Remove(container);

        public ContainerObject GetContainer()
        {
            Vector2 mousePosition = GetMousePosition();
            foreach (var container in _containerObjects)
            {
                if (container.CheckContainer(mousePosition))
                    return container;

            }
            CriaathDebugger.Log("DragDropManager", Color.blue, "Container not found in position!");
            return null;
        }

        public Vector2 GetMousePosition()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;

            return _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}

