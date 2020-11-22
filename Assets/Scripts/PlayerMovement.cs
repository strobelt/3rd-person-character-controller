using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float MovementSpeed = 5.0f;
    public float RotationSpeed = 100;
    public float JumpHeight = 5.0f;
    public float GravityAcceleration = -9.81f;
    public float MaxJumpTime = 5;

    public GameObject CameraTarget;
    [Range(0, 360)] public float CameraMaxUpwardAngle = 65;
    [Range(0, 360)] public float CameraMaxDownwardAngle = 320;

    [HideInInspector] public Vector2 MovementVector;
    [HideInInspector] public Vector2 LookVector;
    [HideInInspector] public bool IsJumping;

    [HideInInspector] public float JumpTimer;

    //[HideInInspector] public CharacterController Controller;
    [HideInInspector] public ICharacterControllerWrapper CharacterControllerWrapper;
    [HideInInspector] public Vector3 PlayerVelocity;

    private const float MovementThreshold = 1.0f;


    public float RotationDelta => RotationSpeed * Time.deltaTime;
    public bool ShouldMove => PlayerVelocity.magnitude > MovementThreshold;

    private void Start()
    {
        CharacterControllerWrapper = new CharacterControllerWrapper
        {
            CharacterController = GetComponent<CharacterController>()
        };
    }

    void Update() => HandlePlayerMovement();

    public void Move(InputAction.CallbackContext context) =>
        HandleMovementInput(context.ReadValue<Vector2>(), context.started);

    public void HandleMovementInput(Vector2 inputVector, bool startedMoving)
    {
        MovementVector = inputVector;
        if (startedMoving) FaceForward();
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
        => HandleLookInput(context.ReadValue<Vector2>());

    public void HandleLookInput(Vector2 inputVector)
        => LookVector = inputVector;

    public void Jump(InputAction.CallbackContext context)
        => HandleJumpInput(!context.canceled);

    public void HandleJumpInput(bool isJumping)
    {
        IsJumping = isJumping;

        if (!isJumping) JumpTimer = 0;
    }

    public void HandlePlayerMovement()
    {
        PlayerVelocity = CalculateHorizontalVelocity(MovementVector.x, MovementVector.y);

        if (CharacterControllerWrapper.IsGrounded())
        {
            if (IsJumping) ResetJumpTimer();
            StopFalling();
        }

        if (IsJumping)
        {
            if (JumpTimer > 0)
                Jump();
            else
                IsJumping = false;
        }

        Fall();

        if (ShouldMove)
        {
            RotatePlayer();
            CharacterControllerWrapper.Move(PlayerVelocity * Time.deltaTime);
        }
        else
            RotateCamera(LookVector.x, LookVector.y);
    }

    private Vector3 CalculateHorizontalVelocity(float hInput, float vInput)
    {
        var horizontalInput = transform.right * hInput + transform.forward * vInput;
        var horizontalVelocity = horizontalInput * MovementSpeed;
        horizontalVelocity.y = PlayerVelocity.y;

        return horizontalVelocity;
    }

    private void ResetJumpTimer() => JumpTimer = MaxJumpTime;

    private void Jump()
    {
        PlayerVelocity.y = Mathf.Log(JumpHeight * -MovementSpeed * GravityAcceleration);
        JumpTimer -= Time.deltaTime;
    }

    private void StopFalling()
    {
        if (PlayerVelocity.y < 0) PlayerVelocity.y = 0;
    }

    private void Fall() => PlayerVelocity.y += GravityAcceleration * Time.deltaTime;

    private void RotatePlayer()
    {
        var playerRotation = transform.rotation.eulerAngles;
        playerRotation += Vector3.up * LookVector.x * RotationDelta;
        transform.rotation = Quaternion.Euler(playerRotation);

        RotateCamera(0, LookVector.y);
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