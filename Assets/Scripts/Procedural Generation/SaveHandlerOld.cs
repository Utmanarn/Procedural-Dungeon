using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class SaveHandlerOld : MonoBehaviour
{
    private Dictionary<string, Tilemap> _tilemaps = new Dictionary<string, Tilemap>();
    [SerializeField] private BoundsInt bounds; // Bounds for the rooms default would be pos -5, -5 and size 11, 11.
    [SerializeField] private string fileName;
    [SerializeField] private bool saveOnStart = false;
    [SerializeField] private bool loadOnStart = false;

    public int xOffset, yOffset; // Every offset should be set by 11.

    private void Awake()
    {
        InitTilemaps();
        if (saveOnStart) OnSave();
        else if (loadOnStart) OnLoad(fileName); // For now we just load the fileName, in the future we will load multiple different files to fill out the levels.

        saveOnStart = false;
        loadOnStart = false;
    }

    private void InitTilemaps()
    {
        Tilemap[] maps = FindObjectsOfType<Tilemap>();

        foreach (Tilemap map in maps)
        {
            if (map.name == "Tilemap_NoClear") continue;
            if (!saveOnStart) map.ClearAllTiles(); // We clear all of the tilemaps on start to avoid loading over anything currently placed on the maps.
            _tilemaps.Add(map.name, map);
        }
    }

    private void OnSave()
    {
        List<OldTilemapData> data = new List<OldTilemapData>();

        foreach (KeyValuePair<string, Tilemap> tileMap in _tilemaps)
        {
            OldTilemapData mapData = new OldTilemapData();
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
                            OldTileInfo nTile = new OldTileInfo(tile, pos, guid);
                            mapData.tiles.Add(nTile);
                        }
                    }
                }
            }
            
            data.Add(mapData);
        }

        FileHandler.SaveToJSON<OldTilemapData>(data, fileName);
    }
    
    public void OnLoad(string loadFileName)
    {
        List<OldTilemapData> data = FileHandler.ReadListFromJSON<OldTilemapData>(loadFileName);

        foreach (var mapData in data)
        {
            if (!_tilemaps.ContainsKey(mapData.key))
            {
                Debug.LogError("Failed to get tilemap data for " + mapData.key);
                continue;
            }

            var map = _tilemaps[mapData.key];

            if (mapData.tiles != null && mapData.tiles.Count > 0)
            {
                foreach (OldTileInfo tile in mapData.tiles)
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

                    tile.position += new Vector3Int(xOffset, yOffset, 0);
                    map.SetTile(tile.position, tileBase);
                }
            }
        }
    }
}

[Serializable]
public class OldTilemapData
{
    public string key;
    public List<OldTileInfo> tiles = new List<OldTileInfo>();
}

[Serializable]
public class OldTileInfo
{
    public TileBase tile;
    public string guid;
    public Vector3Int position;

    public OldTileInfo(TileBase tile, Vector3Int pos, string guid)
    {
        this.tile = tile;
        position = pos;
        this.guid = guid;
    }
}
