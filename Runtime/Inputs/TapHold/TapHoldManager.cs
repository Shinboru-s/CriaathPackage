using System.Collections.Generic;
using Criaath.MiniTools;
using UnityEngine;

namespace Criaath.Inputs
{
    [DefaultExecutionOrder(-5)]
    public class TapHoldManager : CriaathSingleton<TapHoldManager>
    {
        private List<TapHoldObject> _tapHoldObjects = new();
        private TapHoldObject _interactedObject;

        public void AddTapHoldObject(TapHoldObject tapHoldWorldObject) => _tapHoldObjects.Add(tapHoldWorldObject);
        public void RemoveTapHoldObject(TapHoldObject tapHoldWorldObject) => _tapHoldObjects.Remove(tapHoldWorldObject);

        public void TryToInteract(Vector2 mouseWorldPosition)
        {
            _interactedObject = null;
            foreach (var tapHoldObject in _tapHoldObjects)
            {
                if (tapHoldObject.CheckTouchPosition(mouseWorldPosition))
                {
                    _interactedObject = tapHoldObject;
                    _interactedObject.Interact();
                    break;
                }
            }
        }

        public void EndInteraction(Vector2 mouseWorldPosition)
        {
            if (_interactedObject == null) return;
            _interactedObject.EndInteract(mouseWorldPosition);
        }
    }
}