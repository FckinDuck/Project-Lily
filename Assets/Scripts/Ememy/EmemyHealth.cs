using UnityEngine;

public class EmemyHealth : MonoBehaviour, IDamageable, IEmnemyMoveable, ITriggerCheckable
{

    [SerializeField] private float maxHealth =3f ;
    [SerializeField] private ParticleSystem damageParticle;
    [SerializeField] private AudioClip[] damageSoundClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float currentHealth;

    private HealthBar healthBar;

    private ParticleSystem damageParticleInstance;


    public bool HasTakenDamage {  get; set; }
    public Rigidbody2D rb { get; set; }
    public bool IsFacingRight { get; set; } = true;

    #region StateMachine Variables

    public EnemyStateMachine stateMachine { get; set; }
    public EnemyAttackState attackState { get; set; }
    public EnemyChaseState chaseState { get; set; }
    public EnemyIdleState idleState { get; set; }
    public bool IsAggroed { get ; set; }
    public bool IsWithinStrikeDistance { get; set; }

    #endregion

    #region IdleState Variables

    public float randomMoveRange = 20f;
    public float randomMoveSpeed = 5f;


    #endregion
    private void Awake()
    {
        stateMachine = new EnemyStateMachine();
        attackState = new EnemyAttackState(this, stateMachine);
        chaseState = new EnemyChaseState(this, stateMachine);
        idleState = new EnemyIdleState(this, stateMachine);
    }
    private void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.currentState.FrameUpdate();
    }
    private void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    #region Damageable implementation
    public void Damage(float damageAmount, Vector2 attackDiresction)
    {
        HasTakenDamage = true;
        currentHealth -= damageAmount;
        spawnParticle(attackDiresction);

        SoundFXManager.instance.PlayRandomSoundFX(damageSoundClip, transform, 1f);

        healthBar.UpdateHealthBar(maxHealth,currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Particle System
    private void spawnParticle(Vector2 attackDiresction)
    {
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector2.right, attackDiresction);
        damageParticleInstance = Instantiate(damageParticle, transform.position, spawnRotation);
    }

    #endregion

    #region Moveable implementation
    public void Move(Vector2 velocity)
    {
        CheckLeftOrRightFacing(velocity);
        rb.linearVelocity = velocity;
    }

    public void CheckLeftOrRightFacing(Vector2 velocity)
    {
        if (IsFacingRight && velocity.x < 0f)
        {
            Vector3 rotate = new Vector3(transform.position.x, 180f, transform.position.z);
            transform.rotation = Quaternion.Euler(rotate);
            IsFacingRight = !IsFacingRight;
        }
        else if (!IsFacingRight && velocity.x > 0f)
        {
            Vector3 rotate = new Vector3(transform.position.x, 0f, transform.position.z);
            transform.rotation = Quaternion.Euler(rotate);
            IsFacingRight = !IsFacingRight;
        }
    }

    #endregion

    #region Animation Triggers

    private void AnimationTrigger(AnimationTriggersType triggersType)
    {
        // Implement animation trigger handling here
        stateMachine.currentState.AnimationTrigger(triggersType);
    }


    public enum AnimationTriggersType
    {
        TakeDamage,
        PlayFootStepSound,
        PlayAttackSound,
        Die
    }
    #endregion

    #region TriggerCheckable implementation
    public void SetIsAggroed(bool value)
    {
        IsAggroed = value;
    }

    public void SetWithinStrikeDistance(bool value)
    {
        IsWithinStrikeDistance = value;
    }
    #endregion
}
