using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultStoryStrategy : StoryStrategy
{
    public override Story GenerateStory(BuildingSettings settings, RectInt bounds, int level)
    {
        return new Story(0, settings.wallsStrategy != null ?
                settings.wallsStrategy.GenerateWalls(settings, bounds, level) :
                ((WallsStrategy)ScriptableObject.CreateInstance<DefaultWallsStrategy>()).GenerateWalls(settings, bounds, level)
            );
    }
}
