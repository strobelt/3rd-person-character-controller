using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float PlayerSpeed = 5.0f;
    public float JumpHeight = 5.0f;
    public float GravityValue = -9.81f;
    public float MaxJumpTime = 5;

    private const float MovementThreshold = 1.0f;

    private CharacterController _controller;
    private Vector3 _playerVelocity;
    private Vector2 _movementVector;
    private bool _isJumping;
    private float _jumpTimer;

    private void Start() => _controller = GetComponent<CharacterController>();

    void Update() => HandlePlayerMovement();

    public void Move(InputAction.CallbackContext context)
        => _movementVector = context.ReadValue<Vector2>();

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
            _isJumping = true;

        if (context.canceled)
        {
            _jumpTimer = 0;
            _isJumping = false;
        }
    }

    void HandlePlayerMovement()
    {
        MoveHorizontally(_movementVector.x, _movementVector.y);

        if (_controller.isGrounded)
        {
            if (_isJumping) ResetJumpTimer();
            StopFalling();
        }

        if (_isJumping)
        {
            if (_jumpTimer > 0)
                Jump();
            else
                _isJumping = false;
        }

        Fall();

        if (ShouldMove())
        {
            var cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;
            transform.forward = cameraForward;
            MovePlayer();
        }
    }

    private void MoveHorizontally(float hInput, float vInput)
    {
        var horizontalInput = transform.right * hInput + transform.forward * vInput;
        var horizontalVelocity = horizontalInput * PlayerSpeed;

        _playerVelocity = new Vector3(horizontalVelocity.x, _playerVelocity.y, horizontalVelocity.z);
    }

    private void ResetJumpTimer() => _jumpTimer = MaxJumpTime;

    private void Jump()
    {
        _playerVelocity.y = Mathf.Log(JumpHeight * -PlayerSpeed * GravityValue);
        _jumpTimer -= Time.deltaTime;
    }

    private void StopFalling()
    {
        if (_playerVelocity.y < 0) _playerVelocity.y = 0;
    }

    private void Fall() => _playerVelocity.y += GravityValue * Time.deltaTime;

    private bool ShouldMove() => _playerVelocity.magnitude > MovementThreshold;

    private void MovePlayer() => _controller.Move(_playerVelocity * Time.deltaTime);
}