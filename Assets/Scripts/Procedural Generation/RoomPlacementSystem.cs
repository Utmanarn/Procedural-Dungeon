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
          /*int randomInt = Random.Range(0, 4);
          
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
          */
          // Temporary layout testing. This will be replaced with the actual procedural dungeon generator.
          /*_dungeonLayout = "-0--;" +
                           "-1--;" +
                           "72--;" +
                           "9---";*/

          
          FileHandler.SaveToTXT(_dungeonLayout, _fileName);
          
          LoadDungeonLayoutFromLayout();
     }

    // The actual point where we generate the full layout of the dungeon.
    private string GenerateMapLayout()
    {
        bool finishRoomPlacement = false;
        int lastRoomType = 0;
        int currentPosInString = 0;
        int nextPathLocation = 0;
        string layout = "";
        // Start out with the starting room area placement and the bottom row of rooms.
        int randomInt = Random.Range(0, 4);
        Debug.Log("Rand int: " + randomInt);
        switch (randomInt)
        {
            case 0:
                layout = "";
                lastRoomType = 0;
                currentPosInString = 0;
                break;
            case 1:
                layout = "-";
                lastRoomType = 0;
                currentPosInString = 1;
                break;
            case 2:
                layout = "--";
                lastRoomType = 0;
                currentPosInString = 2;
                break;
            case 3:
                layout = "---";
                lastRoomType = 0;
                currentPosInString = 3;
                break;
        }


        // The modulus operations to check borders with edges might cause problems for the bottom left (0x0) position as it always returns 0 on both %4 and %5 operations.
        while (!finishRoomPlacement)
        {
            if (currentPosInString != 0 && (currentPosInString - 4) % 5 == 0) // We want to add a separator at the end of every row.
            {
                if (currentPosInString < layout.Length - 1)
                {
                    currentPosInString++;
                }
                else
                {
                    layout += ";";
                    currentPosInString++;
                }

                continue;
            }

            // Wall off any room that is not on the path. (TODO: This may later change to just add in a left/right room instead to add rooms that are not on the main path. Though this may lead to everything being filled with left/right rooms everywhere.)
            if (currentPosInString != nextPathLocation)
            {
                if (nextPathLocation < currentPosInString) currentPosInString--;
                else
                {
                    if (currentPosInString < layout.Length - 1)
                    {
                        currentPosInString++;
                    }
                    else
                    {
                        layout += "-";
                        currentPosInString++;
                    }
                }

                continue;
            }

            randomInt = Random.Range(0, 5); // Number from 0-4. On a 0 or 1 the solution path moves left. On a 2 or 3 the solution path moves right. On a 4 the solution path goes up.
            if (nextPathLocation == currentPosInString && lastRoomType == 1)
            {
                randomInt = Random.Range(0, 4);
                layout += "2"; // We connect the new area with the one below.
                
                // If we go left or right as our next step.
                if (currentPosInString % 5 == 0)
                {
                    nextPathLocation++;
                    currentPosInString++;
                }
                else if ((currentPosInString - 3) % 5 == 0)
                {
                    nextPathLocation--;
                    currentPosInString--;
                }
                else if (randomInt == 0 || randomInt == 1)
                {
                    nextPathLocation--;
                    currentPosInString--;
                }
                else if (randomInt == 2 || randomInt == 3)
                {
                    nextPathLocation++;
                    currentPosInString++;
                }

                continue;
            }

            if (randomInt == 0 || randomInt == 1)
            {
                if (currentPosInString % 5 == 0) // If the room is on the leftmost row and tries to go left we go up instead.
                {
                    if (currentPosInString < layout.Length - 1) // If we are not at the end of the string we want to replace the current selected room with the new room type.
                    {
                        string subLayoutBefore = layout.Substring(0, currentPosInString - 1);
                        string subLayoutAfter = layout.Substring(currentPosInString + 1, layout.Length - 1);

                        layout = subLayoutBefore + "1" + subLayoutAfter;
                    }
                    else
                    {
                        layout += "1";
                    }

                    lastRoomType = 1;
                    nextPathLocation = currentPosInString + 5; // Path continues over the current path pos, which is 5 positions forward in the string because we got a 4x4 grid and 1 extra ; separator.
                    currentPosInString++;
                }
                else
                {
                    if (currentPosInString < layout.Length - 1)
                    {
                        string subLayoutBefore = layout.Substring(0, currentPosInString - 1);
                        string subLayoutAfter = layout.Substring(currentPosInString + 1, layout.Length - 1);

                        layout = subLayoutBefore + "0" + subLayoutAfter;
                    }
                    else
                    {
                        layout += "0";
                    }
                    lastRoomType = 0;
                    nextPathLocation = currentPosInString - 1;
                    currentPosInString--;
                }
            }
            else if (randomInt == 2 || randomInt == 3)
            {
                if ((currentPosInString - 3) % 5 == 0) // If the room is on the rightmost row and tries to go right we go up instead.
                {
                    if (currentPosInString < layout.Length - 1)
                    {
                        string subLayoutBefore = layout.Substring(0, currentPosInString - 1);
                        string subLayoutAfter = layout.Substring(currentPosInString + 1, layout.Length - 1);

                        layout = subLayoutBefore + "1" + subLayoutAfter;
                    }
                    else
                    {
                        layout += "1";
                    }

                    lastRoomType = 1;
                    nextPathLocation = currentPosInString + 5;
                    currentPosInString++;
                }
                else
                {
                    if (currentPosInString < layout.Length - 1)
                    {
                        string subLayoutBefore = layout.Substring(0, currentPosInString - 1);
                        string subLayoutAfter = layout.Substring(currentPosInString + 1, layout.Length - 1);

                        layout = subLayoutBefore + "1" + subLayoutAfter;
                    }
                    else
                    {
                        layout += "1";
                    }

                    lastRoomType = 1;
                    nextPathLocation++;
                }
            }
            else if (randomInt == 5)
            {
                // Add a room going up.
            }

            finishRoomPlacement = true;
        }

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
                         // Room going Left and Right.
                         
                         LoadRoomTypeFromFile($"roomLR{randomNumber}.json"); // Testing file to load.
                         Debug.Log("Loaded room type 0.");
                         break;
                    case '1':
                         // Room going Up, Left and Right.
                         
                         LoadRoomTypeFromFile($"roomULR{randomNumber}.json"); // Testing file to load.
                         Debug.Log("Loaded room type 1.");
                         break;
                    case '2':
                         // Room going Down, Left and Right.
                         
                         LoadRoomTypeFromFile($"roomDLR{randomNumber}.json"); // Testing file to load.
                         Debug.Log("Loaded room type 2.");
                         break;
                    /*case '3':
                         // Room going down and right.
                         
                         LoadRoomTypeFromFile($"roomDR{randomNumber}.json"); // Testing file to load.
                         Debug.Log("Loaded room type 3.");
                         break;
                    case '4':
                        // Room going down, left and right.

                         LoadRoomTypeFromFile($"roomDLR{randomNumber}.json");
                         Debug.Log("Loaded room type 4.");
                         break;
                    case '5':
                        // Room going up, down, left and right.

                         LoadRoomTypeFromFile($"roomUDLR{randomNumber}.json");
                         Debug.Log("Loaded room type 5.");
                         break;
                    case '6':
                        // Room going up and left.

                         LoadRoomTypeFromFile($"roomUL{randomNumber}.json");
                         Debug.Log("Loaded room type 6.");
                         break;
                    case '7':
                        // Room going up and right.

                         LoadRoomTypeFromFile($"roomUR{randomNumber}.json");
                         Debug.Log("Loaded room type 7.");
                         break;
                    case '8':
                        // Room going up, left and right.

                         LoadRoomTypeFromFile($"roomULR{randomNumber}.json");
                         Debug.Log("Loaded room type 8.");
                         break;
                    case '9':
                        // Room going down.

                         LoadRoomTypeFromFile($"roomD{randomNumber}.json");
                         Debug.Log("Loaded room type 9.");
                         break;*/
                    case '-':
                        // Blocked room.

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
