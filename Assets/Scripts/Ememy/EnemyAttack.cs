using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemyAttackData
{
    [Header("Info")]
    public string attackName;
    public string animationTrigger;

    [Header("Hit")]
    public Transform attackTransform;
    public float attackRange = 1.5f;
    public float damage = 1f;

    [Header("Range Trigger")]
    public AttackRangeTrigger rangeTrigger;

    [Header("Cooldown")]
    public float cooldown;

    [HideInInspector] public float lastUsedTime;

    public bool CanUseAttack =>
        rangeTrigger != null && rangeTrigger.IsTargetInside;
}


public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Check Cooldown")]
    [SerializeField] private float checkAttackCooldown = 0.3f;
    private float lastCheckTime;

    [Header("Attacks")]
    [SerializeField] private EnemyAttackData[] attacks;
    [SerializeField] private LayerMask attackableTargetLayer;

    private Animator anim;
    private EnemyAttackData currentAttack;
    private bool shouldBeDamage;
    public bool IsAttacking;

    private readonly List<IDamageable> damaged = new();

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    #region ATTACK FOR STATE TO CALL

    public bool CanCheckAttack()
    {
        return Time.deltaTime >= lastCheckTime + checkAttackCooldown;
    }

    public EnemyAttackData GetValidAttack()
    {
        lastCheckTime = Time.deltaTime;

        List<EnemyAttackData> valid = new();

        foreach (var atk in attacks)
        {
            if (!atk.CanUseAttack)
                continue;

            if (Time.deltaTime < atk.lastUsedTime + atk.cooldown)
                continue;

            valid.Add(atk);
        }

        if (valid.Count == 0)
            return null;

        return valid[Random.Range(0, valid.Count)];
    }


    public void ExecuteAttack(EnemyAttackData attack)
    {
        if (attack == null) return;

        currentAttack = attack;
        IsAttacking = true;
        attack.lastUsedTime = Time.time;
        if (anim != null && !string.IsNullOrEmpty(attack.animationTrigger))
            anim.SetTrigger(attack.animationTrigger);
    }

    #endregion

    #region DAMAGE 

    public IEnumerator DamageWhileAttackActive()
    {
        shouldBeDamage = true;

        while (shouldBeDamage)
        {
            var hits = Physics2D.CircleCastAll(
                currentAttack.attackTransform.position,
                currentAttack.attackRange,
                Vector2.zero,
                0f,
                attackableTargetLayer
            );

            foreach (var hit in hits)
            {
                IDamageable dmg = hit.collider.GetComponent<IDamageable>();
                if (dmg != null && !dmg.HasTakenDamage)
                {
                    dmg.Damage(currentAttack.damage, transform.right);
                    damaged.Add(dmg);
                }
            }

            yield return null;
        }

        ResetDamageables();
    }
    #endregion

    #region HELPERS FOR ANIMATION EVENTS
    public void EnableDamage()
    {
        StartCoroutine(DamageWhileAttackActive());
    }

    public void DisableDamage()
    {
        shouldBeDamage = false;
        currentAttack = null;
    }

    private void ResetDamageables()
    {
        foreach (var d in damaged)
            d.HasTakenDamage = false;

        damaged.Clear();
        IsAttacking = false;
    }

    #endregion

    #region DEBUGGING
    private void OnDrawGizmosSelected()
    {
        if (attacks == null) return;

        Gizmos.color = Color.red;
        foreach (var atk in attacks)
        {
            if (atk.attackTransform != null)
                Gizmos.DrawWireSphere(atk.attackTransform.position, atk.attackRange);
        }
    }
    #endregion
}
