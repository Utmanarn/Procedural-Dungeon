using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class SaveHandler  : MonoBehaviour
{
    public List<CustomTile> tiles = new List<CustomTile>();

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
        List<TilemapData> data = new List<TilemapData>();

        foreach (KeyValuePair<string, Tilemap> tileMap in _tilemaps)
        {
            TilemapData mapData = new TilemapData();
            mapData.mapKey = tileMap.Key;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    TileBase tile = tileMap.Value.GetTile(pos);
                    CustomTile tempTile = tiles.Find(t => t.Tile == tile);

                    if (tempTile)
                    {
                        TileInfo nTile = new TileInfo(tempTile.ID, x, y);

                        mapData.tiles.Add(nTile);
                    }
                }
            }
            
            data.Add(mapData);
        }

        FileHandler.SaveToJSON<TilemapData>(data, fileName);
    }
    
    public void OnLoad(string loadFileName)
    {
        List<TilemapData> data = FileHandler.ReadListFromJSON<TilemapData>(loadFileName);

        foreach (var mapData in data)
        {
            if (mapData.mapKey == null)
            { 
                Debug.LogError("Map Data key missing!");
                continue;
            }

            if (!_tilemaps.ContainsKey(mapData.mapKey))
            {
                Debug.LogError("Failed to get tilemap data for " + mapData.mapKey);
                continue;
            }

            var map = _tilemaps[mapData.mapKey];

            if (mapData.tiles != null && mapData.tiles.Count > 0)
            {
                foreach (var tile in mapData.tiles)
                {
                    tile.x_pos += xOffset;
                    tile.y_pos += yOffset;
                    map.SetTile(new Vector3Int(tile.x_pos, tile.y_pos, 0), tiles.Find(t => t.ID == tile.id).Tile);
                }
            }
        }
    }
}

[Serializable]
public class TilemapData
{
    public string mapKey;
    public List<TileInfo> tiles = new List<TileInfo>();
}

[Serializable]
public class TileInfo
{
    public string id;
    public int x_pos;
    public int y_pos;

    public TileInfo(string id, int x, int y)
    {
        this.id = id;
        x_pos = x;
        y_pos = y;
    }
}
