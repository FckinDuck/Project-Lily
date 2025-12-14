using UnityEngine;

public class bush : NPC, IDamageable
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private ParticleSystem damageParticle;
    [SerializeField] private AudioClip[] damageSoundClip;

    private PlayerStealth Stealth;

    private float currentHealth;

    private ParticleSystem damageParticleInstance;


    public bool HasTakenDamage { get; set; }

    void Start()
    {
        currentHealth = maxHealth;
        Stealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStealth>();
    }


    public void Damage(float damageAmount, Vector2 attackDiresction )
    {
        HasTakenDamage = true;
        currentHealth -= 1f;

        spawnParticle(attackDiresction);
        SoundFXManager.instance.PlayRandomSoundFX(damageSoundClip, transform, 1f);

        if (currentHealth <= 0)
        {
            spawnParticle(attackDiresction);
            spawnParticle(attackDiresction);
            spawnParticle(attackDiresction);
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

    public override void Interact()
    {
        // Debug.Log("Bush interacted");
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            return;
        }
        if (Stealth == null)
        {
            Stealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStealth>();
            return;
        }

        Stealth.IsHidden = !Stealth.IsHidden;
    }
}
