using System;
using Criaath.AdvState;
using NaughtyAttributes;
using UnityEngine;

namespace Criaath.Entity
{
    public class Entity<T> : MonoBehaviour where T : EntityData
    {
        [Foldout("Base Settings")] public Animator Animator;
        [Foldout("Base Settings")] public StateMachine StateMachine;
        [Foldout("Base Settings")] public T Data;

        #region Initialization
        protected virtual void Awake()
        {
            InitializeEntity();
        }

        protected virtual void InitializeEntity() { }
        #endregion

    }
}