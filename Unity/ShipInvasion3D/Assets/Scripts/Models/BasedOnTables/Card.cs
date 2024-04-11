using System;
using System.Collections.Generic;

[Serializable]
public class Card
{
    public int Id;
    public string Name;
    public int QualityId;
    public int CardTypeId;
    public int AreaId;
    public int? SkinId; 
    public int? EffectId; 

    // Propiedades de navegación
    public Quality Quality;
    public CardType CardType;
    public Area Area;
    // public Sprite Skin;
    // public Sprite Effect;
    public List<Play> Plays;
}
