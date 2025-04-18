using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class FinishDoor : MonoBehaviour
{
    private TilemapCollider2D tilemapCollider;

    private void Awake()
    {
        tilemapCollider = GetComponent<TilemapCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SceneManager.LoadScene("VictoryScene");
    }
}
