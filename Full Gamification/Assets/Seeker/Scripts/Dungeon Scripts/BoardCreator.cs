using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    // The type of tile that will be laid in a specific position.
    public enum TileType
    {
        Wall, Floor, Trap, Reward, People
    }

    public int min_rewards = 1;
    public int max_rewards = 5;
    public int reward_level = 1;
    public int trap_difficulty = 1;                          // How hard the traps hit
    private int mod_power = 0;
    private int div_power = 0;
    private float trap_chance;                               // Actual chance of traps

    // MAY NEED MINIMUM AMOUNT OF REWARDS
    public float reward_chance = 1.0f;                             // Chance to contain reward
    public float people_chance = 1.0f;                             // chance to contain people

    public int columns = 100;                                 // The number of columns on the board (how wide it will be).
    public int rows = 100;                                    // The number of rows on the board (how tall it will be).
    public IntRange numRooms = new IntRange(15, 20);         // The range of the number of rooms there can be.
    public IntRange roomWidth = new IntRange(3, 10);         // The range of widths rooms can have.
    public IntRange roomHeight = new IntRange(3, 10);        // The range of heights rooms can have.
    public IntRange corridorLength = new IntRange(6, 10);    // The range of lengths corridors between rooms can have.
    public GameObject[] floorTiles;                           // An array of floor tile prefabs.
    public GameObject[] wallTiles;                            // An array of wall tile prefabs.
    public GameObject[] outerWallTiles;                       // An array of outer wall tile prefabs.
    public GameObject treasure;                            // Items found in dungeon
    public GameObject[] citizens;
    public GameObject[] traps;                                // Traps found in dungeon
    public GameObject exit;                                   // Exit


    private TileType[][] tiles;                               // A jagged array of tile types representing the board, like a grid.
    private Room[] rooms;                                     // All the rooms that are created for this board.
    private Corridor[] corridors;                             // All the corridors that connect the rooms.
    private GameObject boardHolder;                           // GameObject that acts as a container for all other tiles.

    private void Start()
    {
        // Create the board holder.
        boardHolder = new GameObject("BoardHolder");

        SetupTilesArray();

        CreateRoomsAndCorridors();

        SetTilesValuesForRooms();
        SetTilesValuesForCorridors();

        InstantiateTiles();
        InstantiateOuterWalls();
    }


    void SetupTilesArray()
    {
        // Set the tiles jagged array to the correct width.
        tiles = new TileType[columns][];

        // Go through all the tile arrays...
        for (int i = 0; i < tiles.Length; i++)
        {
            // ... and set each tile array is the correct height.
            tiles[i] = new TileType[rows];
        }
    }

    void CreateRoomsAndCorridors()
    {
        // Get Div and Mod of hit_power to adjust amount of traps

        // mod_power adjusts the exponential amount of traps
        mod_power = trap_difficulty % 10;

        // div_power adjusts the starting amount of traps. Caps at 20.
        div_power = trap_difficulty / 10;
        if (div_power > 20)
        {
            div_power = 20;
        }

        // Function creates odds of getting a trap. Maths by me. :D
        //At hit_power 20, mod_power 9, There is a 95.14% chance of getting a trap.
        trap_chance = Mathf.Pow(mod_power, (0.04f * mod_power) + 1.0f) + Mathf.Pow(div_power, (0.02f * div_power) + 1.0f) + 9.0f;


        // Create the rooms array with a random size.
        rooms = new Room[numRooms.Random];

        // There should be one less corridor than there is rooms.
        corridors = new Corridor[rooms.Length - 1];

        // Create the first room and corridor.
        rooms[0] = new Room();
        corridors[0] = new Corridor();

        // Setup the first room, there is no previous corridor so we do not use one.
        rooms[0].SetupRoom(roomWidth, roomHeight, columns, rows);

        // Setup the first corridor using the first room.
        corridors[0].SetupCorridor(rooms[0], corridorLength, roomWidth, roomHeight, columns, rows, true);

        for (int i = 1; i < rooms.Length; i++)
        {
            // Create a room.
            rooms[i] = new Room();

            // Setup the room based on the previous corridor.
            rooms[i].SetupRoom(roomWidth, roomHeight, columns, rows, corridors[i - 1], reward_chance, people_chance);

            // If we haven't reached the end of the corridors array...
            if (i < corridors.Length)
            {
                // ... create a corridor.
                corridors[i] = new Corridor();

                // Setup the corridor based on the room that was just created.
                corridors[i].SetupCorridor(rooms[i], corridorLength, roomWidth, roomHeight, columns, rows, false);
            }
        }

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
                // Else et the tile at these coordinates to Floor.
                else
                {
                    tiles[xCoord][yCoord] = TileType.Floor;
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

                    treasure.GetComponent<TreasureManager>().num_rewards = Random.Range(min_rewards, max_rewards);
                    treasure.GetComponent<TreasureManager>().rank = reward_level;

                    Instantiate(treasure, new Vector3(i, j), Quaternion.identity);
                }

                // If there's people
                else if (tiles[i][j] == TileType.People)
                {
                    InstantiateFromArray(floorTiles, i, j);
                    InstantiateFromArray(citizens, i, j);
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
        float rightEdgeX = columns + 0f;
        float bottomEdgeY = -1f;
        float topEdgeY = rows + 0f;

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