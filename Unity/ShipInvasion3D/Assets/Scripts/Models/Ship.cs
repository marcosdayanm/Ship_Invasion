using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable] public class Ship{
    public string Name;
    public int LengthX;
    public int LengthY;
    public List<Transform> quads;
    public bool sunken;
}