using System.Collections;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private float timer;
    private float attackCooldown = 2f;

    private float _exitTimer;
    private float _timeTillExit = 3f;
    private float _distanceToCountExit = 2f;

    public EnemyAttackState(EmemyHealth enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
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

        enemy.Move(Vector2.zero);
        if (timer > attackCooldown)
        {
            timer = 0f;
            //commence attack
            Debug.Log("Attack commenced!");
        }
        if (!enemy.IsWithinStrikeDistance)
        {
            _exitTimer += Time.deltaTime;
            if (_exitTimer >= _timeTillExit)
            {
                stateMachine.ChangeState(enemy.chaseState);
            }
        }
        else
        {
            _exitTimer = 0f;
        }
        timer += Time.deltaTime;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
