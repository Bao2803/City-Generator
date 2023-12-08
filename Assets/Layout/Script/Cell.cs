using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each cell in the defined grid in LayoutGeneration
/// </summary>
public class Cell
{
    public Tuple<int, int> Index { get; set; }
    public bool IsCollapsed { get; set; }
    public List<int> Options { get; set; }
    public int Height { get; set; }
    /*private int chosenOption = -1;
    public int ChosenOption
    {
        get => this.chosenOption;
        set => this.chosenOption = value;
    }*/

    public Cell(Tuple<int, int> index, int tileOptions)
    {
        this.Index = index;
        this.IsCollapsed = false;
        this.Height = 0;
        /*this.chosenOption = -1;*/
        this.Options = new List<int>();
        for (int i = 0; i < tileOptions; i++)
        {
            this.Options.Add(i);
        }
    }

    /*public void Setup()
    {
        this.Index = index;
        this.IsCollapsed = false;
        for (int i = 0; i < tileOptions; i++)
        {
            Options.Add(i);
        }
    }*/

    public override string ToString()
    {
        string options = "[" + string.Join(", ", this.Options) + "]";
        return $"{{\n" +
            $"\tindex: {Index}\n" +
            $"\tisCollapsed: {IsCollapsed}\n" +
            $"\toptions: {options}\n" +
            $"}}";
    }
}
