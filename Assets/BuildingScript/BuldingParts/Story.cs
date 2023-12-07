using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story
{
    int level;
    Wall[] walls;

    public int Level { get => level; }
    public Wall[] Walls { get => walls; set => walls = value; }

    public Story(int level, Wall[] walls)
    {
        this.level = level;
        this.Walls = walls;
    }

    public override string ToString()
    {
        string story = "Story level: " + level + "\n";
        story += "\t\tWalls: ";
        foreach (Wall w in Walls)
        {
            story += w.ToString() + ", ";
        }
        return story;
    }
}
