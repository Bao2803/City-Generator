using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDemo : MonoBehaviour
{
    public BuildingSettings settings;
    public Vector3 locaiton;
    // Start is called before the first frame update
    void Start()
    {
        Building b = BuildingGenerator.Generate(settings, locaiton);
        GetComponent<BuildingRenderer>().Render(b);
        Debug.Log(b.ToString());
    }

}
