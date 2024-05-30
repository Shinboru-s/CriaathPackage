using System;
using Criaath.MiniTools;
using UnityEngine;

namespace Criaath.AdvState
{
    public class StateMachine : MonoBehaviour
    {
        public State CurrentState;
        public State PreviousState;
        public Action OnStateChanged;

        public void Initialize(State startingState)
        {
            CurrentState = startingState;
            CurrentState.Initialize();
            CurrentState.Enter();
            OnStateChanged?.Invoke();
            CriaathDebugger.Log("State Machine", Color.magenta, $"Current state is {CurrentState.GetType()}", Color.white);
        }


        public void ChangeState(State newState)
        {
            PreviousState = CurrentState;
            CurrentState = newState;

            PreviousState.Exit();
            CurrentState.Initialize();
            CurrentState.Enter();

            CriaathDebugger.Log("State Machine", Color.magenta, $"Current state is {CurrentState.GetType()}", Color.white);
            OnStateChanged?.Invoke();
        }

        void Update()
        {
            if (CurrentState == null) return;
            CurrentState.Do();
        }
        void FixedUpdate()
        {
            if (CurrentState == null) return;
            CurrentState.FixedDo();
        }
    }
}
