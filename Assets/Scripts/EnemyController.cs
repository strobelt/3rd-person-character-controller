using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public void OnPartDestroyed(GameObject part)
    {
        Debug.Log($"{part.name} was destroyed!");
    }

    public void OnPartHit(GameObject part)
    {
        Debug.Log($"{part.name} was hit!");
    }
}
