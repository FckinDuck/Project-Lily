using System.Collections;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private Vector3 _targetPos;
    private Vector3 _dirction;

    private bool _isWaiting = false;
    private float _waitTimer = 0f;
    private float _waitDuration = 0f;


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

        //_targetPos = GetRandomPoint();

        _targetPos = enemy.patrolPointA != null
            ? enemy.patrolPointA.position
            : enemy.transform.position;
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

        if (_isWaiting)
        {
            _waitTimer += Time.deltaTime;
            Debug.Log("Wait for " + _waitDuration + " seconds before continue patrol");
            if (_waitTimer >= _waitDuration)
            {
                _isWaiting = false;
            }
            else
            {
                enemy.Move(Vector2.zero);
                // Trigger Idle animation while waiting
                enemy.AnimationTrigger(EmemyHealth.AnimationTriggersType.Idle);
                return;
            }
        }

        _dirction = (_targetPos - enemy.transform.position).normalized;
        enemy.Move(_dirction * enemy.randomMoveSpeed);
        enemy.HandleJump(enemy.randomMoveSpeed);

        // Trigger Move animation when moving
        AnimationTrigger(EmemyHealth.AnimationTriggersType.Move);

        if (enemy.patrolPointA != null)
        {
            if ((enemy.transform.position - _targetPos).sqrMagnitude < 1f)
            {
                _isWaiting = true;
                _waitTimer = 0f;
                _waitDuration = Random.Range(enemy.randomWaitTimeMin, enemy.randomWaitTimeMax);

                _targetPos = (_targetPos == enemy.patrolPointA.position)
                    ? enemy.patrolPointB.position
                    : enemy.patrolPointA.position;

                Debug.Log("patrol target change");
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
