using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float CameraRotationSpeed = 100;
    [Range(0, 360)] public float CameraMaxUpwardAngle = 65;
    [Range(0, 360)] public float CameraMaxDownwardAngle = 320;
    public GameObject CameraTarget;

    private Vector2 _lookVector;
    private bool _isMoving;

    public void Look(InputAction.CallbackContext context) => _lookVector = context.ReadValue<Vector2>();

    public void SetIsMoving(InputAction.CallbackContext context) => _isMoving = !context.canceled;

    void Update()
    {
        var hInput = _lookVector.x;
        var vInput = _lookVector.y;

        var rotationDelta = CameraRotationSpeed * Time.deltaTime;

        if (_isMoving)
            RotatePlayer(hInput, vInput, rotationDelta);
        else
            RotateAroundPlayer(hInput, vInput, rotationDelta);
    }

    void RotatePlayer(float hInput, float vInput, float rotationDelta)
    {
        var playerRotation = transform.rotation.eulerAngles;
        playerRotation += Vector3.up * hInput * rotationDelta;
        transform.rotation = Quaternion.Euler(playerRotation);

        RotateAroundPlayer(0, vInput, rotationDelta);
    }

    void RotateAroundPlayer(float hInput, float vInput, float rotationDelta)
    {
        var camRotation = CameraTarget.transform.rotation.eulerAngles;
        camRotation += Vector3.up * hInput * rotationDelta;

        var futureAngle = camRotation.x + vInput * rotationDelta;
        if (CameraMaxUpwardAngle >= futureAngle || futureAngle >= CameraMaxDownwardAngle)
            camRotation += Vector3.right * vInput * rotationDelta;
        CameraTarget.transform.rotation = Quaternion.Euler(camRotation);
    }
}