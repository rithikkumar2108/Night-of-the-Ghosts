using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float Health = 100;
    public void TakeDamage(float d)
    {
        Debug.Log("Player Hit");
        Health -= d;
    }
}
