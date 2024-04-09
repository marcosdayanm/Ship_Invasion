using System;

[Serializable]
public class Play
{
    public string PlayNumber;
    public string IsPlayerPlay;
    public string NumFieldsCovered;
    public string GameId;
    public string CardPlayedId;

    public Play(string playNumber, string isPlayerPlay, string numFieldsCovered, string gameId, string cardPlayedId)
    {
        PlayNumber = playNumber;
        IsPlayerPlay = isPlayerPlay;
        NumFieldsCovered = numFieldsCovered;
        GameId = gameId;
        CardPlayedId = cardPlayedId;
    }

}
