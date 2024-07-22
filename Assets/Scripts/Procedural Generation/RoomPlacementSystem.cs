using System.Collections.Generic;
using UnityEngine;

public class RoomPlacementSystem : MonoBehaviour
{
     private HashSet<string> _roomFileNames = new HashSet<string>();
     
     private SaveHandler _saveHandler;

     private string _fileName;
     private string _dungeonLayout;

     private bool _specialModifierFlag;

    [Header("Debugging")]
    [SerializeField] private bool testLoadLayouts = false;

    private void Awake()
     {
          _saveHandler = GetComponent<SaveHandler>();
          AddRoomsToHasSet();
     }

     private void Start()
     {
        _fileName = "DungeonLayout.txt";
        _specialModifierFlag = false;
        if (testLoadLayouts)
        {
            TestLoadMaps();
            return;
        }
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
          _dungeonLayout = GenerateMapLayout();
          
          // Should move this to after the map layout is done.
          int randomInt = Random.Range(0, 4);
          
          switch (randomInt)
          {
               case 0:
                    // The bottom left room shall be the starter room (special case).
                    _dungeonLayout += "r0x0;";
                    break;
               case 1:
                    // The room next over to the bottom left shall be the starter room (special case).
                    _dungeonLayout += "r1x0;";
                    break;
               case 2:
                    _dungeonLayout += "r2x0;";
                    break;
               case 3:
                    _dungeonLayout += "r3x0;";
                    break;
               default:
                    Debug.LogError($"Something went wrong while choosing the starter room, {randomInt} was out of range.");
                    break;
          }
          
          // Temporary layout testing. This will be replaced with the actual procedural dungeon generator.
          _dungeonLayout = "-0--;" +
                           "-1--;" +
                           "72--;" +
                           "9---";

          
          FileHandler.SaveToTXT(_dungeonLayout, _fileName);
          
          LoadDungeonLayoutFromLayout();
     }

    // The actual point where we generate the full layout of the dungeon.
    private string GenerateMapLayout()
    {
        string layout = "";
        // Start out with the starting room area placement and the bottom row of rooms.


        return layout;
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

     /// <summary>
     /// Load a specific room number from a char. Alt: Decide a specific modifier for a room.
     /// </summary>
     /// <param name="roomNumber">The number that denotes the room type. Alt: Modifier key.</param>
     private void LoadRoomTypeFromChar(char roomNumber)
     {
          //int randomNumber = Random.Range(0, however many rooms of the roomNumber type exists + 1); // Use the Random.Range[int] variant https://docs.unity3d.com/ScriptReference/Random.Range.html
          int randomNumber = 0; // TEMP DEBUGGING VARIABLE

          if (!_specialModifierFlag)
          {
               switch (roomNumber)
               {
                    case '0':
                         // Load a random room type of room type 0.
                         
                         LoadRoomTypeFromFile($"roomU{randomNumber}.json"); // Testing file to load.
                         Debug.Log("Loaded room type 0.");
                         break;
                    case '1':
                         // Load a random... type 1.
                         
                         LoadRoomTypeFromFile($"roomUD{randomNumber}.json"); // Testing file to load.
                         Debug.Log("Loaded room type 1.");
                         break;
                    case '2':
                         // So on...
                         
                         LoadRoomTypeFromFile($"roomDL{randomNumber}.json"); // Testing file to load.
                         Debug.Log("Loaded room type 2.");
                         break;
                    case '3':
                         // So forth...
                         
                         LoadRoomTypeFromFile($"roomDR{randomNumber}.json"); // Testing file to load.
                         Debug.Log("Loaded room type 3.");
                         break;
                    case '4':
                         LoadRoomTypeFromFile($"roomDLR{randomNumber}.json");
                         Debug.Log("Loaded room type 4.");
                         break;
                    case '5':
                         LoadRoomTypeFromFile($"roomUDLR{randomNumber}.json");
                         Debug.Log("Loaded room type 5.");
                         break;
                    case '6':
                         LoadRoomTypeFromFile($"roomUL{randomNumber}.json");
                         Debug.Log("Loaded room type 6.");
                         break;
                    case '7':
                         LoadRoomTypeFromFile($"roomUR{randomNumber}.json");
                         Debug.Log("Loaded room type 7.");
                         break;
                    case '8':
                         LoadRoomTypeFromFile($"roomULR{randomNumber}.json");
                         Debug.Log("Loaded room type 8.");
                         break;
                    case '9':
                         LoadRoomTypeFromFile($"roomD{randomNumber}.json");
                         Debug.Log("Loaded room type 9.");
                         break;
                    case '-':
                         LoadRoomTypeFromFile($"roomB.json");
                         Debug.Log("Loaded room type -.");
                         break;
                    case '*':
                         // This denotes a shift from the layout of the rooms to the special modifiers such as start point and end point location.
                         _specialModifierFlag = true;
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
          else
          {
               // if (roomNumber = an integer) { We want to process it based on the modifier flag that is currently active. (Example: If the 'r' flag is active we want to save it
               // as a Vector2 coordiante.) }
               
               // The layout is done and we start to work in with special modifiers for rooms.
               switch (roomNumber)
               {
                    case ';':
                         // Denotes the end of the current flag being modified.
                         break;
                    case 'r':
                         // Denotes that the following numbers will be used to decide the x and y pos of the room we wish to modify.
                         
                         break;
                    case 'x':
                         // Denotes the end of the x-axis and start of the y-axis coordinates.
                         break;
                    default:
                         Debug.LogError("Unrecognised modifier type: " + roomNumber);
                         break;
               }
               
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
