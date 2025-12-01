using UnityEngine;
public interface IInteractable
{
    void Interact();
    GameObject Player { get; set; }
    bool CanInteract {  get; set; }
}

