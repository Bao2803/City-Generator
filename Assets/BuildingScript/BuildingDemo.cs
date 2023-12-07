using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDemo : MonoBehaviour
{
    public BuildingSettings settings;
    // Start is called before the first frame update
    void Start()
    {
        Building b = BuildingGenerator.Generate(settings);
        GetComponent<BuildingRenderer>().Render(b);
        Debug.Log(b.ToString());
    }

}
