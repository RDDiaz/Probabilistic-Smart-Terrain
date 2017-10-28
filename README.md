# Probabilistic-Smart-Terrain
Probabilistic Smart Terrain Implementation in Unity

### What is Probabilistic Smart Terrain?

"Smart Terrain is a gaming artificial intelligence concept that where the environment guides the AI to its objectives.
The concept is that objects that meet a specific character need radiate signals across the map and the character then follows
the signal to the desired item. These mappings are usually created on a grid, filled with values.  The higher the value the 
closer the character is to the item it wants." (O'Rell 2012)

(Pictured: Left: A screen capture of "The Sims" video game / Right: Visualization of a Smart Terrain map
![smartterrain3](https://user-images.githubusercontent.com/28874711/32074106-5d54871c-ba66-11e7-81ae-7f865f7e607b.png)

### Unity Asset Package Project

The objective of this project was to develop and create an asset package that would allow its users to implement probabilistic smart terrain environments in Unity.  The asset package can serve as a testbed or as the foundation of a game project.  
The effect of Probabilistic Smart Terrain is achieved through the interaction of the package’s three scripts.  The PstAI script is attached to game object the user wishes to be the A.I.  The WaypointGrid script is attached to an empty game object with the “WaypointGrid” tag.  The waypointGrid functions as a parent object whose children are SmartWaypoints – objects which have the SmartWaypoint script attached and have the tag “Waypoint"

### Loading the Package in a New Project

To load the testbed environment that comes pre-loaded with the package first create a new project in Unity.  Once Unity has launched the new project select **Assets -> Import Package -> Custom Package**.  Navigate to where the package is stored and select it.  Select import all.  Once Unity has loaded the package, navigate to the project’s Assets folder in the project view.  Navigate to the pre-made scene as follows:  **Assets -> Core Functionality -> Scenes -> AssetPackageDemo**.  Double click or drag and drop to load the scene.   If another scene is already loaded it may be necessary to right click the AssetPackageDemo scene in the hierarchy and select “Set as Active Scene.”


(Pictured: Probabilistic Smart Terrain Asset package loaded into a scene with obstacles)
![smartterrain2](https://user-images.githubusercontent.com/28874711/32063683-d4384d26-ba45-11e7-9dc3-b8cc8aeada76.png)


### Script Specifics

**PstAI script:**  

This script determines the method of the AI movement within the waypoint grid system; movement options that utilize the grid system include “Random”, “Manual”, “PstPatrol” and “Pst”. This script is also responsible for assigning player need levels which are used in the SetRoute() function to determine the signal strength values associated with each waypoint.  Lastly, the script is also responsible for handling collision events between the AI and waypoints. These collisions are the method utilized in the package to simulate the AI collecting an object that satisfies one of its current needs. The script is designed to allow a user to set movement behaviors and values associated with them in the inspector.

(Pictured: Inspector view of game object with PstAI script attached) 

![smartterrain4](https://user-images.githubusercontent.com/28874711/32076327-935ee0f8-ba6d-11e7-8a2b-4d85c9c66bc0.png)


**SmartWaypoint script:**

The SmartWaypoint script contains information regarding the individual waypoints that make up the grid.  This information includes what needs can presently be satisfied at the individual waypoint, the probability that the waypoint will be able to satisfy a given need in the future, as well as the rate of degradation for needs at that waypoint location.  During Update() this script calculates and applies degradation to objects at its location according to the rate of degradation set by the user in the inspector.  Lastly, this script is responsible for “removing” a need (setting its existence value to false for that waypoint) when an AI collides with it.  The script is designed to allow the user to set all the initial values of the waypoint in the inspector.

(Pictured: Inspector view of game object with SmartWaypoint script attached)

![smartterrain6](https://user-images.githubusercontent.com/28874711/32076335-9941a0c8-ba6d-11e7-9b86-c943ca78c030.png)

**WaypointGrid script:**

The WaypointGrid is responsible for calculating the movement of an AI that has the PstAI script attached and that has the “Pst” movement option selected.  The script allows users to drag and drop waypoint objects in the inspector to create an array of transforms which are then stored in a 2d array called “Grid”.  The Grid represents a 2d tile view of the map.  The primary method of the class is CalculateRoute() which is called by the pstAI script attached to an AI character in the Unity environment.  When called the CalculateRoute() function first iterates through the Grid and checks the waypoints at the associated index locations for whether they contain resources that can satisfy a character’s needs.  This creates a map of 0’s and 1’s representing where character objectives are located and is done for each need.  The values in the objective array are then used to calculate the values of each “tile” on the 2d Grid by multiplying the characters need for an objective by the magnitude between the objective and the character and storing that information in a strength array for the need in question. The individual strength arrays are then combined into a single strength array representing the highest possible values for each tile. Lastly, a series of checks on the resulting strength array is used to determine the highest available tile to move to. The resulting values are then used to set a Vector3 which is returned to the character object and assigned as its destination. The script allows the user to set and create a WaypointGrid of any size in the inspector so long as the resulting Grids length is equal to its height. The script also allows for the user to set whether needs will change (degrade and ultimately disappear) overtime.

(Pictured: Inspector view of game object with WaypointGrid script attached) 

![smartterrain5](https://user-images.githubusercontent.com/28874711/32076332-96cdd1fe-ba6d-11e7-846f-7ee256d67241.png)



