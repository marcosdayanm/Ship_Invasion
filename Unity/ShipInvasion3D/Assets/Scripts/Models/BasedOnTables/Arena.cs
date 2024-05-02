using System;
using System.Collections.Generic;

[Serializable]
public class Arena
{
    public int Id;
    public string Name;
    public int Level;

    public int Cost;
    public int MatchesRequired;
    public int? MusicIdUnity; // Nullable
    public int? SpriteId;
}

public class ArenasList
{
    public List<Arena> arenas;
}
