using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountQuadState : MonoBehaviour
{
    public int totalNone = 0;
    public int totalMiss = 0;
    public int totalHit = 0;
    public int totalShip = 0;

    void Start()
    {
        GridState();
    }

    public List<int> GridState()
    {
        foreach (Transform row in transform) 
        {
            foreach (Transform quad in row) 
            {
                Quad quadScript = quad.GetComponent<Quad>();
                if (quadScript != null && quadScript.state == Quad.quadState.miss) totalMiss++;
                else if (quadScript != null && quadScript.state == Quad.quadState.hit) totalHit++;
                else if (quadScript != null && quadScript.state == Quad.quadState.ship) totalShip++;
                
                totalNone++;
            }
        }
        
            totalNone = totalNone - totalMiss - totalHit - totalShip;
            Debug.Log($"totalNone: {totalNone}");
            Debug.Log($"totalMiss: {totalMiss}");
            Debug.Log($"totalHit: {totalHit}");
            Debug.Log($"totalShip: {totalShip}");

            return new List<int> {totalNone, totalMiss, totalHit, totalShip};
    }


    public void PlaceShipOnQuadPosition() {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
