using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyChaseState : EnemyState
{
    private Transform _playerTransform;
    private float _moveSpeed = 7f;
    private float _chaseDuration = 30f;
    private float _chaseTimer = 0f;

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
        enemy.HandleJump(_moveSpeed);
        if (enemy.IsWithinStrikeDistance)
        {
            enemy.stateMachine.ChangeState(enemy.attackState);
        }
        else
        {
            if (!enemy.IsWithinAggroDistance)
            {
                _chaseTimer += Time.deltaTime;

                if (_chaseTimer >= _chaseDuration)
                {
                    _chaseTimer = 0f;
                    stateMachine.ChangeState(enemy.idleState);
                }
                Debug.Log("Player outside aggro distance; Chasing the player untill de-aggro");

            }
            else
            {
                _chaseTimer = 0f; // Reset chase timer if player is within aggro distance
                Debug.Log("Player inside aggro distance; Chasing the player");

            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
