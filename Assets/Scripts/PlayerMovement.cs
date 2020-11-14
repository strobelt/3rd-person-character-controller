using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float MovementSpeed = 5.0f;
    public float RotationSpeed = 100;
    public float JumpHeight = 5.0f;
    public float GravityValue = -9.81f;
    public float MaxJumpTime = 5;

    public GameObject CameraTarget;
    [Range(0, 360)] public float CameraMaxUpwardAngle = 65;
    [Range(0, 360)] public float CameraMaxDownwardAngle = 320;

    private const float MovementThreshold = 1.0f;

    private CharacterController _controller;
    private Vector3 _playerVelocity;
    private Vector2 _movementVector;
    private Vector2 _lookVector;
    private bool _isJumping;
    private float _jumpTimer;

    private void Start() => _controller = GetComponent<CharacterController>();

    void Update() => HandlePlayerMovement();

    public void Move(InputAction.CallbackContext context)
    {
        _movementVector = context.ReadValue<Vector2>();
        if (context.started)
            FaceForward();
    }

    private void FaceForward()
    {
        var targetHorizontalRotation = CameraTarget.transform.localRotation.eulerAngles.x;

        var forward = new Vector3(CameraTarget.transform.forward.x, 0, CameraTarget.transform.forward.z);
        forward.Normalize();
        transform.forward = forward;

        CameraTarget.transform.localRotation = Quaternion.identity;
        CameraTarget.transform.Rotate(Vector3.right, targetHorizontalRotation, Space.Self);
    }

    public void Look(InputAction.CallbackContext context)
        => _lookVector = context.ReadValue<Vector2>();

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
            RotatePlayer();
            MovePlayer();
        }
        else
        {
            var rotationDelta = RotationSpeed * Time.deltaTime;
            var camRotation = CameraTarget.transform.rotation.eulerAngles;

            camRotation += Vector3.up * _lookVector.x * rotationDelta;

            var futureAngle = camRotation.x + _lookVector.y * rotationDelta;
            if (CameraMaxUpwardAngle >= futureAngle || futureAngle >= CameraMaxDownwardAngle)
                camRotation += Vector3.right * _lookVector.y * rotationDelta;

            CameraTarget.transform.rotation = Quaternion.Euler(camRotation);
        }
    }

    private void RotatePlayer()
    {
        var rotationDelta = RotationSpeed * Time.deltaTime;

        var playerRotation = transform.rotation.eulerAngles;
        playerRotation += Vector3.up * _lookVector.x * rotationDelta;
        transform.rotation = Quaternion.Euler(playerRotation);

        var camRotation = CameraTarget.transform.rotation.eulerAngles;
        var futureAngle = camRotation.x + _lookVector.y * rotationDelta;
        if (CameraMaxUpwardAngle >= futureAngle || futureAngle >= CameraMaxDownwardAngle)
            camRotation += Vector3.right * _lookVector.y * rotationDelta;
        CameraTarget.transform.rotation = Quaternion.Euler(camRotation);
    }

    private void MoveHorizontally(float hInput, float vInput)
    {
        var horizontalInput = transform.right * hInput + transform.forward * vInput;
        var horizontalVelocity = horizontalInput * MovementSpeed;

        _playerVelocity = new Vector3(horizontalVelocity.x, _playerVelocity.y, horizontalVelocity.z);
    }

    private void ResetJumpTimer() => _jumpTimer = MaxJumpTime;

    private void Jump()
    {
        _playerVelocity.y = Mathf.Log(JumpHeight * -MovementSpeed * GravityValue);
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