using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointGrid : MonoBehaviour
{

    public Transform[] waypointList; //List of waypoint locations associated with the Grid
    public GameObject[,] Grid;       //Array of gameobjects making up the Grid       
    public int gridLength;           //Length and/or Width of Grid 
    public bool degradingNeeds;      //Sets whether or not to calculate degrading needs of an object.  If an object degrades to 0 it is no longer available

    private float[,] distances;      //Stores the calculated distances between the A.I. and the objective (need)             
    private int[,] str1Array;        //str Arrays store calculated 'strengths' associated with each value based on distances
    private int[,] str2Array;
    private int[,] str3Array;
    private int [,]objective1;       //Map of where all instances of objective1 exist on the map
    private int [,]objective2;       //Map of where all instances of objective1 exist on the map
    private int objective1X;         //Stores coordinates for known objective locations
    private int objective1Y;         //Stores coordinates for known objective locations
    private int objective2X;         //Stores coordinates for known objective locations
    private int objective2Y;         //Stores coordinates for known objective locations
    private int characterX;          //Stores coordinates for A.I. location
    private int characterY;          //Stores coordinates for A.I. location
    private int counter;        
    private int[] square;            //Used to store values when comparing nearby 'tile' values
    private int[] squareX;           //Used to store coordinates when comparing nearby 'tile' values
    private int[] squareY;           //Used to store coordinates when comparing nearby 'tile' values
    private int finalX;              //Stores the optimal X coordinate to travel to
    private int finalY;              //Stores the optimal Y coordinate to travel to
    private int topLeft;             //Used as a check to indicate during route calculation that player is in the named square
    private int topRight;            //Used as a check to indicate during route calculation that player is in the named square
    private int bottomLeft;          //Used as a check to indicate during route calculation that player is in the named square
    private int bottomRight;         //Used as a check to indicate during route calculation that player is in the named square
    private int leftWall;            //Used as a check to indicate during route calculation that player is in the named square
    private int rightWall;           //Used as a check to indicate during route calculation that player is in the named square
    private int topWall;             //Used as a check to indicate during route calculation that player is in the named square
    private int bottomWall;          //Used as a check to indicate during route calculation that player is in the named square

    void Awake()
    {
        //Create refrences

        Transform[] waypointList = this.waypointList;
        Grid = new GameObject[gridLength, gridLength];
        int gridCounter = 0;

        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                Grid[i, j] = waypointList[gridCounter].gameObject;
                gridCounter++;
            }

        }

        /* For debugging
      print(Grid[0, 0].transform.position);
      print(Grid[0, 1].transform.position);
      print(Grid[0, 2].transform.position);
      print(Grid[1, 0].transform.position);
      print(Grid[1, 1].transform.position);
      print(Grid[1, 2].transform.position);
      print(Grid[2, 0].transform.position);
      print(Grid[2, 1].transform.position);
      print(Grid[2, 2].transform.position);
      */
    }

    // Calculate 'strength' based on distance between A.I. and instance of need location
    int CalculateSignalStrength(int x1, int y1, int x2, int y2)
    {
        int strength = 0;
        int resultx = 0;
        int resulty = 0;

        if (x1 > x2)
        {
            resultx = (x1 - x2) * (x1 - x2);
        }
        else if (x1 < x2)
        {
            resultx = (x2 - x1) * (x2 - x1);
        }

        if (y1 > y2)
        {
            resulty = (y1 - y2) * (y1 - y2);
        }
        else if (y1 < y2)
        {
            resulty = (y2 - y1) * (y2 - y1);

        }

        strength = Convert.ToInt32(Math.Sqrt(resulty + resultx));
        strength = (gridLength + (gridLength / 2)) - strength;


        if (strength < 0)
        {
            strength = 0;
        }
        return strength;
    }

    //Function call to allow degradingNeeds to be toggled on and off
    void ToggleChangingNeeds()
    {

        if (degradingNeeds == false)
        {
            degradingNeeds = true;
        }
        else
        {
            degradingNeeds = false;
        }
    }

    //This function calculates the optimal route for the A.I. to take based on the infomation provided by the Grid

    public Vector3 CalculateRoute(Vector3 myLocation, GameObject ai)
    {
        //Create refrences for this particular calculation

        Transform[] waypointList = this.waypointList;

        float min;
        int Testval;
        float temp;
        int indexX = 0;
        int indexY = 0;
        objective1X = 0;
        objective1Y = 0;
        objective2X = 0;
        objective2Y = 0;
        counter = 0;

        bool noResource1 = false;
        bool noResource2 = false;

        gridLength = Convert.ToInt16(Math.Sqrt(waypointList.Length));



        str1Array = new int[gridLength, gridLength];
        str2Array = new int[gridLength, gridLength];
        str3Array = new int[gridLength, gridLength];
        objective1 = new int[gridLength, gridLength];
        objective2 = new int[gridLength, gridLength];
        square = new int[9]; 
        squareX = new int[9]; 
        squareY = new int[9]; 
        distances = new float[gridLength, gridLength];
        GameObject pstAI = ai;
        PstAI pstAI1 = pstAI.GetComponent<PstAI>();

        /*  For debugging
      print(Grid[0, 0].transform.position);
      print(Grid[0, 1].transform.position);
      print(Grid[0, 2].transform.position);
      print(Grid[1, 0].transform.position);
      print(Grid[1, 1].transform.position);
      print(Grid[1, 2].transform.position);
      print(Grid[2, 0].transform.position);
      print(Grid[2, 1].transform.position);
      print(Grid[2, 2].transform.position);
      */

        //Determine and set objectives

        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                if (Grid[i, j].GetComponent<SmartWaypoint>().hasGreen == true)
                {
                    str1Array[i, j] += 1;
                    objective1[i,j] += 1;
                    
                }
                if (Grid[i, j].GetComponent<SmartWaypoint>().hasBlue == true)
                {
                    str2Array[i, j] += 1;
                    objective2[i, j] += 1;
                }
            }
        }

        //Find first instance of objectives

        for (int x = 0; x < gridLength; x++)
        {
            for (int y = 0; y < gridLength; y++)
            {
                if (objective1[x, y] > 0)
                {

                    objective1X = x;
                    objective1Y = y;
                    counter++;
                    break;

                }
                if (x == gridLength - 1 && y == gridLength - 1 && counter == 0)
                {
                    noResource1 = true;
                    
                }
            }
        }

        counter = 0;

        // Calculate and store signal strength

        for (int x = 0; x < gridLength; x++)
        {
            for (int y = 0; y < gridLength; y++)
            {
                if (noResource1 == true)
                {
                    str1Array[x, y] = 0;
                }

                else if (objective1[x, y] > 0)
                {
                    objective1X = x;
                    objective1Y = y;
                    str1Array[x, y] = pstAI1.greenNeed * (CalculateSignalStrength(x, y, objective1X, objective1Y));
                }

                else if (noResource1 == false)
                {
                    str1Array[x, y] = pstAI1.greenNeed * (CalculateSignalStrength(x, y, objective1X, objective1Y));
                }

            }
        }

        for (int x = 0; x < gridLength; x++)
        {
            for (int y = 0; y < gridLength; y++)
            {
                if (objective2[x, y] > 0)
                {
                    objective2X = x;
                    objective2Y = y;
                    counter++;
                    break;
                }
                if (x == gridLength - 1 && y == gridLength - 1 && counter == 0)
                {
                    noResource2 = true;
                    counter = 0;
                }
            }
        }

        counter = 0;

        for (int x = 0; x < gridLength; x++)
        {
            for (int y = 0; y < gridLength; y++)
            {
                if (noResource2 == true)
                {
                    str2Array[x, y] = 0;
                }

                else if (objective2[x, y] > 0)
                {
                    objective2X = x;
                    objective2Y = y;
                    str2Array[x, y] = pstAI1.blueNeed * (CalculateSignalStrength(x, y, objective2X, objective2Y));
                    
                }
                
                else if (noResource2 == false)
                {
                    str2Array[x, y] = pstAI1.blueNeed * (CalculateSignalStrength(x, y, objective2X, objective2Y));
                }
                
            }
        }

        //Check if all resources are absent

        int maxVal1 = str1Array[0,0];
        for (int i = 0; i < gridLength; i++)
        {
            for(int j = 0; j < gridLength; j++)
            {
                if (str1Array[i, j] > maxVal1)
                {
                    maxVal1 = str1Array[i, j];
                }
                    
            }
        }

        int maxVal2 = str2Array[0, 0];
        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                if (str2Array[i, j] > maxVal2)
                {
                    maxVal2 = str2Array[i, j];
                }
            }
        }

        /* Verify str1array map
        for (int x = 0; x <= 2; x++)
        {
            for (int y = 0; y <= 2; y++)
        {
                print(str1Array[x, y]);
            }
        }
        */

        /*Verify Objective Maps
        for (int x = 0; x <= 2; x++)
        {
            for (int y = 0; y <= 2; y++)
            {
                print(objective2[x, y]);
            }
        }
        */


        //Create a grid that represents waypoint distances from player

        for (int x = 0; x < gridLength; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                temp = (myLocation - Grid[x, y].transform.position).magnitude;
                distances[x, y] = (myLocation - Grid[x, y].transform.position).magnitude;
                }
            }

        min = distances[0, 0];

        //Find waypoint with shortest distance and store the index numbers
        for (int x =0; x < gridLength; x++)
        {
                for (int y = 0; y <= 2; y++)
                {
                    if (distances[x, y] < min){
                                            
                             min = distances[x,y];
                             indexX = x;
                             indexY = y;
                         }                                            
                    }
        }

    

        // Set minimum value index to character location to represent character on 2d Grid
    
        characterX = indexX;  
        characterY = indexY;

        //print("I'm at " + "X: " + characterX + " Y:  " + characterY);

        //If map contains no resources calls can be made here to change player's movement type
        if (maxVal1 == 0 && maxVal2 == 0)
        {

            //print("MAP EMPTY!)");

        }


        //Combine signal strength graphs
        for (int x = 0; x < gridLength; x++)
        {
            for (int y = 0; y < gridLength; y++)
            {
                if (str1Array[x, y] > str2Array[x, y])
                {
                    str3Array[x, y] = str1Array[x, y];
                }

                else if (str2Array[x, y] > str1Array[x, y])
                {
                    str3Array[x, y] = str2Array[x, y];
                }

                else if (str2Array[x, y] == str1Array[x, y])
                {
                    //Tiebreaker case
                    str3Array[x, y] = str1Array[x, y];
                }
            }
        }

        //print("str1 array 0,0  = " + str1Array[0, 0]);
        //print("str2 array 0,0  = " + str2Array[0, 0]);

        //Print out the final strength array to the console - this array is what is used in the final set of calculations
        for (int x = 0; x < gridLength; x++)
        {
            for (int y = 0; y < gridLength; y++)
            {
                print("str3Array" + str3Array[x, y]);
            }
        }


        // bottom left corner check

        if (characterY == 0 && characterX == gridLength-1)
        {

            square[0] = str3Array[characterX-1, characterY];
            squareX[0] = characterX-1;
            squareY[0] = characterY;
            square[1] = str3Array[characterX - 1, characterY + 1];
            squareX[1] = characterX - 1;
            squareY[1] = characterY + 1;
            square[2] = str3Array[characterX, characterY+1];
            squareX[2] = characterX;
            squareY[2] = characterY + 1;
            square[3] = str3Array[characterX, characterY];
            squareX[3] = characterX;
            squareY[3] = characterY;
            Testval = square[0];
            finalX = squareX[0];
            finalY = squareY[0];
            bottomLeft = 1;


            for (int x = 1; x <= 3; x++)
            {
                if (Testval < square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
                else if (Testval == square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
            }
        }


        // top left corner check
        if (characterY == 0 && characterX == 0)
        {
            square[0] = str3Array[characterX + 1, characterY];
            squareX[0] = characterX + 1;
            squareY[0] = characterY;
            square[1] = str3Array[characterX + 1, characterY + 1];
            squareX[1] = characterX + 1;
            squareY[1] = characterY + 1;
            square[2] = str3Array[characterX, characterY + 1];
            squareX[2] = characterX;
            squareY[2] = characterY + 1;
            square[3] = str3Array[characterX, characterY];
            squareX[3] = characterX;
            squareY[3] = characterY;
            topLeft = 1;

            Testval = square[0];
            finalX = squareX[0];
            finalY = squareY[0];
            for (int x = 1; x <= 3; x++)
            {
                if (Testval < square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
                else if (Testval == square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
            }
        }
        // top right corner check
        if (characterY == gridLength - 1 && characterX == 0)
        {

            square[0] = str3Array[characterX + 1, characterY];
            squareX[0] = characterX + 1;
            squareY[0] = characterY;
            square[1] = str3Array[characterX, characterY - 1];
            squareX[1] = characterX;
            squareY[1] = characterY - 1;
            square[2] = str3Array[characterX + 1, characterY - 1];
            squareX[2] = characterX + 1;
            squareY[2] = characterY - 1;
            square[3] = str3Array[characterX, characterY];
            squareX[3] = characterX;
            squareY[3] = characterY;
            topRight = 1;

            Testval = square[0];
            finalX = squareX[0];
            finalY = squareY[0];
            for (int x = 1; x <= 3; x++)
            {
                if (Testval < square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
                else if (Testval == square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
            }
        }
        // bottom right corner check
        if (characterY == gridLength - 1 && characterX == gridLength - 1)
        {
            square[0] = str3Array[characterX - 1, characterY - 1];
            squareX[0] = characterX - 1;
            squareY[0] = characterY - 1;
            square[1] = str3Array[characterX, characterY - 1];
            squareX[1] = characterX;
            squareY[1] = characterY - 1;
            square[2] = str3Array[characterX - 1, characterY];
            squareX[2] = characterX - 1;
            squareY[2] = characterY;
            square[3] = str3Array[characterX, characterY];
            squareX[3] = characterX;
            squareY[3] = characterY;
            bottomRight = 1;

            Testval = square[0];
            finalX = squareX[0];
            finalY = squareY[0];
            for (int x = 1; x <= 3; x++)
            {
                if (Testval < square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
                else if (Testval == square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
            }
        }

        // left wall check
        if (characterX < gridLength -1&& characterX > 0 && characterY == 0 && bottomLeft == 0 && topLeft == 0)
        {
            square[0] = str3Array[characterX + 1, characterY + 1];
            squareX[0] = characterX;
            squareY[0] = characterY + 1;
            square[1] = str3Array[characterX + 1, characterY + 1];
            squareX[1] = characterX + 1;
            squareY[1] = characterY + 1;
            square[2] = str3Array[characterX + 1, characterY];
            squareX[2] = characterX + 1;
            squareY[2] = characterY;
            square[3] = str3Array[characterX -1, characterY];
            squareX[3] = characterX - 1;
            squareY[3] = characterY;
            square[4] = str3Array[characterX - 1, characterY + 1];
            squareX[4] = characterX - 1;
            squareY[4] = characterY + 1;
            square[5] = str3Array[characterX, characterY];
            squareX[5] = characterX;
            squareY[5] = characterY;
            Testval = square[0];
            finalX = squareX[0];
            finalY = squareY[0];
            leftWall = 1;

            for (int x = 1; x <= 5; x++)
            {
                if (Testval < square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
                else if (Testval == square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
            }
        }

        //right wall check
        if (characterX < gridLength-1 && characterX > 0 && characterY == gridLength-1 && bottomRight == 0 && topRight == 0)
        {
            square[0] = str3Array[characterX, characterY - 1];
            squareX[0] = characterX;
            squareY[0] = characterY - 1;
            square[1] = str3Array[characterX - 1, characterY - 1];
            squareX[1] = characterX - 1;
            squareY[1] = characterY - 1;
            square[2] = str3Array[characterX - 1, characterY];
            squareX[2] = characterX - 1;
            squareY[2] = characterY;
            square[3] = str3Array[characterX + 1, characterY - 1];
            squareX[3] = characterX + 1;
            squareY[3] = characterY - 1;
            square[4] = str3Array[characterX+1, characterY];
            squareX[4] = characterX + 1;
            squareY[4] = characterY;
            square[5] = str3Array[characterX, characterY];
            squareX[5] = characterX;
            squareY[5] = characterY;
            Testval = square[0];
            finalX = squareX[0];
            finalY = squareY[0];
            rightWall = 1;

            for (int x = 1; x <= 5; x++)
            {
                if (Testval < square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
                else if (Testval == square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
            }
        }

        //top wall check
        if (characterX == 0 && topRight == 0 && topLeft == 0)
        {
            square[0] = str3Array[characterX + 1, characterY];
            squareX[0] = characterX + 1;
            squareY[0] = characterY;
            square[1] = str3Array[characterX + 1, characterY + 1];
            squareX[1] = characterX + 1;
            squareY[1] = characterY + 1;
            square[2] = str3Array[characterX, characterY + 1];
            squareX[2] = characterX;
            squareY[2] = characterY + 1;
            square[3] = str3Array[characterX + 1, characterY - 1];
            squareX[3] = characterX + 1;
            squareY[3] = characterY - 1;
            square[4] = str3Array[characterX, characterY - 1];
            squareX[4] = characterX;
            squareY[4] = characterY - 1;
            square[5] = str3Array[characterX, characterY];
            squareX[5] = characterX;
            squareY[5] = characterY;
            Testval = square[0];
            finalX = squareX[0];
            finalY = squareY[0];
            topWall = 1;

            for (int x = 1; x <= 5; x++)
            {
                if (Testval < square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
                else if (Testval == square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
            }
        }

        // bottom wall check
        if (characterX == gridLength-1 && bottomRight == 0 && bottomLeft == 0)
        {
            square[0] = str3Array[characterX, characterY - 1];
            squareX[0] = characterX;
            squareY[0] = characterY - 1;
            square[1] = str3Array[characterX - 1, characterY - 1];
            squareX[1] = characterX - 1;
            squareY[1] = characterY - 1;
            square[2] = str3Array[characterX - 1, characterY];
            squareX[2] = characterX - 1;
            squareY[2] = characterY;
            square[3] = str3Array[characterX, characterY + 1];
            squareX[3] = characterX;
            squareY[3] = characterY + 1;
            square[4] = str3Array[characterX - 1, characterY + 1];
            squareX[4] = characterX - 1;
            squareY[4] = characterY + 1;
            square[5] = str3Array[characterX, characterY];
            squareX[5] = characterX;
            squareY[5] = characterY;
            Testval = square[0];
            finalX = squareX[0];
            finalY = squareY[0];
            bottomWall = 1;

            for (int x = 1; x <= 5; x++)
            {
                if (Testval < square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
                else if (Testval == square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
            }
        }

        // general check
        if (topLeft == 0 && topRight == 0 && bottomLeft == 0 && bottomRight == 0 &&
           bottomWall == 0 && topWall == 0 && rightWall == 0 && leftWall == 0)
        {
            square[0] = str3Array[characterX - 1, characterY - 1];
            squareX[0] = characterX - 1;
            squareY[0] = characterY - 1;
            square[1] = str3Array[characterX, characterY - 1];
            squareX[1] = characterX;
            squareY[1] = characterY - 1;
            square[2] = str3Array[characterX + 1, characterY - 1];
            squareX[2] = characterX + 1;
            squareY[2] = characterY - 1;
            square[3] = str3Array[characterX + 1, characterY];
            squareX[3] = characterX + 1;
            squareY[3] = characterY;
            square[4] = str3Array[characterX + 1, characterY + 1];
            squareX[4] = characterX + 1;
            squareY[4] = characterY + 1;
            square[5] = str3Array[characterX, characterY + 1];
            squareX[5] = characterX;
            squareY[5] = characterY + 1;
            square[6] = str3Array[characterX - 1, characterY + 1];
            squareX[6] = characterX - 1;
            squareY[6] = characterY + 1;
            square[7] = str3Array[characterX - 1, characterY];
            squareX[7] = characterX - 1;
            squareY[7] = characterY;
            square[8] = str3Array[characterX, characterY];
            squareX[8] = characterX;
            squareY[8] = characterY;

            Testval = square[0];
            finalX = squareX[0];
            finalY = squareY[0];

            for (int x = 1; x <= 8; x++)
            {
                if (Testval < square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
                else if (Testval == square[x])
                {
                    Testval = square[x];
                    finalX = squareX[x];
                    finalY = squareY[x];
                }
            }
        }

        characterX = finalX;
        characterY = finalY;

        //print("I am going to " + "X: " + finalX + " Y: " + finalY);

        //Convert highest value index to a 3d position associated with a wayPoint
        Vector3 bestDestination = Grid[finalX, finalY].transform.position;

        //Reset stored values for my own personal sanity...

        topLeft = 0;
        topRight = 0;
        bottomLeft = 0;
        bottomRight = 0;
        bottomWall = 0;
        topWall = 0;
        rightWall = 0;
        leftWall = 0;

        return bestDestination;

    }
}
