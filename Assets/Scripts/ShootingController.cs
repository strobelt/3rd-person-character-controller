using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    public GameObject Bullet;
    public Cinemachine3rdPersonAim Aim;
    public Canvas Canvas;

    private bool _isShooting;
    private int _hittableLayerMask;

    private readonly Color[] _colors =
    {
        Color.red, Color.blue, Color.yellow, Color.black, Color.green
    };

    public void Shoot(InputAction.CallbackContext context) => _isShooting = !context.canceled;

    void Start()
    {
        _hittableLayerMask = 1 << LayerMask.NameToLayer("Player");
        _hittableLayerMask = ~_hittableLayerMask;
    }

    void FixedUpdate()
    {
        if (_isShooting)
        {
            var ray = GetRayFromTargetingReticle();

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _hittableLayerMask))
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.yellow);
                Debug.Log("Did Hit");
                HandleHit(hit);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.white);
                Debug.Log("Did not Hit");
            }
        }
    }

    Ray GetRayFromTargetingReticle()
    {
        var canvasCenter = Canvas.pixelRect.center;
        var aimCenter = Aim.AimTargetReticle.rect.center;
        return Camera.main.ScreenPointToRay(canvasCenter - aimCenter);
    }

    void HandleHit(RaycastHit hit)
    {
        var random = new System.Random();
        var randomColor = _colors[random.Next(0, _colors.Length - 1)];
        hit.transform.gameObject.GetComponent<Renderer>().material.color = randomColor;
    }
}