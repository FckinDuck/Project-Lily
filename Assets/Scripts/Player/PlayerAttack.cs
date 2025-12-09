using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask attackableTargetLayer;
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float attackCooldown = 0.15f;



    private RaycastHit2D[] hits;

    private Animator anim;

    private float attackTimeCounter;

    private List<IDamageable> iDamageables = new List<IDamageable>();
    private List<IDeflectable> iDeflectables = new List<IDeflectable>();

    public bool ShouldBeDamage {  get; set; }=false;


    private void Start()
    {
        anim = GetComponent<Animator>();

        attackTimeCounter = attackCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.attackPressed && attackTimeCounter >= attackCooldown)
        {

            //Attack();
            anim.SetTrigger("attack");
            attackTimeCounter = 0;
        }
            attackTimeCounter += Time.deltaTime;
    }

    public IEnumerator DamageWhileSlashActive ()
    {
        ShouldBeDamage = true;

        while(ShouldBeDamage)
        {
            hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableTargetLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                IDamageable iDamageable = hits[i].collider.gameObject.GetComponent<IDamageable>();
                if (iDamageable != null && !iDamageable.HasTakenDamage)
                {
                    iDamageable.Damage(damageAmount, transform.right);
                    iDamageables.Add(iDamageable);
                }

                IDeflectable deflectable = hits[i].collider.gameObject.GetComponent<IDeflectable>();
                if (deflectable != null && !iDeflectables.Contains(deflectable))
                {
                    deflectable.Deflect(transform.right);
                    iDeflectables.Add(deflectable);
                }

            }

            yield return null;
        }
        ReturnAttackableToDamageableAndDeflectable(); 
    }


    /*
    private void Attack()
    {
        hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableTargetLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            IDamageable iDamageable = hits[i].collider.gameObject.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                iDamageable.Damage(damageAmount);
            }

        }
    }
    */
    private void ReturnAttackableToDamageableAndDeflectable()
    {
        foreach(IDamageable thingThatWasDamage in iDamageables)
        {
            thingThatWasDamage.HasTakenDamage =false;
        }
        iDamageables.Clear();
        iDeflectables.Clear();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackTransform.position,attackRange);
    }
    #region Animation Trigger
    public void ShouldBeDamageTrue()
    {
        ShouldBeDamage = true;

    }
    public void ShouldBeDamageFalse()
    {
        ShouldBeDamage = false;

    }
    #endregion
}
