using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float startMaxHealth = 10f;
    [SerializeField] private float capMaxHealth = 150f;

    [SerializeField] private float InvincibilityTimeAfterHit = 0.3f;

    [SerializeField] private ParticleSystem damageParticle;

    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Image healthBarCanister;

    [SerializeField] private Behaviour[] components;
    [SerializeField] private AudioClip[] damageSoundClip;

    private ParticleSystem damageParticleInstance;
    public bool HasTakenDamage { get; set ; }
    public bool IsDead => currentHealth <= 0f;

    private float currentHealth;
    private float takeHitTimer = 0f;

    private Animator anim;
    private void Start()
    {
        currentHealth = startMaxHealth;
        anim = GetComponent<Animator>();
        // Scale factor based on maxHealth
        float scale = Mathf.Clamp(startMaxHealth / 10f, 1f, 3f);

        // Adjust HealthBar size
        if (healthBar != null)
        {
            RectTransform barRect = healthBar.GetComponent<RectTransform>();
            if (barRect != null)
            {
                Vector2 size = barRect.sizeDelta;
                size.x = scale; // Set width directly to the clamped scale
                barRect.sizeDelta = size;
            }
        }

        // Adjust HealthBarCanister size
        if (healthBarCanister != null)
        {
            RectTransform canisterRect = healthBarCanister.GetComponent<RectTransform>();
            if (canisterRect != null)
            {
                Vector2 size = canisterRect.sizeDelta;
                size.x = scale; // Set width directly to the clamped scale
                canisterRect.sizeDelta = size;
            }
        }
    }
    private void FixedUpdate()
    {
        takeHitTimer+= Time.fixedDeltaTime;
        if (currentHealth> capMaxHealth)
        {
            currentHealth = capMaxHealth;
        }
    }
    public void Damage(float damageAmount, Vector2 attackDirection)
    {
        if (takeHitTimer > InvincibilityTimeAfterHit)
        { 
            HasTakenDamage = true;
            currentHealth -= damageAmount;
            spawnParticle(attackDirection);
            anim.SetTrigger("Hited");
            SoundFXManager.instance.PlayRandomSoundFX(damageSoundClip, transform, 1f);

            healthBar.UpdateHealthBar(startMaxHealth, currentHealth);
            takeHitTimer = 0f;

        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        anim.SetTrigger("Dead");
        foreach (Behaviour item in components)
        {
            item.enabled = false;
        }
    }

    private void spawnParticle(Vector2 attackDiresction)
    {
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector2.right, attackDiresction);
        damageParticleInstance = Instantiate(damageParticle, transform.position, spawnRotation);
    }
}
