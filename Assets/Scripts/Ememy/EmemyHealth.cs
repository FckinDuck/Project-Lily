using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EmemyHealth : MonoBehaviour, IDamageable, IEmnemyMoveable, ITriggerCheckable
{
    [Header("Health")]
    [SerializeField] private float maxHealth =3f ;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCoolDown = 5f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float obstacleCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Particle and Effects")]
    [SerializeField] private ParticleSystem damageParticle;
    [SerializeField] private AudioClip[] damageSoundClip;
    [SerializeField] private AudioClip[] footSoundClips;
    [SerializeField] private float footStepSoundInterval = 5f;

    [Header("Collision for checks")]
    [SerializeField] private Collider2D feetColl;
    [SerializeField] private Collider2D bodyColl;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float currentHealth;

    private Animator anim;

    private HealthBar healthBar;

    private float jumpTimer = 0f;

    private ParticleSystem damageParticleInstance;


    public bool HasTakenDamage {  get; set; }
    public Rigidbody2D rb { get; set; }
    public bool IsFacingRight { get; set; } = true;
    private bool IsGrounded;
    private bool IsThereObstacle;
    private float soundfxPlayTimer =0f;

    #region StateMachine Variables

    public EnemyStateMachine stateMachine { get; set; }
    public EnemyAttackState attackState { get; set; }
    public EnemyChaseState chaseState { get; set; }
    public EnemyIdleState idleState { get; set; }
    public bool IsAggroed { get ; set; }
    public bool IsWithinStrikeDistance { get; set; }
    public bool IsWithinAggroDistance { get; set; }

    #endregion

    #region IdleState Variables
    [Header("Idlestate Variable")]
    public float randomWaitTimeMin = 2f;
    public float randomWaitTimeMax = 10f;
    public float randomMoveSpeed = 5f;

    // Patrol points
    public Transform patrolPointA;
    public Transform patrolPointB;

    public Vector3 currentPatrolTarget;
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
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        
        stateMachine.currentState.FrameUpdate();
        GroundCheck();
        ObstacleCheck();
        soundfxPlayTimer += Time.deltaTime;
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
        anim.SetTrigger("hit");
        spawnParticle(attackDiresction);

        SoundFXManager.instance.PlayRandomSoundFX(damageSoundClip, transform, 1f);

        healthBar.UpdateHealthBar(maxHealth,currentHealth);

        StartCoroutine(ResetHasTakenDamage(1f));

        if (currentHealth <= 0)
        {
            anim.SetTrigger("dead");
            Die();
        }
    }

    // Add this coroutine to your EmemyHealth class
    private IEnumerator ResetHasTakenDamage(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        HasTakenDamage = false;
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
        //anim.SetBool("Move", true);
        FootstepSound();
        //SoundFXManager.instance.PlayRandomSoundFX(footSoundClip, transform, 0.75f);
    }

    public void CheckLeftOrRightFacing(Vector2 velocity)
    {
        if (IsFacingRight && velocity.x < 0f)
        {
            Vector3 rotate = new(0f, 180f, 0f);
            transform.rotation = Quaternion.Euler(rotate);
            IsFacingRight = !IsFacingRight;
        }
        else if (!IsFacingRight && velocity.x > 0f)
        {
            Vector3 rotate = new(0f, 0f, 0f);
            transform.rotation = Quaternion.Euler(rotate);
            IsFacingRight = !IsFacingRight;
        }
    }

    #endregion

    #region jump
    public void HandleJump(float velocity)
    {
        if (jumpTimer < jumpCoolDown)
        {
            jumpTimer += Time.deltaTime;
            return;
        }
        if (IsGrounded && IsThereObstacle)
        {
            rb.linearVelocity = new Vector2(velocity, jumpForce);
            jumpTimer = 0f;
        }
    }
    #endregion

    #region Animation Triggers

    public void AnimationTrigger(AnimationTriggersType triggersType)
    {
        switch (triggersType)
        {
            case AnimationTriggersType.TakeDamage:
                anim.SetTrigger("hit");
                break;

            case AnimationTriggersType.Die:
                anim.SetTrigger("dead");
                break;

            case AnimationTriggersType.Move:
                anim.SetBool("Move", true);
                break;

            case AnimationTriggersType.Idle:
                anim.SetBool("Move", false);
                break;
        }
    }


    public enum AnimationTriggersType
    {
        TakeDamage,
        PlayFootStepSound,
        PlayAttackSound,
        Die,
        Move,
        Idle
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

    public void GroundCheck()
    {
        IsGrounded = BoxCastDown(feetColl);
    }

    public void ObstacleCheck()
    {
        IsThereObstacle = BoxCastForward(bodyColl, Vector2.right);
    }

    public void FootstepSound()
    {
        StartCoroutine(FootstepSoundCoroutine());

    }
    private IEnumerator FootstepSoundCoroutine()
    {
        if (soundfxPlayTimer > footStepSoundInterval)
        {
            //float randomVolume = Random.Range(0.4f, 0.6f);
            SoundFXManager.instance.PlayRandomSoundFX(footSoundClips, transform, 0.5f);
            soundfxPlayTimer = 0f;
        }
        else
        {
            soundfxPlayTimer += Time.deltaTime;
            yield return null;
        }
    }
    #endregion

    #region Check caster
    private bool BoxCastForward(Collider2D coll, Vector2 direction)
    {
        Vector2 origin = coll.bounds.center;
        Vector2 size = new(coll.bounds.size.x * obstacleCheckDistance, coll.bounds.size.y );

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            size,
            0f,
            direction,
            obstacleCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }
    private bool BoxCastDown(Collider2D coll)
    {
        Vector2 origin = new(coll.bounds.center.x, coll.bounds.min.y);
        Vector2 size = new(coll.bounds.size.x, groundCheckDistance);

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            size,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    #endregion
}
