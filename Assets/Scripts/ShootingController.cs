using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    public GameObject Bullet;
    public Cinemachine3rdPersonAim aim;

    private bool isShooting = false;
    private int layerMask;
    private Camera mainCamera;

    private readonly Color[] _colors =
    {
        Color.red, Color.blue, Color.yellow, Color.black, Color.green
    };

    public void Shoot(InputAction.CallbackContext context) => isShooting = !context.canceled;

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Player");
        layerMask = ~layerMask;

        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        if (isShooting)
        {
            var cameraDirection = mainCamera.transform.TransformDirection(Vector3.forward);
            if (Physics.Raycast(mainCamera.transform.position, cameraDirection,
                out var hit, Mathf.Infinity,
                layerMask))
            {
                Debug.DrawRay(mainCamera.transform.position,
                    cameraDirection * hit.distance,
                    Color.yellow);
                Debug.Log("Did Hit");
                HandleHit(hit);
            }
            else
            {
                Debug.DrawRay(mainCamera.transform.position,
                    cameraDirection * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }
    }

    void HandleHit(RaycastHit hit)
    {
        var random = new System.Random();
        var randomColor = _colors[random.Next(0, _colors.Length - 1)];
        hit.transform.gameObject.GetComponent<Renderer>().material.color = randomColor;
    }
}