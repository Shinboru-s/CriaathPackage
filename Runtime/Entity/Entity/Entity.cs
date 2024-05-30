using System;
using Criaath.AdvState;
using NaughtyAttributes;
using UnityEngine;

namespace Criaath
{
    public class Entity<T> : MonoBehaviour, IDamageable, IHealable where T : EntityData
    {
        [Foldout("Base Settings")] public Animator Animator;
        [Foldout("Base Settings")] public Rigidbody2D Rb;
        [Foldout("Base Settings")] public StateMachine StateMachine;
        [Foldout("Base Settings")] public T Data;

        #region Movement Variables
        [HideInInspector] public bool IsFacingRight = true;
        [Foldout("Serialized Variables")][ReadOnly] public Vector2 Movement;
        #endregion

        #region Health Variables
        public int CurrentHealth { get; private set; }
        public Action OnDeath;
        public Action<int> OnHealthChanged;
        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            InitializeEntity();
        }

        protected virtual void InitializeEntity() { }
        #endregion

        #region Health Functions
        public virtual void SetHealth(int value)
        {
            CurrentHealth = value;
            CheckHealthLimits();
            OnHealthChanged?.Invoke(CurrentHealth);
        }
        public virtual void UpdateHealth(int changeValue)
        {
            CurrentHealth += changeValue;
            CheckHealthLimits();
            OnHealthChanged?.Invoke(CurrentHealth);

            if (CurrentHealth <= 0) OnDeath?.Invoke();
        }
        private void CheckHealthLimits() => CurrentHealth = Mathf.Clamp(CurrentHealth, 0, Data.MaxHealth);

        public void TakeDamage(int damage) => UpdateHealth(-damage);
        public void TakeHeal(int heal) => UpdateHealth(heal);
        #endregion

        #region Movement Functions
        public virtual void Move() { }
        public virtual void Move(Vector2 target)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, Data.Speed * Time.deltaTime);
        }
        public void SetVelocityZero()
        {
            Rb.velocity = Vector2.zero;
        }

        public virtual void CheckFlip()
        {
            if (Movement.x > 0 && !IsFacingRight)
                Flip();

            else if (Movement.x < 0 && IsFacingRight)
                Flip();
        }
        public void Flip()
        {
            IsFacingRight = !IsFacingRight;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
        #endregion
    }
}