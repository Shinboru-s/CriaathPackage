using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Criaath
{
    public class AnimationEventHelper : MonoBehaviour
    {
        public UnityEvent AnimationFinish;
        public void AnimationFinishTrigger()
        {
            AnimationFinish?.Invoke();
        }
    }
}
