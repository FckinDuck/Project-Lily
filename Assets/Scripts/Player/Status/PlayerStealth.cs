using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
    public bool IsHidden = false;

    private SpriteRenderer spriteRenderer;
    private Player playerScript;
    private float originalAlpha;
    private int originalSortingOrder;
    [SerializeField] private int hiddenSortingOrder = 0; // Lower than bush

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerScript = GetComponent<Player>();
        if (spriteRenderer != null)
        {
            originalAlpha = spriteRenderer.color.a;
            originalSortingOrder = spriteRenderer.sortingOrder;
        }
    }

    private void Update()
    {
        if (spriteRenderer == null || playerScript == null) return;

        if (IsHidden)
        {
            // Make player semi-transparent
            var color = spriteRenderer.color;
            color.a = 0.4f;
            spriteRenderer.color = color;

            // Render behind objects
            spriteRenderer.sortingOrder = hiddenSortingOrder;

            // Prevent movement but allow rotation
            playerScript.enabled = false;
            // Allow manual rotation if needed (e.g., via keys or mouse)
        }
        else
        {
            // Restore appearance
            var color = spriteRenderer.color;
            color.a = originalAlpha;
            spriteRenderer.color = color;

            // Restore sorting order
            spriteRenderer.sortingOrder = originalSortingOrder;

            // Enable movement
            playerScript.enabled = true;
        }
    }
}
