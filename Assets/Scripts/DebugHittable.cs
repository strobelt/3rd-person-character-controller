using UnityEngine;

public interface IHittable
{
    void Hit(GameObject hitter);
}

public class DebugHittable : MonoBehaviour, IHittable
{
    public int TimesHit = 0;

    public void Hit(GameObject hitter)
        => Debug.Log($"{name} was hit {TimesHit++} times");
}
