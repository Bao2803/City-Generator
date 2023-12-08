using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWallsStrategy : WallsStrategy
{
    public override Wall[] GenerateWalls(BuildingSettings settings, RectInt bounds, int level)
    {
        return new Wall[(bounds.size.x + bounds.size.y) * 2];
    }
}
