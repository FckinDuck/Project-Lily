using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool jumpPressed;
    public static bool jumpReleased;
    public static bool JumpHeld;
    public static bool RunHeld;

    public static bool Pause;

    public static bool attackPressed;

    public static bool interactPressed;

    private InputAction _moveAction;
    private InputAction _attackAction;
    private InputAction _jumpAction;
    private InputAction _runAction;

    private InputAction _pauseGame;
    private InputAction _interact;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];
        _attackAction = PlayerInput.actions["Attack"];
        _pauseGame = PlayerInput.actions["Cancel"];
        _interact = PlayerInput.actions["Interact"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        jumpPressed = _jumpAction.WasPressedThisFrame();
        jumpReleased = _jumpAction.WasReleasedThisFrame();
        JumpHeld = _jumpAction.IsPressed();

        RunHeld = _runAction.IsPressed();

        attackPressed = _attackAction.WasPressedThisFrame();

        interactPressed = _interact.WasPressedThisFrame();

        Pause = _pauseGame.WasPressedThisFrame();
    }

}

