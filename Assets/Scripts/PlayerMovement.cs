using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float PlayerSpeed = 5.0f;
    public float JumpHeight = 5.0f;
    public float GravityValue = -9.81f;

    private CharacterController _controller;
    private Camera _mainCamera;
    private Vector3 _playerVelocity;
    private readonly float _movementThreshold = 1.0f;
    private Vector2 _movementVector;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
    }

    void Update() => MovePlayer();

    public void Move(InputAction.CallbackContext context)
        => _movementVector = context.ReadValue<Vector2>();

    void MovePlayer()
    {
        var hInput = _movementVector.x;
        var vInput = _movementVector.y;

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

        if (_playerVelocity.magnitude > _movementThreshold)
            _controller.Move(_playerVelocity * Time.deltaTime);
    }

    void OldUpdate()
    {
        var camForward = _mainCamera.transform.forward;
        var camRight = _mainCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        var hInput = 0;
        var vInput = 0;
        var horizontalInput = camRight * hInput + camForward * vInput;
        var horizontalVelocity = horizontalInput * PlayerSpeed;

        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, _mainCamera.transform.position,
            PlayerSpeed * Time.deltaTime);

        if (horizontalInput != Vector3.zero) transform.forward = horizontalInput;

        _playerVelocity = new Vector3(horizontalVelocity.x, _playerVelocity.y, horizontalVelocity.z);

        if (_controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
                _playerVelocity.y += Mathf.Log(JumpHeight * -PlayerSpeed * GravityValue);

            if (_playerVelocity.y < 0) _playerVelocity.y = 0;
        }

        _playerVelocity.y += GravityValue * Time.deltaTime;

        if (_playerVelocity.magnitude > _movementThreshold)
            _controller.Move(_playerVelocity * Time.deltaTime);
    }
}