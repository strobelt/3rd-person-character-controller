using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float CameraRotationSpeed = 100;
    [Range(0, 360)] public float CameraMaxUpwardAngle = 65;
    [Range(0, 360)] public float CameraMaxDownwardAngle = 320;
    public GameObject CameraTarget;

    public void Look(InputAction.CallbackContext context)
    {
        var lookVector = context.ReadValue<Vector2>();
        var hInput = lookVector.x;
        var vInput = lookVector.y;

        var rotationDelta = CameraRotationSpeed * Time.deltaTime;

        var playerRotation = transform.rotation.eulerAngles;
        playerRotation += Vector3.up * hInput * rotationDelta;
        transform.rotation = Quaternion.Euler(playerRotation);

        var camRotation = CameraTarget.transform.rotation.eulerAngles;
        var futureAngle = camRotation.x + vInput * rotationDelta;
        if (CameraMaxUpwardAngle >= futureAngle || futureAngle >= CameraMaxDownwardAngle)
            camRotation += Vector3.right * vInput * rotationDelta;
        CameraTarget.transform.rotation = Quaternion.Euler(camRotation);
    }
}
