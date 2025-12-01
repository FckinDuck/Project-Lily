using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyChaseState : EnemyState
{
    private Transform _playerTransform;
    private float _moveSpeed = 1.7f;

    public EnemyChaseState(EmemyHealth enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void AnimationTrigger(EmemyHealth.AnimationTriggersType triggersType)
    {
        base.AnimationTrigger(triggersType);
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.Move((Vector2)(((_playerTransform.position - enemy.transform.position).normalized) * _moveSpeed));

        if (enemy.IsWithinStrikeDistance)
        {
            enemy.stateMachine.ChangeState(enemy.attackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
