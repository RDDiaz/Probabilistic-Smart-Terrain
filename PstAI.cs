using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PstAI : MonoBehaviour
{

    private UnityEngine.AI.NavMeshAgent nav;                //Refrence for navagent 
    private GameObject[] probList;                          //List of probabilities associated with each waypoint - Used with pstPatrol movement option
    private Vector3 currentTarget;                          //Used to store the current targets positon
    private int gridSize;                                   //Total number of 'tiles' on the grid (Or can be thought as of the number of waypoints that make up the Grid)
    private int wayPointIndex;								//A counter for the way point array. 
    private float patrolTimer;								//A timer for the patrolWaitTime
    private bool onMyWay = false;                           //Used to prevent A.I. from changing paths unexpectedly
    public float patrolWaitTime = 1f;                       //The amount of time to wait when the patrol way point is reached.
    public Transform[] patrolWayPoints;                     //An array of transforms for the manual patrol route movement option.

    //Intial Route Options
    public bool randomized;
    public bool manual;
    public bool pstPatrol;
    public bool pst;

    //Needs 
    public string primaryNeed = "Green";                    //primaryNeed is used to determine the order of the probability list for pstPatrol, default set to Green
    public int greenNeed=10;  
    public int blueNeed=9;

    //Physics
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float sneakSpeed = .5f;

    //Other
    int counter;                                            //Counter

    void Awake()
    {
        //Setting up the references and intializing variables/lists

        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        gridSize = (GameObject.Find("WaypointGrid").GetComponent<WaypointGrid>().gridLength * GameObject.Find("WaypointGrid").GetComponent<WaypointGrid>().gridLength);
        probList = GameObject.FindGameObjectsWithTag("Waypoint");
        

        //Create appropriate probList

        if (primaryNeed == "blue" || primaryNeed == "Blue")
        {
            for (var i = 0; i < gridSize-1; i++)
            {
                for (var j = gridSize-1; j > i; j--)
                {
                    GameObject temp;

                    if (probList[j].GetComponent<SmartWaypoint>().blueChance < probList[j - 1].GetComponent<SmartWaypoint>().blueChance)
                    {
                        temp = probList[j];
                        probList[j] = probList[j - 1];
                        probList[j - 1] = temp;
                    }
                }
            }
        }

        if (primaryNeed == "green" || primaryNeed == "Green")
        {
            for (var i = 0; i < gridSize-1; i++)
            {
                for (var j = gridSize-1; j > i; j--)
                {
                    GameObject temp;

                    if (probList[j].GetComponent<SmartWaypoint>().greenChance < probList[j - 1].GetComponent<SmartWaypoint>().greenChance)
                    {
                        temp = probList[j];
                        probList[j] = probList[j - 1];
                        probList[j - 1] = temp;
                    }
                }
            }
        }
    }

     void Update()
    {
        //Check if A.I. has arrived at destination

        if (this.transform.position.x == currentTarget.x && this.transform.position.z == currentTarget.z)
        {
            onMyWay = false;
        }

        //Determine and execute movement type 

        if (randomized && onMyWay == false)
        {
            //Create a vector randomized currentTarget
            currentTarget = GameObject.Find("WaypointGrid").GetComponent<WaypointGrid>().waypointList[Random.Range(0, gridSize)].position;
            nav.destination = currentTarget;
            onMyWay = true;
        }

        else if (manual)
        {

            //Set an appropriate speed for the NavMeshAgent.
            nav.speed = walkSpeed;

                //... increment the timer.
                patrolTimer += Time.deltaTime;

                //If the timer exceeds the wait time...
                if (patrolTimer >= patrolWaitTime)
                {
                    //... increment the wayPointIndex.
                    if (wayPointIndex == patrolWayPoints.Length - 1)
                        wayPointIndex = 0;
                    else
                        wayPointIndex++;

                    //Reset the timer.
                    patrolTimer = 0;
                }

             else
                //If not near a destination, reset the timer.
                patrolTimer = 0;

            //Set the destination to the patrolWayPoint.
            nav.destination = patrolWayPoints[wayPointIndex].position;
        }

        else if (pstPatrol && onMyWay == false)
        {

            nav.destination = probList[(gridSize - 1) - counter].transform.position;
            print((gridSize - 1) - counter);
            currentTarget = probList[(gridSize-1) - counter].transform.position;
            counter++;
            print(counter);
            onMyWay = true;
           

            if(counter == gridSize)
            {
                counter = 0;
            }
        }

        else if (pst && onMyWay == false && GameObject.FindGameObjectWithTag("WaypointGrid").GetComponent<WaypointGrid>().degradingNeeds == false)
        {
            SetPath();
            currentTarget = nav.destination;
            onMyWay = true;
        }
        else if (pst && onMyWay == false && GameObject.FindGameObjectWithTag("WaypointGrid").GetComponent<WaypointGrid>().degradingNeeds == true)
        {
            SetPath();
        }

    }

    //Function call to set primaryNeeds

    void CalculateNeed()
    {

        if (greenNeed > blueNeed)
                    {
                        primaryNeed = "green";
                    }
        else
                    {
                        primaryNeed = "blue";
                    }                  
    }

    //Function call to have waypointGrid calculate and return route

    void SetPath()
    {
        //Set destination of the player to the highest value waypoint.
        nav.destination = GameObject.Find("WaypointGrid").GetComponent<WaypointGrid>().CalculateRoute(transform.position, gameObject);
    }

    //Modifiy needs values when player collides with green and blue objects (Or rather, in the current implementation, the waypoints that represent them)

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Waypoint")
        {

            if (other.gameObject.GetComponent<SmartWaypoint>().hasGreen)
            {
                //greenNeed = 0;
                //print("Got Green");
                onMyWay = false;
                other.gameObject.GetComponent<SmartWaypoint>().hasGreen = false;
            }
            else if (other.gameObject.GetComponent<SmartWaypoint>().hasBlue)
            {
                //blueNeed = 0;
                //print("Got Blue");
                onMyWay = false;
                other.gameObject.GetComponent<SmartWaypoint>().hasBlue = false;
            }
            
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(this.transform.position.x == currentTarget.x && this.transform.position.z == currentTarget.z)
        {
            onMyWay = false;
        }
    }



}
      
