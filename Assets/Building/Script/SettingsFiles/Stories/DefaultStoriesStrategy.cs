using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultStoriesStrategy : StoriesStrategy
{
    public override Story[] GenerateStories(BuildingSettings settings, RectInt bounds, int numberOfStories)
    {
        return new Story[]
        {
            settings.storyStrategy != null ?
                settings.storyStrategy.GenerateStory(settings, bounds, 10) :
                ((StoryStrategy) ScriptableObject.CreateInstance<DefaultStoryStrategy>()).GenerateStory(settings, bounds, 10)
        };
    }
}
