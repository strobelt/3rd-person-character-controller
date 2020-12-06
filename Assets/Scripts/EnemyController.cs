using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int Toughness = 1000;

    public void OnPartHit(GameObject part, int shootingDamage)
    {
        Debug.Log($"{part.name} was hit for {shootingDamage} points of damage!");
        Toughness -= shootingDamage;
    }

    public void OnPartDestroyed(GameObject part)
    {
        Debug.Log($"{part.name} was destroyed!");
    }
}
