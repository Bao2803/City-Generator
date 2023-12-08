using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator
{
    public static Building Generate(BuildingSettings settings, Vector3 location)
    {
        return new Building(
            settings.Size.x, 
            settings.Size.y, 
            settings.wingsStrategy != null ? 
                settings.wingsStrategy.GenerateWings(settings) :
                ((WingsStrategy) ScriptableObject.CreateInstance<DefaultWingsStrategy>()).GenerateWings(settings),
            location
            );
    }
}
