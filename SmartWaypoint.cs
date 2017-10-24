using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartWaypoint : MonoBehaviour {

    public bool hasBlue = false;  //Whether or not the map calculates this waypoint as having a Blue need
    public bool hasGreen = false; //Whether or not the map calculates this waypoint as having a Green need
    public float blueChance; //Probability for this waypoint to have a Blue need in the future
    public float greenChance; //Probability for this waypoint to have a Green need in the future
    public float blueLife = 0; //The 'lifespan' of a Blue need at this waypoint
    public float greenLife = 0; //The 'lifespan' of a Green need at this wapoint
    public float degradeRateBlue; //The rate that a Blue needs 'life' decreases at this waypoint
    public float degradeRateGreen; //The rate that a Green needs 'life' decreases at this waypoint


    private CapsuleCollider col;  // Reference to the sphere collider trigger component.

    void Awake()

        //Determine and set needs available at this location
    {
        if (hasBlue == true)
        {
            blueLife = 1;
        }

        if (hasGreen == true)
        {
            greenLife = 1;
        }
    }

    void Update()

        //Degrade needs if degrading needs is set
    {
        if (GameObject.FindGameObjectWithTag("WaypointGrid").GetComponent<WaypointGrid>().degradingNeeds)
        {
            if (hasBlue)
            {
                blueLife = blueLife - degradeRateBlue;

                if (blueLife < 0)
                {
                    blueLife = 0;
                }

                if (blueLife == 0)
                {
                    hasBlue = false;
                }
            }

            if (hasGreen)
            {
                greenLife = greenLife - degradeRateGreen;

                if (greenLife < 0)
                {
                    greenLife = 0;
                }

                if (greenLife == 0)
                {
                    hasGreen = false;
                }
            }
        }
       
    }

    public void blueToggle() //function call to toggle availability of a need
    {
        if (! hasBlue){
            hasBlue = true;
        }
        else
        {
            hasBlue = false;
        }
    }

    public void greenToggle() //function call to toggle availability of a need
    {
        if (!hasGreen)
        {
            hasGreen = true;
        }
        else
        {
            hasGreen = false;
        }
    }

    //These function are for when the needs have been divorced from the waypoint itself.  Future implementation will detect and utilize a list of objects that are detected through a sphere collider.
    //And the CalculateRoute() function will direct the A.I. to the object itself, rather than the waypoint
    //The exit function exists to demonstrate how we can accomadate for needs that might move, such as needs that can be satisfied by moving characters
    void OnTriggerStay(Collider other) 
    {
        string tag;
        tag = other.gameObject.tag;

        if (tag == "blue") { hasBlue = true; }
        if (tag == "green") { hasGreen = true; }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (tag == "blue") { hasBlue = false; }
        if (tag == "green") { hasGreen = false; }
       
    }

    //These functions exist so that they can be called if we want to update the probabilities associated with a need appearing at a waypoint

    public void updateGreenChance(float a)
    {
        greenChance = a;
    }

    public void updateBlueChance(float a)
    {
        blueChance = a;
    }
}

