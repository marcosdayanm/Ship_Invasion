using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GridMatrixController : MonoBehaviour
{
    private enum gridState { none, miss, ship, hit };
    [SerializeField] List<List<gridState>> gridMatrix = new List<List<gridState>>();
    

    void Start()
    {
        this.initializeGrid();


// Testing
        this.PlaceShip(0, 0, 0, 2);
        this.CountShips();

        this.ValidateAttack(0, 1, 0, 2);
        this.CountMisses();
        this.CountHits();
        this.CountShips();
    }

    void initializeGrid()
    {
        for (int i = 0; i < 12; i++)
        {
            gridMatrix.Add(new List<gridState>());
            for (int j = 0; j < 12; j++)
            {
                gridMatrix[i].Add(gridState.none);
            }
        }
        Debug.Log($"Grid Matrix Length: {gridMatrix.Count} x {gridMatrix[0].Count}");
    }


    int CountShips()
    {
        int count = 0;
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                if (gridMatrix[i][j] == gridState.ship)
                {
                    count++;
                }
            }
        }
        Debug.Log($"Number of ships: {count}");
        return count;
    }

    int CountMisses()
    {
        int count = 0;
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                if (gridMatrix[i][j] == gridState.miss)
                {
                    count++;
                }
            }
        }
        Debug.Log($"Number of misses: {count}");
        return count;
    }

    int CountHits()
    {
        int count = 0;
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                if (gridMatrix[i][j] == gridState.hit)
                {
                    count++;
                }
            }
        }
        Debug.Log($"Number of hits: {count}");
        return count;
    }

    void PlaceShip(int x1, int y1, int x2, int y2)
    {
        for (int i = x1; i <= x2; i++)
        {
            for (int j = y1; j <= y2; j++)
            {
                gridMatrix[i][j] = gridState.ship;
            }
        }
    }


    void ValidateAttack(int x1, int y1, int x2, int y2)
    {
        for (int i = x1; i <= x2; i++)
        {
            for (int j = y1; j <= y2; j++)
            {
                if (gridMatrix[i][j] == gridState.ship)
                {
                    gridMatrix[i][j] = gridState.hit;
                }
                else
                {
                    gridMatrix[i][j] = gridState.miss;
                }
            }
        }
    }

}
