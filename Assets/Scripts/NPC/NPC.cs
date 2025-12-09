using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public abstract class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer _interactSprite;

    private Transform _playerTransform;
    private const float INTERACT_DISTANCE = 5f;

    public GameObject Player { get ; set; }
    public bool CanInteract { get; set; }

    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        if (InputManager.interactPressed && IsInInteractRange())
        {
            Interact();
        }
        if (_interactSprite.gameObject.activeSelf && !IsInInteractRange())
        {
            //deactive
            _interactSprite.gameObject.SetActive(false);
        }

        else if (!_interactSprite.gameObject.activeSelf && IsInInteractRange())
        {
            //active
            _interactSprite.gameObject.SetActive(true);
        }
    }
    public abstract void Interact();
   

    private bool IsInInteractRange()
    {
        if (Vector2.Distance(_playerTransform.position,transform.position) < INTERACT_DISTANCE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
