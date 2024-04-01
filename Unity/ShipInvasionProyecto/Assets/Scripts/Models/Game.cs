using System;
using System.Collections.Generic;

[Serializable]
public class Game
{
    public int Id;
    public DateTime Date;
    public bool IsPlayerWon;
    public int PlayerId;
    public int ArenaId;

    // Propiedades de navegación
    public Player Player { get; set; }
    public Arena Arena { get; set; }
    public List<Play> Plays { get; set; }
}
