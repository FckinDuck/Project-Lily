using UnityEngine;

public class AttackRangeTrigger : MonoBehaviour
{
    public bool IsTargetInside { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            IsTargetInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            IsTargetInside = false;
    }
}
