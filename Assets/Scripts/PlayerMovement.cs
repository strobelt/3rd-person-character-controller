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
    [HideInInspector] public Vector2 _movementVector;
    private Vector2 _lookVector;
    private bool _isJumping;
    private float _jumpTimer;

    private float RotationDelta => RotationSpeed * Time.deltaTime;
    private bool ShouldMove => _playerVelocity.magnitude > MovementThreshold;

    private void Start() => _controller = GetComponent<CharacterController>();

    void Update() => HandlePlayerMovement();

    public void Move(InputAction.CallbackContext context) => HandleMovementInput(context.ReadValue<Vector2>(), context.started);

    public void HandleMovementInput(Vector2 inputVector, bool startedMoving)
    {
        _movementVector = inputVector;
        if (startedMoving)
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
        _playerVelocity = CalculateHorizontalVelocity(_movementVector.x, _movementVector.y);

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

        if (ShouldMove)
        {
            RotatePlayer();
            _controller.Move(_playerVelocity * Time.deltaTime);
        }
        else
            RotateCamera(_lookVector.x, _lookVector.y);
    }

    private Vector3 CalculateHorizontalVelocity(float hInput, float vInput)
    {
        var horizontalInput = transform.right * hInput + transform.forward * vInput;
        var horizontalVelocity = horizontalInput * MovementSpeed;
        horizontalVelocity.y = _playerVelocity.y;

        return horizontalVelocity;
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

    private void RotatePlayer()
    {
        var playerRotation = transform.rotation.eulerAngles;
        playerRotation += Vector3.up * _lookVector.x * RotationDelta;
        transform.rotation = Quaternion.Euler(playerRotation);

        RotateCamera(0, _lookVector.y);
    }

    private void RotateCamera(float hInput, float vInput)
    {
        var camRotation = CameraTarget.transform.rotation.eulerAngles;

        camRotation += Vector3.up * hInput * RotationDelta;

        var futureAngle = camRotation.x + vInput * RotationDelta;
        if (CameraMaxUpwardAngle >= futureAngle || futureAngle >= CameraMaxDownwardAngle)
            camRotation += Vector3.right * vInput * RotationDelta;

        CameraTarget.transform.rotation = Quaternion.Euler(camRotation);
    }
}