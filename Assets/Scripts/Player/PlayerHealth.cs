using NUnit.Framework.Internal.Builders;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 5f;
    [SerializeField] private ParticleSystem damageParticle;
    private ParticleSystem damageParticleInstance;
    public bool HasTakenDamage { get; set ; }
    private bool isDead = false;

    private float currentHealth;
    private Animator anim;
    private void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }
    public void Damage(float damageAmount, Vector2 attackDirection)
    {
        HasTakenDamage = true;
        currentHealth -= damageAmount;
        spawnParticle(attackDirection);
        anim.SetTrigger("Hited");
        //SoundFXManager.instance.PlayRandomSoundFX(damageSoundClip, transform, 1f);

        //healthBar.UpdateHealthBar(maxHealth, currentHealth);

        if (currentHealth <= 0)
        {
            if (!isDead)
                Die();
        }
    }
    public void Die()
    {
        GetComponent<Player>().enabled = false;
        anim.SetTrigger("Dead");
        isDead = true;
    }

    private void spawnParticle(Vector2 attackDiresction)
    {
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector2.right, attackDiresction);
        damageParticleInstance = Instantiate(damageParticle, transform.position, spawnRotation);
    }
}
