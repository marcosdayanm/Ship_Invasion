using System;

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
    public Quality Quality { get; set; }
    public CardType CardType { get; set; }
    public Area Area { get; set; }
    public Sprite Skin { get; set; }
    public Sprite Effect { get; set; }
    public List<Play> Plays { get; set; }
}
