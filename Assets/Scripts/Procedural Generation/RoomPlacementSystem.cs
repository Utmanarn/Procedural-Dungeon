using System.Collections.Generic;
using UnityEngine;

public class RoomPlacementSystem : MonoBehaviour
{
     private HashSet<string> _roomFileNames = new HashSet<string>();
     
     private SaveHandler _saveHandler;

     private string _fileName;
     private string _dungeonLayout;

     private void Awake()
     {
          _saveHandler = GetComponent<SaveHandler>();
          AddRoomsToHasSet();
     }

     private void Start()
     {
          _fileName = "DungeonLayout.txt";
          GenerateDungeonLayout();
     }

     private void LoadDungeonLayoutFromLayout()
     {
          if (_dungeonLayout == null)
          {
               Debug.LogError("Tried to access _dungeonLayout but the value is null.");
               return;
          }

          string layout = FileHandler.ReadStringFromTXT(_fileName);

          if (layout == "")
          {
               Debug.LogError("Failed to load dungeon layout, are you sure the layout was saved correctly?");
               return;
          }
         
          Debug.Log("Layout is: " + layout);
          for (int i = 0; i < layout.Length; i++)
          {
               char roomType = layout[i];

               LoadRoomTypeFromChar(roomType);
          }
     }

     private void GenerateDungeonLayout()
     {
          // Temporary layout testing. This will be replaced with the actual procedural dungeon generator.
          _dungeonLayout = "0100;" +
                           "0100;" +
                           "0100;" +
                           "0100";
          
          FileHandler.SaveToTXT(_dungeonLayout, _fileName);
          
          LoadDungeonLayoutFromLayout();
     }

     private void AddRoomsToHasSet()
     {
          System.String[] files = System.IO.Directory.GetFiles(Application.dataPath + "/Rooms/");

          foreach (string name in files)
          {
               string n = System.IO.Path.GetFileName(name);
               if (n.Contains(".meta") || !n.Contains(".json")) continue;
               
               _roomFileNames.Add(n);
          }
     }

     private void LoadRoomTypeFromChar(char roomNumber)
     {
          switch (roomNumber)
          {
               case '0':
                    // Load a random room type of room type 0.
                    
                    LoadRoomTypeFromFile("mapData.json"); // Testing file to load.
                    
                    Debug.Log("Loaded room type 0.");
                    break;
               case '1':
                    // Load a random... type 1.
                    
                    LoadRoomTypeFromFile("mapData2.json"); // Testing file to load.
                    
                    Debug.Log("Loaded room type 1.");
                    break;
               case '2':
                    // So on...
                    
                    LoadRoomTypeFromFile("mapData3.json"); // Testing file to load.
                    
                    Debug.Log("Loaded room type 2.");
                    break;
               case '3':
                    // So forth...
                    
                    LoadRoomTypeFromFile("mapData4.json"); // Testing file to load.
                    
                    Debug.Log("Loaded room type 3.");
                    break;
               case ';':
                    // Shift the y-axis offset up one and reset the x-axis offset.

                    _saveHandler.xOffset = 0;
                    _saveHandler.yOffset += 11;
                    
                    Debug.Log("Shifting the room y-axis offset and resetting the x-axis offset.");
                    break;
               default:
                    Debug.LogError("Room number was outside the range of allowed types.");
                    break;
          }
     }

     private void LoadRoomTypeFromFile(string fileName)
     {
          _saveHandler.OnLoad(fileName);
          _saveHandler.xOffset += 11;
     }

     private void TestLoadAllSavedRooms()
     {
          foreach (var room in _roomFileNames) // This is just testing the loading of all the saved maps. For the actual map loading it will have to take in the generated map information and load it according to that.
          {
               _saveHandler.OnLoad(room);
               _saveHandler.xOffset += 11;
          }
     }

     public void TestLoadMaps()
     {
          Debug.Log("Test loading maps.");
          TestLoadAllSavedRooms();
     }
}
