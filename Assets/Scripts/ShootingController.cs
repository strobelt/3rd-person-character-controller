using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    public GameObject Bullet;

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            var instance = Instantiate(Bullet, transform.position, transform.rotation);
        }
    }
}