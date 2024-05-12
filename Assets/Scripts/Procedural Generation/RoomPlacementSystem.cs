using System.Collections.Generic;
using UnityEngine;

public class RoomPlacementSystem : MonoBehaviour
{
     private HashSet<string> _roomFileNames = new HashSet<string>();
     
     private SaveHandler _saveHandler;

     private int xOffset, yOffset;

     private void Awake()
     {
          _saveHandler = GetComponent<SaveHandler>();
          AddRoomsToHasSet();
     }

     private void AddRoomsToHasSet()
     {
          System.String[] files = System.IO.Directory.GetFiles(Application.dataPath + "/Rooms/");

          foreach (string name in files)
          {
               string n = System.IO.Path.GetFileName(name);
               if (n.Contains(".meta")) continue;
               
               _roomFileNames.Add(n);
          }
     }

     private void TestLoadAllSavedRooms()
     {
          foreach (var room in _roomFileNames) // This is just testing the loading of all the saved maps. For the actual map loading it will have to take in the generated map information and load it according to that.
          {
               _saveHandler.OnLoad(room);
               xOffset += 11;
               _saveHandler.SetOffsetValues(xOffset, yOffset);
          }
     }

     public void TestLoadMaps()
     {
          Debug.Log("Test loading maps.");
          TestLoadAllSavedRooms();
     }
}
