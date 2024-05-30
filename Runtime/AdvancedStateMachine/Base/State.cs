using UnityEngine;

namespace Criaath.AdvState
{
    public abstract class State
    {
        public bool IsComplete { get; protected set; }
        protected float enterTime;

        public float StateTime => Time.time - enterTime;

        public virtual void Enter() { }
        public virtual void Do() { }
        public virtual void FixedDo() { }
        public virtual void Exit() { }

        public void Initialize()
        {
            enterTime = Time.time;
            IsComplete = false;
        }

        public virtual void AnimationFinishTrigger() { }
    }
}
