using UnityEngine;

public class EnemyState
{
    protected EmemyHealth enemy;
    protected EnemyStateMachine stateMachine;

    public EnemyState(EmemyHealth ememy, EnemyStateMachine stateMachine)
    {
        this.enemy = ememy;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void AnimationTrigger(EmemyHealth.AnimationTriggersType triggersType) { }
}
