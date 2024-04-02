using System;

[Serializable]
public class Arena
{
    public int Id;
    public string Name;
    public int Level;
    public int MatchesRequired;
    public int? MusicIdUnity; // Nullable
    public int SpriteId;
}
