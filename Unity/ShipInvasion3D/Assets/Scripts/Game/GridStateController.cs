using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GridStateController : MonoBehaviour
{
    // Variables para almacenar el estado del grid 
    public int totalNone = 0;
    public int totalMiss = 0;
    public int totalHit = 0;
    public int totalShip = 0;

    // Variable que guarda la cantidad de casillas que fueron atacadas en un turno
    public int totalAttacked = 0;

    void Start()
    {
        GridState();
    }

    // NOTA: Tal vez debamos reiniciar las cariables de conteo de los estados, porque si no se contaran varias veces
    // Función para obtener el estado del grid, se recorre todos los quads del grid y se cuenta cuantos hay de cada tipo (para mandarlos a UI)
    public List<int> GridState()
    {

        // reiniciar las variables de conteo
        totalNone = 0;
        totalMiss = 0;
        totalHit = 0;
        totalShip = 0;

        // loop de todos los rows dentro del gird
        foreach (Transform row in transform) 
        {
            // loop de todos los quads dentro de cada row
            foreach (Transform quad in row) 
            {
                // se obtiene el script de cada quad para poder acceder a su estado, y se cuenta cuantos hay de cada tipo
                Quad quadScript = quad.GetComponent<Quad>();
                if (quadScript != null && quadScript.state == Quad.quadState.miss) totalMiss++;
                else if (quadScript != null && quadScript.state == Quad.quadState.hit) totalHit++;
                else if (quadScript != null && quadScript.state == Quad.quadState.ship) totalShip++;
                
                totalNone++;
            }
        }
            // se calculan los totalNone y se manda a imprimir en consola
            totalNone = totalNone - totalMiss - totalHit - totalShip;
            // Debug.Log($"totalNone: {totalNone}");
            // Debug.Log($"totalMiss: {totalMiss}");
            // Debug.Log($"totalHit: {totalHit}");
            // Debug.Log($"totalShip: {totalShip}");

            // se regresan los valores para poder mandarlos a la UI
            return new List<int> {totalNone, totalMiss, totalHit, totalShip};
    }

    // Función para colocar barcos o misiles en cuadrícula y cambiar el estado de todos los quads en donde se situ el barco o misil, a ésta función se le pasa un objeto de la carta jugada, y se le pasa el quad donde se situará el barco o misil
    public int PlaceShipMisile(CardDetails cardDetails, Transform startingQuad)
    {
        // checar si la carta es de ataque o de defensa
        bool isShip = cardDetails.CardType == "Defense";

        int numFieldsCovered = 0;

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

        // Activación de la búsqueda una vez encontrada la posición inicial.
        bool isActivatedSearch = false;
        
        // Iterar sobre cada fila del grid.
        foreach (Transform row in transform)
        {
            // se itera de forma descendente sobre cada quad de cada row porque tenemos definido el anchor point en la esquina superior derecha del grid
            for (int i = row.childCount - 1; i >= 0; i--) // Itera de atrás hacia adelante
            {
                // se separan las coordenadas de cada quad iterado igual que con el quad inical
                Transform loopedQuad = row.GetChild(i);
                Quad quadScript = loopedQuad.GetComponent<Quad>();
                string[] coordinates = loopedQuad.name.Split(',');
                int x = int.Parse(coordinates[0]);
                int y = int.Parse(coordinates[1]);

                // Si llegamos al quad inicial, se coloca la primera posición de la carta y se comienza a buscar en base a la coordenada extraída del quad inicial
                if (x == xToFind && y == yToFind)
                {
                    // acción depoendiendo de si es barco o misil
                    if (isShip)
                        quadScript.state = Quad.quadState.ship;
                    else 
                        quadScript.AdjustQuadState();

                    // se activa la flag de búsqueda
                    isActivatedSearch = true;

                    // dependiendo de las características del barco se ajustan las longitudes pendientes a buscar
                    if (isHorizontal)
                        xLength--;
                    else if (isVertical)
                        yLength--;
                }

                // Continuar colocando el barco o misil mientras la búsqueda esté activada.
                else if (isActivatedSearch)
                {
                    if ((isHorizontal) && xLength > 0 && y == yToFind)
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

                if (quadScript.state == Quad.quadState.hit) numFieldsCovered++;

                // Terminar los loops si ya no hay más que buscar
                if ((isHorizontal && xLength <= 0) || (isVertical && yLength <= 0))
                    break;
            }
            // si es square se debe de reiniciar la xLength para que se pueda seguir iterando sobre las filas de abajo del cuadrado, y de igual forma se decrementa la yLength para ir manteniendo rastro de las posiciones pendientes a cambiar su estado
            

                // Terminar los loops si ya no hay más que buscar
            if ((isHorizontal && xLength <= 0) || (isVertical && yLength <= 0))
                break;
        }

        return numFieldsCovered;
    }



    // Función para validar si se puede colocar barcos en cuadrícula basándose en que en caso de ser un ship no esté chocando con otro, que no se pueda poner en un lugar en donde ya se lanzó un misil, y que no se pueda poner en un lugar en donde ya se puso un barco
    public bool ValidateShipPlacing(CardDetails cardDetails, Transform startingQuad)
    {
        // checar si la carta es de ataque o de defensa, si es de ataque no es necesario validar si se puede poner en la cuadrícula
        bool isShip = cardDetails.CardType == "Defense";
        if (!isShip)
            return false;

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
        bool is1x1 = !isHorizontal && !isVertical;

        // Activación de la búsqueda una vez encontrada la posición inicial.
        bool isActivatedSearch = false;
        
        // Iterar sobre cada fila del grid.
        foreach (Transform row in transform)
        {
            // se itera de forma descendente sobre cada quad de cada row porque tenemos definido el anchor point en la esquina superior derecha del grid
            for (int i = row.childCount - 1; i >= 0; i--) // Itera de atrás hacia adelante
            {
                // se separan las coordenadas de cada quad iterado igual que con el quad inical
                Transform loopedQuad = row.GetChild(i);
                Quad quadScript = loopedQuad.GetComponent<Quad>();
                string[] coordinates = loopedQuad.name.Split(',');
                int x = int.Parse(coordinates[0]);
                int y = int.Parse(coordinates[1]);

                // Si llegamos al quad inicial, se coloca la primera posición de la carta y se comienza a buscar en base a la coordenada extraída del quad inicial
                if (x == xToFind && y == yToFind)
                {
                    // se activa la flag de búsqueda
                    isActivatedSearch = true;

                    // se valida que no se esté poniendo en un lugar donde ya se lanzó un misil o ya se puso un barco
                    if (quadScript.state == Quad.quadState.miss || quadScript.state == Quad.quadState.ship)
                        return false;

                    // dependiendo de las características del barco se ajustan las longitudes pendientes a buscar
                    if (isHorizontal)
                        xLength--;
                    else if (isVertical)
                        yLength--;
                    else if (is1x1)
                        return true;
                }

                // Continuar colocando el barco o misil mientras la búsqueda esté activada.
                else if (isActivatedSearch)
                {
                    if (isHorizontal && xLength > 0 && y == yToFind)
                    {
                        if (quadScript.state == Quad.quadState.miss || quadScript.state == Quad.quadState.ship)
                            return false;
                        xLength--;
                    }
                    else if (isVertical && yLength > 0 && x == xToFind)
                    {
                        if (quadScript.state == Quad.quadState.miss || quadScript.state == Quad.quadState.ship)
                            return false;
                        yLength--;
                    }
                }

                // Terminar los loops si ya no hay más que buscar
                if ((isHorizontal && xLength <= 0) || (isVertical && yLength <= 0))
                    return true;
            }

                // Terminar los loops si ya no hay más que buscar
            if ((isHorizontal && xLength <= 0) || (isVertical  && yLength <= 0))
                return true;
        }
        if ((isHorizontal && xLength <= 0) || (isVertical && yLength <= 0))
            return true;
        return false; 
    }

    public List<Transform> getQuadsList(int LengthX, int LengthY, Transform startingQuad){
        List<Transform> quadList = new List<Transform>();
        int currentX = int.Parse(startingQuad.name.Split(',')[0]);
        int currentY = int.Parse(startingQuad.name.Split(',')[1]);

        // El misil es horizontal
        if(LengthX > 1){
            for(int i = currentX - LengthX; i < currentX; i++){
                if(i >= 0 && i < startingQuad.parent.childCount){
                    Transform loopedQuad = startingQuad.parent.GetChild(i);
                    if(loopedQuad != null){
                        quadList.Add(loopedQuad);
                    }
                }
            }
        // El misil es vertical
        }else{
            for(int i = currentY; i < currentY + LengthY; i++){
                if(i-1 >= 0 && i-1 < startingQuad.parent.parent.childCount && currentX-1 >= 0 && currentX-1 < startingQuad.parent.parent.GetChild(i-1).childCount){
                    Transform loopedQuad = startingQuad.parent.parent.GetChild(i-1).GetChild(currentX-1);
                    if(loopedQuad != null){
                        quadList.Add(loopedQuad);
                    }
                }
            }
        }
        return quadList;
    }

    

}

