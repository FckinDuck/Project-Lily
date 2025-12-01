using System.Collections;
using UnityEngine;


public class EnemyStrikeDistanceCheck : MonoBehaviour
{

    public GameObject PlayerTarget { get; set; }
    private EmemyHealth _enemy;

    private void Awake()
    {
        PlayerTarget = GameObject.FindGameObjectWithTag("Player");
        _enemy = GetComponentInParent<EmemyHealth>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerTarget)
        {
            _enemy.SetWithinStrikeDistance(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerTarget)
        {
            _enemy.SetWithinStrikeDistance(false);
        }
    }
}
