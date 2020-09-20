using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float PlayerSpeed = 5.0f;
    public float JumpHeight = 5.0f;
    public float GravityValue = -9.81f;

    private CharacterController controller;
    private Camera mainCamera;
    private Vector3 playerVelocity;
    private float movementThreshold = 1.0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        var camForward = mainCamera.transform.forward;
        var camRight = mainCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        var hInput = Input.GetAxis("Horizontal");
        var vInput = Input.GetAxis("Vertical");
        var horizontalInput = camRight * hInput + camForward * vInput;
        //new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        var horizontalVelocity = horizontalInput * PlayerSpeed;

        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, mainCamera.transform.position,
            PlayerSpeed * Time.deltaTime);

        if (horizontalInput != Vector3.zero) transform.forward = horizontalInput;

        playerVelocity = new Vector3(horizontalVelocity.x, playerVelocity.y, horizontalVelocity.z);

        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
                playerVelocity.y += Mathf.Log(JumpHeight * -PlayerSpeed * GravityValue);

            if (playerVelocity.y < 0) playerVelocity.y = 0;
        }

        playerVelocity.y += GravityValue * Time.deltaTime;

        if (playerVelocity.magnitude > movementThreshold)
            controller.Move(playerVelocity * Time.deltaTime);
    }
}