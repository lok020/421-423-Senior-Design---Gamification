using UnityEngine;

public class Room
{
    public int xPos;                      // The x coordinate of the lower left tile of the room.
    public int yPos;                      // The y coordinate of the lower left tile of the room.
    public int roomWidth;                     // How many tiles wide the room is.
    public int roomHeight;                    // How many tiles high the room is.
    public Direction enteringCorridor;    // The direction of the corridor that is entering this room.
    public bool has_treasure;               
    public float reward_chance;               // 0 - 10. Higher number means greater chance of reward
    public int treasure_xcoordinate;
    public int treasure_ycoordinate;
    public bool has_people;
    public float people_chance;              // 0 - 100.
    public int people_xcoordinate;
    public int people_ycoordinate;

    private int col;
    private int row;


    // This is used for the first room.  It does not have a Corridor parameter since there are no corridors yet.
    public void SetupRoom(IntRange widthRange, IntRange heightRange, int columns, int rows)
    {
        // Set a random width and height.
        roomWidth = widthRange.Random;
        roomHeight = heightRange.Random;

        // Set the x and y coordinates so the room is always bottom left.
        xPos = 0;
        yPos = 0;
    }


    // This is an overload of the SetupRoom function and has a corridor parameter that represents the corridor entering the room.
    public void SetupRoom(IntRange widthRange, IntRange heightRange, int columns, int rows, Corridor corridor, float t_chance, float p_chance)
    {
        col = columns;
        row = rows;

        reward_chance = t_chance;
        people_chance = p_chance;
        // Set the entering corridor direction.
        enteringCorridor = corridor.direction;

        // Set random values for width and height.
        roomWidth = widthRange.Random;
        roomHeight = heightRange.Random;


        switch (corridor.direction)
        {
            // If the corridor entering this room is going north...
            case Direction.North:
                // ... the height of the room mustn't go beyond the board so it must be clamped based
                // on the height of the board (rows) and the end of corridor that leads to the room.
                roomHeight = Mathf.Clamp(roomHeight, 1, rows - corridor.EndPositionY);

                // The y coordinate of the room must be at the end of the corridor (since the corridor leads to the bottom of the room).
                yPos = corridor.EndPositionY;

                // The x coordinate can be random but the left-most possibility is no further than the width
                // and the right-most possibility is that the end of the corridor is at the position of the room.
                xPos = Random.Range(corridor.EndPositionX - roomWidth + 1, corridor.EndPositionX);

                // This must be clamped to ensure that the room doesn't go off the board.
                xPos = Mathf.Clamp(xPos, 0, columns - roomWidth);
                break;
            case Direction.East:
                roomWidth = Mathf.Clamp(roomWidth, 1, columns - corridor.EndPositionX);
                xPos = corridor.EndPositionX;

                yPos = Random.Range(corridor.EndPositionY - roomHeight + 1, corridor.EndPositionY);
                yPos = Mathf.Clamp(yPos, 0, rows - roomHeight);
                break;
            case Direction.South:
                roomHeight = Mathf.Clamp(roomHeight, 1, corridor.EndPositionY);
                yPos = corridor.EndPositionY - roomHeight + 1;

                xPos = Random.Range(corridor.EndPositionX - roomWidth + 1, corridor.EndPositionX);
                xPos = Mathf.Clamp(xPos, 0, columns - roomWidth);
                break;
            case Direction.West:
                roomWidth = Mathf.Clamp(roomWidth, 1, corridor.EndPositionX);
                xPos = corridor.EndPositionX - roomWidth + 1;

                yPos = Random.Range(corridor.EndPositionY - roomHeight + 1, corridor.EndPositionY);
                yPos = Mathf.Clamp(yPos, 0, rows - roomHeight);
                break;
        }

        // Chance of reward room only if not in first room.
        if (xPos > roomWidth || yPos > roomHeight)
        {
            float x = Random.Range(0.0f, 10.0f);
            if (x >= reward_chance)
            {
                has_treasure = false;
            }
            else
            {
                has_treasure = true;
                treasure_xcoordinate = Random.Range(xPos, xPos + Mathf.Clamp(roomWidth, 1, columns));
                treasure_ycoordinate = Random.Range(yPos, yPos + Mathf.Clamp(roomHeight, 1, rows));
                //Debug.Log("Has treasure at" + treasure_xcoordinate + " " + treasure_ycoordinate);
            }

            x = Random.Range(0.0f, 10.0f);
            if (x >= people_chance || GlobalControl.Instance.current_population >= GlobalControl.Instance.max_population)
            {
                has_people = false;
            }
            else
            {
                has_people = true;
                people_xcoordinate = Random.Range(xPos, xPos + Mathf.Clamp(roomWidth, 1, columns));
                people_ycoordinate = Random.Range(yPos, yPos + Mathf.Clamp(roomHeight, 1, rows));
                while (people_xcoordinate == treasure_xcoordinate && people_ycoordinate == treasure_ycoordinate)
                {
                    people_xcoordinate = Random.Range(xPos, xPos + Mathf.Clamp(roomWidth, 1, columns));
                    people_ycoordinate = Random.Range(yPos, yPos + Mathf.Clamp(roomHeight, 1, rows));
                }
                //Debug.Log("Has people at" + people_xcoordinate + " " + people_ycoordinate);
            }
        }

        
    }

    public void ForceReward(int type)
    {
        if (type == 1)
        { 
            has_treasure = true;
            treasure_xcoordinate = Random.Range(xPos + roomWidth - 1, xPos + Mathf.Clamp(roomWidth, 1, col));
            treasure_ycoordinate = Random.Range(yPos + roomHeight - 1, yPos + Mathf.Clamp(roomHeight, 1, row));
            //Debug.Log("Now has treasure at" + treasure_xcoordinate + " " + treasure_ycoordinate);
        }
        else if (type == 2)
        {
            has_people = true;
            people_xcoordinate = Random.Range(xPos + roomWidth - 1, xPos + Mathf.Clamp(roomWidth, 1, col));
            people_ycoordinate = Random.Range(yPos + roomHeight - 1, yPos + Mathf.Clamp(roomHeight, 1, row));
            //Debug.Log("Now has people at" + people_xcoordinate + " " + people_ycoordinate);
        }
    }
}