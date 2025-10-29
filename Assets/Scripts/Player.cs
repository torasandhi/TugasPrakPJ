using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Controls _controls;

    private Vector2 _moveInput;
    private float _playerSpeed = 5.0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Ensure gravity is off for a top-down game
        _rb.gravityScale = 0;

        // Get the controls instance from your singleton
        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController singleton not found! Make sure it's in your scene.");
            return;
        }
        _controls = PlayerController.Instance.GetCurrentInput();

        // Subscribe to the input actions
        if (_controls != null)
        {
            _controls.Player.Move.performed += OnMovePerformed;
            _controls.Player.Move.canceled += OnMoveCanceled;
        }
        else
        {
            Debug.LogError("Controls object is null! Check PlayerController.cs");
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    void FixedUpdate()
    {
        // --- 2D Movement ---
        // We set the velocity directly. This is great for responsive top-down movement.
        Vector2 moveVelocity = _moveInput.normalized * _playerSpeed;
        _rb.linearVelocity = moveVelocity;

        // --- 2D Rotation ---
        // If the player is moving, rotate them to face that direction.
        // This assumes your sprite's "forward" is the 'up' direction.
        if (moveVelocity != Vector2.zero)
        {
            // Calculate the angle in degrees
            // Atan2 gives the angle in radians, Rad2Deg converts it
            // We subtract 90 degrees because 0 degrees in Atan2 is 'right', but our sprite faces 'up'
            float angle = Mathf.Atan2(moveVelocity.y, moveVelocity.x) * Mathf.Rad2Deg - 90f;

            // Apply the rotation
            _rb.rotation = angle;
        }
    }

    // Unsubscribe when the object is destroyed
    private void OnDestroy()
    {
        if (_controls != null)
        {
            _controls.Player.Move.performed -= OnMovePerformed;
            _controls.Player.Move.canceled -= OnMoveCanceled;
        }
    }
}

