using UnityEngine;

public class EnemyProjectile : MonoBehaviour, IDeflectable
{
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private AnimationCurve speedCurve;

    [field: SerializeField] public float DeflectSpeed { get; set; } = 30f;

    public Collider2D enemyColl { get; set; }
    public bool IsDeflected { get; set; } = false;

    private Collider2D coll;
    private Rigidbody2D rb;
    private float speed;
    private float time;

    private bool hasHitTarget = false;
    private float stuckDuration = 10f;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        IgnoreCollisionWithShooter();
    }

    private void FixedUpdate()
    {
        if (IsDeflected && !hasHitTarget)
        {
            speed = speedCurve.Evaluate(time);
            time += Time.fixedDeltaTime;

            rb.linearVelocity = transform.right * speed * DeflectSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHitTarget) return;

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            //damage
            damageable.Damage(damageAmount, transform.right);

            
            coll.enabled = false;

           
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;

            transform.SetParent(collision.transform);

            hasHitTarget = true;

            // 10s then Destroy
            Destroy(gameObject, stuckDuration);
        }
    }

    private void IgnoreCollisionWithShooter()
    {
        if (enemyColl == null || coll == null) return;
        Physics2D.IgnoreCollision(coll, enemyColl, true);
    }

    public void Deflect(Vector2 deflectDirection)
    {
        if (hasHitTarget) return;

        IsDeflected = true;

        
        Physics2D.IgnoreCollision(coll, enemyColl, false);

    
        if ((deflectDirection.x > 0 && transform.right.x < 0) ||
            (deflectDirection.x < 0 && transform.right.x > 0))
        {
            transform.right = -transform.right;
        }

        
        rb.linearVelocity = transform.right * DeflectSpeed;
    }
}
