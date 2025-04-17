using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu (fileName = "New CustomTile", menuName = "ScriptableObjects/CustomTile")]
public class CustomTile : ScriptableObject
{
    public TileBase Tile;
    public string ID;
}
