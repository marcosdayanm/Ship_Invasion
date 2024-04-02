using System;

[Serializable]
public class Play
{
    public int Id;
    public int PlayNumber;
    public bool IsPlayerPlay;
    public bool IsAttackCardPlayed;
    public int NumFieldsCovered;
    public int GameId;
    public int CardPlayedId;
}
