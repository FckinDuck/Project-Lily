using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] private Rigidbody2D bulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float shootInterval = 2f;
    [SerializeField] private string shootAnimationTrigger;

    private float shootTimer;
    private EnemyProjectile enemyProjectile;
    private EmemyHealth ememy;
    private PlayerHealth target;
    private Collider2D coll;
    private Rigidbody2D bulletRb;
    private Animator animator;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        ememy = GetComponent<EmemyHealth>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval && ememy.IsAggroed && !target.IsDead)
        {
            Shoot();
            shootTimer = 0f;
        }
    }
    private void Shoot()
    {
        
        // Trigger animation 
        if (!string.IsNullOrEmpty(shootAnimationTrigger) && animator != null)
        {
            animator.SetTrigger(shootAnimationTrigger);
        }

        //spawn projectile
        bulletRb = Instantiate(bulletPrefab, transform.position, transform.rotation);

        //set projectile direction
        bulletRb.transform.right = GetTargetDirection();

        //set projectile velocity
        bulletRb.linearVelocity = bulletRb.transform.right * bulletSpeed;

        //ive projectile reference to enemy collider to ignore first collision
        enemyProjectile = bulletRb.gameObject.GetComponent<EnemyProjectile>();

        //set collider
        enemyProjectile.enemyColl = coll;
    }

    public Vector2 GetTargetDirection()
    {
        Transform playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        return (playerTrans.position - transform.position).normalized;
    }
}
