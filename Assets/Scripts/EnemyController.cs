using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int Health = 100;

    public void OnPartHit(GameObject part, int shootingDamage)
    {
        Debug.Log($"{part.name} was hit!");
    }

    public void OnPartDestroyed(GameObject part)
    {
        Debug.Log($"{part.name} was destroyed!");
    }
}
