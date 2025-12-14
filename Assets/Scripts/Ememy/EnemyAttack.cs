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

    [Header("Distance Condition")]
    public float minDistance;
    public float maxDistance;

    [Header("Cooldown")]
    public float cooldown;

    [HideInInspector] public float lastUsedTime;
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

    private readonly List<IDamageable> damaged = new();

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    #region ATTACK FOR STATE TO CALL

    public bool CanCheckAttack()
    {
        return Time.time >= lastCheckTime + checkAttackCooldown;
    }

    public EnemyAttackData GetValidAttack(float distance)
    {
        lastCheckTime = Time.time;

        List<EnemyAttackData> valid = new();

        foreach (var atk in attacks)
        {
            if (distance < atk.minDistance || distance > atk.maxDistance)
                continue;

            if (Time.time < atk.lastUsedTime + atk.cooldown)
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
        attack.lastUsedTime = Time.time;
        if (anim != null && !string.IsNullOrEmpty(attack.animationTrigger))
            anim.SetTrigger(attack.animationTrigger);
    }

    public bool IsAttacking => currentAttack != null;

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
