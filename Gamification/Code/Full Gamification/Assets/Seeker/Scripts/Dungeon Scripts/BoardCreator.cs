using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    // The type of tile that will be laid in a specific position.
    public enum TileType
    {
        Wall, Floor, Trap, Reward, People, HiddenFloor, HiddenTrap, HiddenReward
    }

    private DungeonSetting dungeon_stats;

    private int mod_power = 0;
    private int div_power = 0;
    private float trap_chance;                               // Actual chance of traps

    public GameObject[] floorTiles;                           // An array of floor tile prefabs.
    public GameObject[] wallTiles;                            // An array of wall tile prefabs.
    public GameObject[] outerWallTiles;                       // An array of outer wall tile prefabs.
    public GameObject[] hidingTiles;                         // An array of tiles that are wall tiles but can be passed through
    public GameObject treasure;                            // Items found in dungeon
    public GameObject hiddenTreasure;                       // Hidden treasure found in hidden corridors. Contains incremental stuff.
    public GameObject citizens;
    public GameObject[] traps;                                // Traps found in dungeon
    public GameObject exit;                                   // Exit


    private TileType[][] tiles;                               // A jagged array of tile types representing the board, like a grid.
    private Room[] rooms;                                     // All the rooms that are created for this board.
    private Corridor[] corridors;                             // All the corridors that connect the rooms.
    private List<Corridor> loopable_corridors;       // Used to allow cooridor loops
    private GameObject boardHolder;                           // GameObject that acts as a container for all other tiles.

    private int num_hidden;                                 // Number of hidden rooms. Calculated after rooms and treasure rooms created.
    private HiddenCorridor[] hidden_corridors;                            // Hidden rooms

    private void Awake()
    {
        // Create the board holder.
        boardHolder = new GameObject("BoardHolder");

        dungeon_stats = GlobalControl.Instance.dungeon;

        SetupTilesArray();

        CreateRoomsAndCorridors();
        
        SetTilesValuesForCorridors();
        SetTilesValuesForRooms();

        InstantiateTiles();
        InstantiateOuterWalls();
    }


    void SetupTilesArray()
    {
        // Set the tiles jagged array to the correct width.
        tiles = new TileType[dungeon_stats.columns][];

        // Go through all the tile arrays...
        for (int i = 0; i < tiles.Length; i++)
        {
            // ... and set each tile array is the correct height.
            tiles[i] = new TileType[dungeon_stats.rows];
        }
    }

    void CreateRoomsAndCorridors()
    {
        // Get Div and Mod of hit_power to adjust amount of traps

        // mod_power adjusts the exponential amount of traps
        mod_power = dungeon_stats.trap_difficulty % 10;

        // div_power adjusts the starting amount of traps. Caps at 20.
        div_power = dungeon_stats.trap_difficulty / 10;
        if (div_power > 20)
        {
            div_power = 20;
        }

        // Function creates odds of getting a trap. FORMULA DOES NOT WORK VERY WELL. ):
        // At hit_power 20, mod_power 9, There is a 95.14% chance of getting a trap.
        trap_chance = Mathf.Pow(mod_power, (0.04f * mod_power) + 1.0f) + Mathf.Pow(div_power, (0.02f * div_power) + 1.0f) + 9.0f;


        // Create the rooms array with a random size.
        int ran = dungeon_stats.numRooms.Random;        // Save random number for further use.
        rooms = new Room[ran];
        Debug.Log("Num rooms: " + ran);
        // There should be one less corridor than there is rooms.
        corridors = new Corridor[rooms.Length - 1];

        loopable_corridors = new List<Corridor>();

        // Determine number of hidden corridors.
        for (int i = 0; i < ran; i++)
        {
            // 1 in 10 chance for every room that there will be a hidden room.
            if ((UnityEngine.Random.Range(1, 11)) == 10)
            {
                num_hidden++;
            }
        }

        Debug.Log("Hidden corridors: " + num_hidden);

        hidden_corridors = new HiddenCorridor[num_hidden];

        // Create the first room and corridor.
        rooms[0] = new Room();
        corridors[0] = new Corridor();

        // Setup the first room, there is no previous corridor so we do not use one.
        rooms[0].SetupRoom(dungeon_stats.roomWidth, dungeon_stats.roomHeight, dungeon_stats.columns, dungeon_stats.rows);

        // Setup the first corridor using the first room.
        corridors[0].SetupCorridor(rooms[0], dungeon_stats.corridorLength, dungeon_stats.roomWidth, dungeon_stats.roomHeight, dungeon_stats.columns, dungeon_stats.rows, true);
        

        for (int i = 1; i < rooms.Length; i++)
        {
            // Create a room.
            rooms[i] = new Room();

            // Setup the room based on the previous corridor.
            rooms[i].SetupRoom(dungeon_stats.roomWidth, dungeon_stats.roomHeight, dungeon_stats.columns, dungeon_stats.rows, corridors[i - 1], dungeon_stats.reward_chance, dungeon_stats.people_chance);

            // If we haven't reached the end of the corridors array...
            if (i < corridors.Length)
            {
                int x = i;
                int y = 1;
                // ... create a corridor.
                corridors[i] = new Corridor();

                // Setup the corridor based on the room that was just created.
                corridors[i].SetupCorridor(rooms[i], dungeon_stats.corridorLength, dungeon_stats.roomWidth, dungeon_stats.roomHeight, dungeon_stats.columns, dungeon_stats.rows, false);

                // Check if corridor goes into an existing room
                // If it does, then add it into "looping" corridors and make a new corridor to try again.
                while (CheckCorridors(x))
                {
                    corridors[i].ReSetupCorridor(rooms[x], dungeon_stats.corridorLength, dungeon_stats.roomWidth, dungeon_stats.roomHeight, dungeon_stats.columns, dungeon_stats.rows, y);
                    y++;
                    // If we have done a full rotation, choose a different room to branch from.
                    if (y == 4)
                    {
                        y = 1;
                        if (x == i)
                        {
                            x = Mathf.FloorToInt(x / (Random.Range(1, i)));
                        }
                    }
                }
            }
        }

        // Set up hidden rooms.
        // Hidden rooms are basically just corridors with a chest at the end.
        for (int i = 0; i < hidden_corridors.Length; i++)
        {
            int y = 1; // Used for changing the direction the corridor faces.

            // Create a room.
            hidden_corridors[i] = new HiddenCorridor();

            // Choose random exisiting room to branch off of
            int branch_from = UnityEngine.Random.Range(0, ran);

            // Setup the corridor based on the room chosen
            hidden_corridors[i].SetupCorridor(rooms[branch_from], dungeon_stats.corridorLength, dungeon_stats.roomWidth, dungeon_stats.roomHeight, dungeon_stats.columns, dungeon_stats.rows, false);

            // Check if corridor goes into an existing room
            // If it does, then add it into "looping" corridors and make a new corridor to try again.
            while (CheckHiddenCorridors(i))
            {
                hidden_corridors[i].ReSetupCorridor(rooms[branch_from], dungeon_stats.corridorLength, dungeon_stats.roomWidth, dungeon_stats.roomHeight, dungeon_stats.columns, dungeon_stats.rows, y);
                y++;
                // If we have done a full rotation, choose a different room to branch from.
                if (y == 4)
                {
                    y = 1;
                    branch_from = UnityEngine.Random.Range(0, ran);
                }
            }
        }

        //Forces to have at least 1 treasure or 1 villager in each dungeon
        for (int i = 1; i < rooms.Length; i++)
        {
            if (rooms[i].has_people == true || rooms[i].has_treasure == true)
            {
                return;
            }
        }

        IntRange room_choose = new IntRange(1, rooms.Length - 1);
        IntRange reward_choose = new IntRange(1, 2);
        rooms[room_choose.Random].ForceReward(reward_choose.Random);
    }

    // Used to check if corridors run into existing rooms.
    bool CheckCorridors(int i)
    {
        for (int x = 0; x <= i; x++)
        {
            if (corridors[i].EndPositionX > rooms[x].xPos + 1 && corridors[i].EndPositionX < rooms[x].xPos + rooms[x].roomWidth + 1 &&
                corridors[i].EndPositionY > rooms[x].yPos + 1 && corridors[i].EndPositionY < rooms[x].yPos + rooms[x].roomHeight + 1)
            {
                // Adds the corridor into the looping corridors array
                loopable_corridors.Add(corridors[i]);
                return true;
            }
        }
        return false;
    }


    //Used to check if hidden corridors run into existing rooms.
    bool CheckHiddenCorridors(int i)
    {
        for (int x = 0; x <= i; x++)
        {
            if (hidden_corridors[i].EndPositionX > rooms[x].xPos + 1 && hidden_corridors[i].EndPositionX < rooms[x].xPos + rooms[x].roomWidth + 1 &&
                hidden_corridors[i].EndPositionY > rooms[x].yPos + 1 && hidden_corridors[i].EndPositionY < rooms[x].yPos + rooms[x].roomHeight + 1)
            {
                return true;
            }
        }
        return false;
    }

    void SetTilesValuesForRooms()
    {
        // Go through all the rooms...
        for (int i = 0; i < rooms.Length; i++)
        {
            Room currentRoom = rooms[i];

            // ... and for each room go through it's width.
            for (int j = 0; j < currentRoom.roomWidth; j++)
            {
                int xCoord = currentRoom.xPos + j;

                // For each horizontal tile, go up vertically through the room's height.
                for (int k = 0; k < currentRoom.roomHeight; k++)
                {
                    int yCoord = currentRoom.yPos + k;

                    // The coordinates in the jagged array are based on the room's position and it's width and height.
                    tiles[xCoord][yCoord] = TileType.Floor;
                }
            }
        }
        for (int i = 0; i < rooms.Length; i++)
        {
            Room currentRoom = rooms[i];

            if (currentRoom.has_people)
            {
                tiles[currentRoom.people_xcoordinate][currentRoom.people_ycoordinate] = TileType.People;
            }

            if (currentRoom.has_treasure)
            {
                tiles[currentRoom.treasure_xcoordinate][currentRoom.treasure_ycoordinate] = TileType.Reward;
            }
        }
    }


    void SetTilesValuesForCorridors()
    {
        // Go through every corridor...

        // Regular corridors
        for (int i = 0; i < corridors.Length; i++)
        {
            Corridor currentCorridor = corridors[i];

            // and go through it's length.
            for (int j = 0; j < currentCorridor.corridorLength; j++)
            {
                // Start the coordinates at the start of the corridor.
                int xCoord = currentCorridor.startXPos;
                int yCoord = currentCorridor.startYPos;

                // Depending on the direction, add or subtract from the appropriate
                // coordinate based on how far through the length the loop is.
                switch (currentCorridor.direction)
                {
                    case Direction.North:
                        yCoord += j;
                        break;
                    case Direction.East:
                        xCoord += j;
                        break;
                    case Direction.South:
                        yCoord -= j;
                        break;
                    case Direction.West:
                        xCoord -= j;
                        break;
                }
                // If rolls a trap, set tile at coordinates to Trap.
                if (Random.Range(0.0f, 100.0f) <= trap_chance)
                {
                    tiles[xCoord][yCoord] = TileType.Trap;
                }
                // Else set the tile at these coordinates to Floor.
                else
                {
                    tiles[xCoord][yCoord] = TileType.Floor;
                }
            }
        }

        // Looping corridors
        for (int i = 0; i < loopable_corridors.Count; i++)
        {
            Corridor currentCorridor = loopable_corridors[i];
            
            for (int j = 0; j < currentCorridor.corridorLength; j++)
            {
                int xCoord = currentCorridor.startXPos;
                int yCoord = currentCorridor.startYPos;
                
                switch (currentCorridor.direction)
                {
                    case Direction.North:
                        yCoord += j;
                        break;
                    case Direction.East:
                        xCoord += j;
                        break;
                    case Direction.South:
                        yCoord -= j;
                        break;
                    case Direction.West:
                        xCoord -= j;
                        break;
                }
                if (Random.Range(0.0f, 100.0f) <= trap_chance)
                {
                    tiles[xCoord][yCoord] = TileType.Trap;
                }
                else
                {
                    tiles[xCoord][yCoord] = TileType.Floor;
                }
            }
        }

        // Hidden corridors
        for (int i = 0; i < hidden_corridors.Length; i++)
        {
            HiddenCorridor currentCorridor = hidden_corridors[i];

            for (int j = 0; j < currentCorridor.corridorLength; j++)
            {
                int xCoord = currentCorridor.startXPos;
                int yCoord = currentCorridor.startYPos;
                
                switch (currentCorridor.direction)
                {
                    case Direction.North:
                        yCoord += j;
                        break;
                    case Direction.East:
                        xCoord += j;
                        break;
                    case Direction.South:
                        yCoord -= j;
                        break;
                    case Direction.West:
                        xCoord -= j;
                        break;
                }

                // At the end of hidden corridor, put treasure chest.
                if (xCoord == currentCorridor.EndPositionX && yCoord == currentCorridor.EndPositionY)
                {
                    tiles[xCoord][yCoord] = TileType.HiddenReward;
                    Debug.Log("Hidden X: " + xCoord + " Hidden Y: " + yCoord);
                }
                // Much higher chance of having a trap.
                else if (Random.Range(0.0f, 100.0f) <= 80.0f)
                {
                    tiles[xCoord][yCoord] = TileType.HiddenTrap;
                }
                else
                {
                    tiles[xCoord][yCoord] = TileType.HiddenFloor;
                }
            }
        }
    }
    


    void InstantiateTiles()
    {
        // Go through all the tiles in the jagged array...
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                // If the tile type is Floor
                if (tiles[i][j] == TileType.Floor)
                {
                    // ... instantiate a floor
                    InstantiateFromArray(floorTiles, i, j);
                }

                // If the tile type is Trap
                else if (tiles[i][j] == TileType.Trap)
                {
                    // ... instantiate a floor
                    InstantiateFromArray(floorTiles, i, j);
                    // and then a trap over it
                    InstantiateFromArray(traps, i, j);
                }

                // If there's a reward
                else if (tiles[i][j] == TileType.Reward)
                {
                    InstantiateFromArray(floorTiles, i, j);

                    treasure.GetComponent<TreasureManager>().num_rewards = Random.Range(dungeon_stats.min_rewards, dungeon_stats.max_rewards);

                    Instantiate(treasure, new Vector3(i, j), Quaternion.identity);
                }

                // If there's people
                else if (tiles[i][j] == TileType.People)
                {
                    InstantiateFromArray(floorTiles, i, j);
                    Instantiate(citizens, new Vector3(i, j), Quaternion.identity);
                }

                // If it is a hidden floor
                else if (tiles[i][j] == TileType.HiddenFloor)
                {
                    InstantiateFromArray(floorTiles, i, j);
                    InstantiateFromArray(hidingTiles, i, j);
                }

                // If it is a hidden floor trap
                else if (tiles[i][j] == TileType.HiddenTrap)
                {
                    InstantiateFromArray(floorTiles, i, j);
                    InstantiateFromArray(traps, i, j);
                    InstantiateFromArray(hidingTiles, i, j);
                }

                // If it is a hidden treasure
                else if (tiles[i][j] == TileType.HiddenReward)
                {
                    InstantiateFromArray(floorTiles, i, j);
                    Instantiate(hiddenTreasure, new Vector3(i, j), Quaternion.identity);
                    InstantiateFromArray(hidingTiles, i, j);
                }

                // If the tile type is Wall...
                else if (tiles[i][j] == TileType.Wall)
                {
                    // ... instantiate a wall 
                    InstantiateFromArray(wallTiles, i, j);
                }
            }
        }
    }


    void InstantiateOuterWalls()
    {
        // The outer walls are one unit left, right, up and down from the board.
        float leftEdgeX = -1f;
        float rightEdgeX = dungeon_stats.columns + 0f;
        float bottomEdgeY = -1f;
        float topEdgeY = dungeon_stats.rows + 0f;

        // Instantiate both vertical walls (one on each side).
        InstantiateVerticalOuterWall(leftEdgeX, bottomEdgeY, topEdgeY);
        InstantiateVerticalOuterWall(rightEdgeX, bottomEdgeY, topEdgeY);

        // Instantiate both horizontal walls, these are one in left and right from the outer walls.
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, bottomEdgeY);
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, topEdgeY);
    }


    void InstantiateVerticalOuterWall(float xCoord, float startingY, float endingY)
    {
        // Start the loop at the starting value for Y.
        float currentY = startingY;

        // While the value for Y is less than the end value...
        while (currentY <= endingY)
        {
            if (currentY == 0 && xCoord == -1)
            {
                Instantiate(exit, new Vector3(-1, 0, 0), Quaternion.identity);
                InstantiateFromArray(outerWallTiles, -2, 0);
            }
            // ... instantiate an outer wall tile at the x coordinate and the current y coordinate.
            else
            {
                InstantiateFromArray(outerWallTiles, xCoord, currentY);
            }

            currentY++;
        }
    }


    void InstantiateHorizontalOuterWall(float startingX, float endingX, float yCoord)
    {
        // Start the loop at the starting value for X.
        float currentX = startingX;

        // While the value for X is less than the end value...
        while (currentX <= endingX)
        {
            // ... instantiate an outer wall tile at the y coordinate and the current x coordinate.
            InstantiateFromArray(outerWallTiles, currentX, yCoord);

            currentX++;
        }
    }


    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord)
    {
        // Create a random index for the array.
        int randomIndex = Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = boardHolder.transform;
    }
}