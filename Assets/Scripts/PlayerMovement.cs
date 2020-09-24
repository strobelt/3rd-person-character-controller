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
    private Camera _mainCamera;
    private Vector3 _playerVelocity;
    private Vector2 _movementVector;
    private bool _isJumping;
    private float _jumpTimer;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
    }

    void Update() => HandlePlayerMovement();

    public void Move(InputAction.CallbackContext context)
        => _movementVector = context.ReadValue<Vector2>();

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
            _isJumping = true;

        if (context.canceled)
            _isJumping = false;
    }

    void HandlePlayerMovement()
    {
        CalculateHMovementRelativeToCam(_movementVector.x, _movementVector.y);

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
            MovePlayer();
    }

    private void CalculateHMovementRelativeToCam(float hInput, float vInput)
    {
        var camForward = _mainCamera.transform.forward;
        var camRight = _mainCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        var horizontalInput = camRight * hInput + camForward * vInput;
        var horizontalVelocity = horizontalInput * PlayerSpeed;

        if (horizontalInput != Vector3.zero) transform.forward = horizontalInput;

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