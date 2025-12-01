using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputs : MonoBehaviour
{
    public Controls control;
    public Vector2 MoveInput;
    public static UserInputs instance;
    /*
    public static PlayerInput PlayerInput;

    public static bool WasJumpPressed;
    public static bool IsJumpBeingPressed;
    public static bool WasJumpReleased;
    public static bool WasAttackPressed;
    public static bool WasDogdePressed;
    public static bool WasInteractPressed;

    private InputAction _moveAction;
    private InputAction _attackAction;
    private InputAction _jumpAction;
    private InputAction _dogdeAction;
    private InputAction _interactAction;
    */
    private void Awake()
    {
        /*
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _interactAction = PlayerInput.actions["Interact"];
        _attackAction = PlayerInput.actions["Attack"];
        _dogdeAction = PlayerInput.actions["Dogde"];
        */
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        control = new Controls();
        control.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();

    }

    private void Update()
    {
        /*
        MoveInput = _moveAction.ReadValue<Vector2>();

        WasAttackPressed = _attackAction.WasPressedThisFrame();
        WasDogdePressed = _dogdeAction.WasPressedThisFrame();
        WasInteractPressed = _interactAction.WasPressedThisFrame();
        WasJumpPressed = _jumpAction.WasPressedThisFrame();
        WasJumpReleased = _jumpAction.WasReleasedThisFrame();
        IsJumpBeingPressed = _jumpAction.IsPressed();
        */
    }

    private void OnEnable()
    {
        control.Enable();
    }
    private void OnDisable()
    {
        control.Disable();
    }
}
