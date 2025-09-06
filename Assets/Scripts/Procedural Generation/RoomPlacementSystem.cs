using System.Collections.Generic;
using UnityEngine;

public class RoomPlacementSystem : MonoBehaviour
{
     private HashSet<string> _roomFileNames = new HashSet<string>();
     
     private SaveHandler _saveHandler;

     private string _fileName;
     private char[,] _dungeonLayout;

     private bool _specialModifierFlag;

    private const int width = 4, height = 4;

     [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> enemies;

    [Header("Debugging")]
    [SerializeField] private bool testLoadLayouts = false;

    private void Awake()
     {
        _saveHandler = GetComponent<SaveHandler>();
        AddRoomsToHashSet();
     }

     private void Start()
     {
         _dungeonLayout = new char[width, height];
        _fileName = "DungeonLayout.json";
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

          char[,] layout = FileHandler.Read2DArrayFromTXT(_fileName, width, height);

          if (layout == null)
          {
               Debug.LogError("Failed to load dungeon layout, are you sure the layout was saved correctly?");
               return;
          }

          int randEnemyCount = Random.Range(1, 8);
          int randomSpawnChance = Random.Range(1, 4);
          int spawnedEnemyCount = 0;
          int randomEnemyUnit = 0;
          bool hasSpawnedPlayer = false;
          bool roomOccupiedByPlayer = false;

          for (int y = 0; y < height; y++)
          {
              for (int x = 0; x < width; x++)
              {
                  char roomType = layout[x, y];

                  if (roomType != '-' && !hasSpawnedPlayer)
                  {
                      // Spawn player
                      Vector3 playerSpawnOffset = new Vector3(11 * x, 11 * y);
                      Instantiate(player, playerSpawnOffset, Quaternion.identity);
                      hasSpawnedPlayer = true;
                      roomOccupiedByPlayer = true;
                  }

                  if (roomType != '-' && !roomOccupiedByPlayer && randEnemyCount > spawnedEnemyCount && randomSpawnChance < 3)
                  {
                      // Spawn enemy
                      Vector3 enemySpawnOffset = new Vector3(11 * x, 11 * y);
                      randomEnemyUnit = Random.Range(0, 3);

                      Instantiate(enemies[randomEnemyUnit], enemySpawnOffset, Quaternion.identity);
                      spawnedEnemyCount++;
                  }

                  Debug.Log("roomType = " + roomType + " Offset = " + new Vector2Int(x, y));
                  LoadRoomTypeFromChar(roomType, new Vector2Int(x, y));

                  randomSpawnChance = Random.Range(1, 4);
                  roomOccupiedByPlayer = false;
              }
          }
    }

     private void GenerateDungeonLayout()
     {
          _dungeonLayout = GenerateMapLayout();
          FileHandler.SaveArrayToTXT(_dungeonLayout, _fileName);
          LoadDungeonLayoutFromLayout();
     }

    // The actual point where we generate the full layout of the dungeon.
    private char[,] GenerateMapLayout()
    {
        bool finishRoomPlacement = false;
        char lastRoomType = '0';
        Vector2Int currentPosition = new Vector2Int(0, 0);
        Vector2Int nextPathLocation = new Vector2Int(0, 0);
        char[,] layout = new char[width, height];
        // Start out with the starting room area placement and the bottom row of rooms.
        int randomInt = Random.Range(0, 4);
        Debug.Log("Rand int: " + randomInt);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                layout[i, j] = '-';
            }
        }
        switch (randomInt)
        {
            case 0:
                lastRoomType = '0';
                currentPosition = new Vector2Int(0, 0);
                nextPathLocation = new Vector2Int(0, 0);
                break;
            case 1:
                lastRoomType = '0';
                currentPosition = new Vector2Int(1, 0);
                nextPathLocation = new Vector2Int(1, 0);
                break;
            case 2:
                lastRoomType = '0';
                currentPosition = new Vector2Int(2, 0);
                nextPathLocation = new Vector2Int(2, 0);
                break;
            case 3:
                lastRoomType = '0';
                currentPosition = new Vector2Int(3, 0);
                nextPathLocation = new Vector2Int(3, 0);
                break;
        }

        int safetyCounter = 0;
        while (!finishRoomPlacement)
        {
            if (safetyCounter > 40)
            {
                Debug.LogWarning("Loop reached the max number of iterations.");
                break;
            }
            safetyCounter++;

            if (currentPosition.y >= height) break; // If we reach a number greater than height on the y axis we know we are done.

            if (nextPathLocation == currentPosition && lastRoomType == '1')
            {
                randomInt = Random.Range(0, 4);
                layout[currentPosition.x, currentPosition.y] = '2'; // We connect the new area with the one below.
                lastRoomType = '2';
                
                // If we go left or right as our next step.
                if (currentPosition.x == 0)
                {
                    nextPathLocation.x++;
                    currentPosition.x++;
                }
                else if (currentPosition.x == width - 1)
                {
                    nextPathLocation.x--;
                    currentPosition.x--;
                }
                else if (randomInt == 0 || randomInt == 1)
                {
                    nextPathLocation.x--;
                    currentPosition.x--;
                }
                else if (randomInt == 2 || randomInt == 3)
                {
                    nextPathLocation.x++;
                    currentPosition.x++;
                }
                
                continue;
            }

            randomInt = Random.Range(0, 5); // Number from 0-4. On a 0 or 1 the solution path moves left. On a 2 or 3 the solution path moves right. On a 4 the solution path goes up.

            if (randomInt == 0 || randomInt == 1)
            {
                if (currentPosition.x == 0) // If the room is on the leftmost row and tries to go left we go up instead.
                {
                    if (TestForFinishDoor(currentPosition))
                    {
                        if (layout[currentPosition.x, currentPosition.y] == '2')
                        {
                            layout[currentPosition.x, currentPosition.y] = ':';
                        }
                        else
                        {
                            layout[currentPosition.x, currentPosition.y] = '.';
                        }
                        break;
                    }

                    if (layout[currentPosition.x, currentPosition.y] == '2')
                    {
                        layout[currentPosition.x, currentPosition.y] = '3';

                        lastRoomType = '1'; // We still pretend that the last room only goes up.
                    }
                    else
                    {
                        layout[currentPosition.x, currentPosition.y] = '1';
                        lastRoomType = '1';
                    }
                    
                    nextPathLocation.y++; // Path continues over the current path pos, which is 5 positions forward in the string because we got a 4x4 grid and 1 extra ; separator.
                    currentPosition.y++;
                }
                else
                {
                    if (layout[currentPosition.x, currentPosition.y] == '-')
                    {
                        layout[currentPosition.x, currentPosition.y] = '0';

                        lastRoomType = '0';
                    }
                    
                    nextPathLocation.x--;
                    currentPosition.x--;
                }
            }
            else if (randomInt == 2 || randomInt == 3)
            {
                if (currentPosition.x == width - 1) // If the room is on the rightmost row and tries to go right we go up instead.
                {
                    if (TestForFinishDoor(currentPosition))
                    {
                        if (layout[currentPosition.x, currentPosition.y] == '2')
                        {
                            layout[currentPosition.x, currentPosition.y] = ':';
                        }
                        else
                        {
                            layout[currentPosition.x, currentPosition.y] = '.';
                        }
                        break;
                    }

                    if (layout[currentPosition.x, currentPosition.y] == '2')
                    {
                        layout[currentPosition.x, currentPosition.y] = '3';

                        lastRoomType = '1'; // We still pretend that the last room only goes up.
                    }
                    else
                    {
                        layout[currentPosition.x, currentPosition.y] = '1';
                        lastRoomType = '1';
                    }
                    nextPathLocation.y++;
                    currentPosition.y++;
                }
                else
                {
                    if (layout[currentPosition.x, currentPosition.y] == '-')
                    {
                        layout[currentPosition.x, currentPosition.y] = '0';

                        lastRoomType = '0';
                    }
                    nextPathLocation.x++;
                    currentPosition.x++;
                }
            }
            else if (randomInt == 4)
            {
                // Add a room going up.
                if (TestForFinishDoor(currentPosition))
                {
                    if (layout[currentPosition.x, currentPosition.y] == '2')
                    {
                        layout[currentPosition.x, currentPosition.y] = ':';
                    }
                    else
                    {
                        layout[currentPosition.x, currentPosition.y] = '.';
                    }
                    break;
                }
                else
                {
                    if (layout[currentPosition.x, currentPosition.y] == '2')
                    {
                        layout[currentPosition.x, currentPosition.y] = '3';

                        lastRoomType = '1'; // We still pretend that the last room only goes up.
                    }
                    else
                    {
                        layout[currentPosition.x, currentPosition.y] = '1';
                        lastRoomType = '1';
                    }
                    nextPathLocation.y++;
                    currentPosition.y++;
                }
            }
        }

        string debugCheck = "";
        
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                debugCheck += layout[j, i];
                if (j >= width - 1)
                {
                    debugCheck += "\n";
                }
            }
        }
        
        Debug.Log(debugCheck);
        
        return layout;
    }

    private bool TestForFinishDoor(Vector2 pos)
    {
        // TODO: This will check if we are at the top row of the layout string and if so we want to add the finish room instead of adding a room that goes up.
        if (pos.y == height - 1)
        {
            return true;
        }
        return false;
    }

     private void AddRoomsToHashSet()
     {
          System.String[] files = System.IO.Directory.GetFiles(Application.streamingAssetsPath + "/Rooms/");

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
     private void LoadRoomTypeFromChar(char roomNumber, Vector2Int offset)
     {
          //int randomNumber = Random.Range(0, however many rooms of the roomNumber type exists + 1); // Use the Random.Range[int] variant https://docs.unity3d.com/ScriptReference/Random.Range.html
          int randomNumber = 0; // TEMP DEBUGGING VARIABLE
          _saveHandler.xOffset = offset.x * 11;
          _saveHandler.yOffset = offset.y * 11;

          if (!_specialModifierFlag)
          {
               switch (roomNumber)
               {
                    case '0':
                         // Room going Left and Right.
                         
                         LoadRoomTypeFromFile($"roomLR{randomNumber}.json");
                         break;
                    case '1':
                         // Room going Up, Left and Right.

                         LoadRoomTypeFromFile($"roomULR{randomNumber}.json");
                         break;
                    case '2':
                         // Room going Down, Left and Right.
                         
                         LoadRoomTypeFromFile($"roomDLR{randomNumber}.json");
                         break;
                    case '3':
                         // Room going up, down, left and right.
                         
                         LoadRoomTypeFromFile($"roomUDLR{randomNumber}.json");
                         Debug.Log("Loaded room type 3.");
                         break;
                    /*case '4':
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
                         break;
                    case '.':
                         // This is the finish room going left and right.

                         LoadRoomTypeFromFile($"roomLRF{randomNumber}.json");
                         break;
                    case ':':
                        // This is the finish room going down, left and right.
                        
                        LoadRoomTypeFromFile($"roomDLRF{randomNumber}.json");
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
