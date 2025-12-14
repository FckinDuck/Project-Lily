using UnityEngine;

public class EnemyAggroCheck : MonoBehaviour
{
    public GameObject PlayerTarget { get; set; }

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float rayDistance = 20f;

    private EmemyHealth _enemy;
    private Collider2D _playerCollider;

    private void Awake()
    {
        PlayerTarget = GameObject.FindGameObjectWithTag("Player");
        _enemy = GetComponentInParent<EmemyHealth>();
        _playerCollider = PlayerTarget.GetComponent<Collider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject != PlayerTarget)
            return;

        // Check player hidden
        var stealth = PlayerTarget.GetComponent<PlayerStealth>();
        if (stealth != null && stealth.IsHidden)
        {
            _enemy.SetIsAggroed(false);
            return;
        }

        int visibleRayCount = CheckVisibilityRays();

        _enemy.SetIsAggroed(visibleRayCount >= 2);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerTarget)
        {
            _enemy.SetIsAggroed(false);
        }
    }

    private int CheckVisibilityRays()
    {
        int hitCount = 0;

        Vector2 enemyPos = transform.position;
        Bounds bounds = _playerCollider.bounds;

        Vector2 top = new(bounds.center.x, bounds.max.y);
        Vector2 center = bounds.center;
        Vector2 bottom = new(bounds.center.x, bounds.min.y);

        if (CastRay(enemyPos, top)) hitCount++;
        if (CastRay(enemyPos, center)) hitCount++;
        if (CastRay(enemyPos, bottom)) hitCount++;

        return hitCount;
    }

    private bool CastRay(Vector2 origin, Vector2 target)
    {
        Vector2 dir = (target - origin).normalized;
        float dist = Vector2.Distance(origin, target);

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist, obstacleLayer);

        Debug.DrawRay(origin, dir * dist, hit ? Color.red : Color.green);

        return hit.collider == null;
    }
}
