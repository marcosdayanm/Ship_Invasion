using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GridStateController : MonoBehaviour
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

    // función para colocar barcos en la cuadrícula
    public void PlaceShipMisile(CardDetails cardDetails, Transform startingQuad)
    {
        // checar si la carta es de ataque o de defensa
        bool isShip = cardDetails.CardType == "Defense";

        // se separa el nombre del quad en coordenadas x e y para poder hacer comparaciones
        string[] coordinatesToFind = startingQuad.name.Split(',');
        int xToFind = int.Parse(coordinatesToFind[0]);
        int yToFind = int.Parse(coordinatesToFind[1]);

        // longitudes del barco
        int xLength = cardDetails.LengthX;
        int yLength = cardDetails.LengthY;

        // Determinar si el barco es horizontal, vertical o cuadrado.
        bool isHorizontal = cardDetails.LengthX > 1;
        bool isVertical = cardDetails.LengthY > 1;
        bool isSquare = isHorizontal && isVertical;
        if (isSquare)
        {
            isHorizontal = false;
            isVertical = false;
        }

        // Activación de la búsqueda una vez encontrada la posición inicial.
        bool isActivatedSearch = false;
        
        // Iterar sobre cada fila del grid.
        foreach (Transform row in transform)
        {
            for (int i = row.childCount - 1; i >= 0; i--) // Itera de atrás hacia adelante
            {
                // se separan las coordenadas de cada quad iterado igual que con el de arriba
                Transform loopedQuad = row.GetChild(i);
                Quad quadScript = loopedQuad.GetComponent<Quad>();
                string[] coordinates = loopedQuad.name.Split(',');
                int x = int.Parse(coordinates[0]);
                int y = int.Parse(coordinates[1]);

                // Activar la búsqueda y ajustar los estados del primer quad.
                if (x == xToFind && y == yToFind)
                {
                        if (isShip)
                            quadScript.state = Quad.quadState.ship;
                        else 
                            quadScript.AdjustQuadState();
                    isActivatedSearch = true;
                    if (isHorizontal)
                        xLength--;
                    else if (isVertical)
                        yLength--;
                }

                // Continuar colocando el barco o misil mientras la búsqueda esté activada.
                else if (isActivatedSearch)
                {
                    if ((isHorizontal || isSquare) && xLength > 0 && y == yToFind)
                    {
                        if (isShip)
                            quadScript.state = Quad.quadState.ship;
                        else 
                            quadScript.AdjustQuadState();
                        xLength--;
                    }
                    else if (isVertical && yLength > 0 && x == xToFind)
                    {
                        if (isShip)
                            quadScript.state = Quad.quadState.ship;
                        else 
                            quadScript.AdjustQuadState();
                        yLength--;
                    }
                }

                // Terminar los loops si ya no hay más que buscar
                if ((isHorizontal && xLength <= 0) || (isVertical && yLength <= 0))
                    break;
            }
            // si es square se debe de reiniciar la xLength para que se pueda seguir iterando sobre las filas de abajo del cuadrado
            if (isSquare)
                xLength = cardDetails.LengthX;

            if ((isHorizontal && xLength <= 0) || ((isVertical || isSquare) && yLength <= 0))
                break;
        }
    }
}

