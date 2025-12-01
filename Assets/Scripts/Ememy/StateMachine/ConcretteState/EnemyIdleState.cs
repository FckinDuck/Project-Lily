using System.Collections;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private Vector3 _targetPos;
    private Vector3 _dirction;
    public EnemyIdleState(EmemyHealth enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void AnimationTrigger(EmemyHealth.AnimationTriggersType triggersType)
    {
        base.AnimationTrigger(triggersType);
    }

    public override void EnterState()
    {
        base.EnterState();

        _targetPos = GetRandomPoint();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (enemy.IsAggroed)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }

        _dirction = (_targetPos - enemy.transform.position).normalized;

        enemy.Move(_dirction * enemy.randomMoveSpeed);
        if ((enemy.transform.position - _targetPos).sqrMagnitude <0.01f)
        {
            _targetPos = GetRandomPoint();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private Vector3 GetRandomPoint()
    {
        return enemy.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * enemy.randomMoveRange;
    }
}
