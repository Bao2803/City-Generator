using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultRoofStrategy : RoofStrategy
{
    public override Roof GenerateRoof(BuildingSettings settings, RectInt bounds)
    {
        return new Roof();
    }
}
