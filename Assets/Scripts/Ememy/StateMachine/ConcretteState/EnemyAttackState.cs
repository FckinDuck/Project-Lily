using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private EnemyAttack enemyAttack;
    private Transform target;

    private float _exitTimer;
    private float _timeTillExit = 3f;

    public EnemyAttackState(EmemyHealth enemy, EnemyStateMachine stateMachine)
        : base(enemy, stateMachine)
    {
        enemyAttack = enemy.GetComponent<EnemyAttack>();
    }

    public override void EnterState()
    {
        base.EnterState();
        _exitTimer = 0f;
        
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
        else Debug.Log("Cant get player target from attack state");
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        enemy.Move(Vector2.zero);
        if (enemyAttack!=null)
        {
            if (enemyAttack.IsAttacking)
                return;
       
            if (!enemyAttack.CanCheckAttack())
                return;

        } else
        {
            Debug.Log("EnemyAttack component is null in EnemyAttackState");
            return;
        }

        EnemyAttackData attack = enemyAttack.GetValidAttack();

        if (attack != null)
        {
            enemyAttack.ExecuteAttack(attack);
        }

        // ===== EXIT LOGIC =====
        if (!enemy.IsWithinStrikeDistance)
        {
            _exitTimer += Time.deltaTime;
            if (_exitTimer >= _timeTillExit)
                stateMachine.ChangeState(enemy.chaseState);
        }
        else
        {
            _exitTimer = 0f;
        }
    }

}
