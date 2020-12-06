using UnityEngine;

public interface IHittable
{
    void Hit(GameObject hitter, int shootingDamage);
}

public class DebugHittable : MonoBehaviour, IHittable
{
    public int TimesHit = 0;

    public void Hit(GameObject hitter, int shootingDamage)
        => Debug.Log($"{name} was hit {TimesHit++} times. {shootingDamage} DMG!");
}
