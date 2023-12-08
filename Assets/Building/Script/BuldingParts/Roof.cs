using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roof
{
    RoofType type;
    RoofDirection direction;

    public RoofType Type { get => type; }
    public RoofDirection Direction { get => direction; }

    public Roof(RoofType type = RoofType.Peak, RoofDirection direction = RoofDirection.North)
    {
        this.type = type;
        this.direction = direction;
    }

    public override string ToString()
    {
        string dirString = (type == RoofType.Peak || type == RoofType.Slope) ?
                                ", " + direction.ToString() : "";
        return "Roof: " + type.ToString() + dirString;
    }
}

public enum RoofType
{
    Point,
    Peak,
    Slope,
    Flat
}

public enum RoofDirection
{
    North,      //positive y
    East,       //positive x
    South,      //negative y
    West        //negative x
}
