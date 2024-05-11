using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class SaveHandler : MonoBehaviour
{
    private Dictionary<string, Tilemap> _tilemaps = new Dictionary<string, Tilemap>();
    [SerializeField] private BoundsInt bounds; // Bounds for the rooms default would be pos -5, -5 and size 11, 11.
    [SerializeField] private string fileName;
    [SerializeField] private bool saveOnStart = false;
    [SerializeField] private bool loadOnStart = false;

    private void Start()
    {
        InitTilemaps();
        if (saveOnStart) OnSave();
        else if (loadOnStart) OnLoad();

        saveOnStart = false;
        loadOnStart = false;
    }

    private void InitTilemaps()
    {
        Tilemap[] maps = FindObjectsOfType<Tilemap>();

        foreach (Tilemap map in maps)
        {
            _tilemaps.Add(map.name, map);
        }
    }

    private void OnSave()
    {
        List<TilemapData> data = new List<TilemapData>();

        foreach (KeyValuePair<string, Tilemap> tileMap in _tilemaps)
        {
            TilemapData mapData = new TilemapData();
            mapData.key = tileMap.Key;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    TileBase tile = tileMap.Value.GetTile(pos);

                    if (tile)
                    {
                        if (UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(tile, out string guid,
                                out long localId))
                        {
                            TileInfo nTile = new TileInfo(tile, pos, guid);
                            mapData.tiles.Add(nTile);
                        }
                    }
                }
            }
            
            data.Add(mapData);
        }

        FileHandler.SaveToJSON<TilemapData>(data, fileName);
    }

    private void OnLoad()
    {
        List<TilemapData> data = FileHandler.ReadListFromJSON<TilemapData>(fileName);

        foreach (var mapData in data)
        {
            if (!_tilemaps.ContainsKey(mapData.key))
            {
                Debug.LogError("Failed to get tilemap data for " + mapData.key);
                continue;
            }

            var map = _tilemaps[mapData.key];
            
            map.ClearAllTiles();

            if (mapData.tiles != null && mapData.tiles.Count > 0)
            {
                foreach (TileInfo tile in mapData.tiles)
                {
                    TileBase tileBase = tile.tile;

                    if (tileBase == null)
                    {
                        Debug.Log("[Loading Tilemap]: InstanceID not found - looking in AssetDatabase");
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(tile.guid);
                        tileBase = UnityEditor.AssetDatabase.LoadAssetAtPath<TileBase>(path);

                        if (tileBase == null)
                        {
                            Debug.LogError("[Loading Tilemap]: Tile not found in AssetDatabase");
                            continue;
                        }
                    }
                    map.SetTile(tile.position, tileBase);
                }
            }
        }
    }
}

[Serializable]
public class TilemapData
{
    public string key;
    public List<TileInfo> tiles = new List<TileInfo>();
}

[Serializable]
public class TileInfo
{
    public TileBase tile;
    public string guid;
    public Vector3Int position;

    public TileInfo(TileBase tile, Vector3Int pos, string guid)
    {
        this.tile = tile;
        position = pos;
        this.guid = guid;
    }
}
