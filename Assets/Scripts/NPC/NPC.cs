using UnityEngine;

public abstract class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer _interactSprite;

    private Transform _playerTransform;
    private const float INTERACT_DISTANCE = 3f;

    public GameObject Player { get ; set; }
    public bool CanInteract { get; set; }

    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        if (_playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _playerTransform = playerObject.transform;
            }
            else
            {
                return; // Skip 
            }
        }

        if (InputManager.interactPressed && IsInInteractRange())
        {
            Interact();
        }
        if (_interactSprite.gameObject.activeSelf && !IsInInteractRange())
        {
            //deactive
            _interactSprite.gameObject.SetActive(false);
            CanInteract = false;
        }

        else if (!_interactSprite.gameObject.activeSelf && IsInInteractRange())
        {
            //active
            _interactSprite.gameObject.SetActive(true);
            CanInteract = true;
        }
    }
    public abstract void Interact();
   

    private bool IsInInteractRange()
    {

        // Debug.Log("From NPC IsInInteractRange() "+_playerTransform.position);
        // Debug.Log("From NPC IsInInteractRange() "+ transform.position);
        if ((_playerTransform.position - transform.position).sqrMagnitude < INTERACT_DISTANCE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
