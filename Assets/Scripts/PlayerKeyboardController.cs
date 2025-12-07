using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerKeyboardController : MonoBehaviour, ICharacterController
{
    private Vector2 moveInput;
    private bool bombPressed;

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        bombPressed = value.isPressed;
    }

    public Vector2 GetMoveDirection() => moveInput;

    public bool ShouldUseBomb()
    {
        bool pressed = bombPressed;
        bombPressed = false; // evita repetição
        return pressed;
    }
}
