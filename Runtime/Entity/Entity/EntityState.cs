using Criaath;
using Criaath.AdvState;

namespace Criaath
{
    public class EntityState<T, U> : State where T : Entity<U> where U : EntityData
    {
        protected T entity;
        protected readonly string animBoolName;

        public EntityState(T entity, U data, string animBoolName)
        {
            this.entity = entity;
            this.animBoolName = animBoolName;
            this.entity.Data = data;
        }

        public override void Enter()
        {
            base.Enter();
            entity.Animator.SetBool(animBoolName, true);
        }
        public override void Do()
        {
            base.Do();
        }
        public override void FixedDo()
        {
            base.FixedDo();
        }
        public override void Exit()
        {
            base.Exit();
            entity.Animator.SetBool(animBoolName, false);
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();
        }

    }
}