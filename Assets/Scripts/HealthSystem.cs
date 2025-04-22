using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("GameOverScene");
            return;
        }

        Destroy(gameObject); // For now we just destroy the object.
    }
}
