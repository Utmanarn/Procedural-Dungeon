using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int health = 3;

    public void DamageUnit(int damage)
    {
        health -= damage;
        if (health <= 0) OnDeath();
    }

    private void OnDeath()
    {
        Destroy(gameObject); // For now we just destroy the object.
    }
}
