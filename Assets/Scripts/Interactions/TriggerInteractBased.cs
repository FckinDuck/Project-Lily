using Unity.VisualScripting;
using UnityEngine;

public class TriggerInteractBased : MonoBehaviour, IInteractable
{
    public GameObject Player { get ; set; }
    public bool CanInteract { get ; set; }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (CanInteract)
        {
            if (UserInputs.instance.control.Player.Interact.WasPressedThisFrame())
            {
                Interact();
                Debug.Log("Interact active");
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player)
        {
            CanInteract = true;
        }
    }
    private void OnTriggerExit2D (Collider2D collision)
    {
        if (collision.gameObject == Player)
        {
            CanInteract = false;
        }
    }

    public virtual void Interact()    {  }

}