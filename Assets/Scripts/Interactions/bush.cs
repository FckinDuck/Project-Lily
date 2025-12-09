using UnityEngine;

public class bush : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private ParticleSystem damageParticle;
    [SerializeField] private AudioClip[] damageSoundClip;
    // Start is called  once before the first execution of Update after the MonoBehaviour is created

    private float currentHealth;

    private ParticleSystem damageParticleInstance;


    public bool HasTakenDamage { get; set; }

    void Start()
    {
        currentHealth = maxHealth;
    }


    public void Damage(float damageAmount, Vector2 attackDiresction )
    {
        HasTakenDamage = true;
        currentHealth -= damageAmount;

        spawnParticle(attackDiresction);
        SoundFXManager.instance.PlayRandomSoundFX(damageSoundClip, transform, 1f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void spawnParticle(Vector2 attackDiresction)
    {
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector2.right,attackDiresction);
        damageParticleInstance = Instantiate(damageParticle, transform.position, spawnRotation);
    }

    
}
