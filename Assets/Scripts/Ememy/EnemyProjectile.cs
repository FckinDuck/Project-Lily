using System.Net.Sockets;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour,IDeflectable
{
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private AnimationCurve speedCurve;
    private IDamageable damageable;

    [field:SerializeField] public float DeflectSpeed { get; set; } = 30f;

    public Collider2D enemyColl { get; set; }
    public bool IsDeflected { get; set; } = false;

    private Collider2D coll;
    private Rigidbody2D rb;
    private float speed;
    private float time;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        IgnoreCollisionWithShooter();
    }

    private void FixedUpdate()
    {
        if (IsDeflected)
        {
            speed = speedCurve.Evaluate(time);
            time += Time.fixedDeltaTime;

            rb.linearVelocity = transform.right * speed * DeflectSpeed;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(damageAmount,transform.right);
            //Destroy(gameObject);
        }
    }

    private void IgnoreCollisionWithShooter()
    {
        if (!Physics2D.GetIgnoreCollision(coll,enemyColl))
        {
            Physics2D.IgnoreCollision(coll, enemyColl, true);

        }
        else
        {
            Physics2D.IgnoreCollision(coll, enemyColl, false);
        }
    }

    public void Deflect(Vector2 deflectDirection)
    {
        //toggle state to deflected
        IsDeflected = true;

        //turn on collision for enemy collider
        IgnoreCollisionWithShooter();

        //check hit direction with current direction
        if((deflectDirection.x >0 && transform.right.x < 0)|| (deflectDirection.x < 0 && transform.right.x > 0))
        {
            transform.right = -transform.right;
        }

        //set new velocity
        rb.linearVelocity = transform.right * DeflectSpeed;
    }

}

