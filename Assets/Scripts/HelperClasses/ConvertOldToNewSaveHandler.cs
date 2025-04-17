using UnityEngine;

public class ConvertOldToNewSaveHandler : MonoBehaviour
{
    RoomPlacementSystem RoomPlacementSystem;

    private void Awake()
    {
        RoomPlacementSystem = GetComponent<RoomPlacementSystem>();
    }
}
