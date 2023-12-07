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
    private int chosenOption = -1;
    public int ChosenOption 
    { 
        get => this.chosenOption; 
        set => this.chosenOption = value; 
    }

    private List<int> currentOptions;
    public List<int> CurrentOptions { get => currentOptions; set => currentOptions = value; }

    private List<int> deletedOptions;

    public int Height { get; set; }

    public Cell(Tuple<int, int> index, int tileOptions)
    {
        this.Index = index;
        this.IsCollapsed = false;
        this.Height = 0;
        this.chosenOption = -1;
        this.deletedOptions = new();
        this.currentOptions = new();
        for (int i = 0; i < tileOptions; i++)
        {
            this.currentOptions.Add(i);

        }
    }

    public bool Collapse()
    {
        int count = CurrentOptions.Count;
        if (count <= 0) return false;

        // Succeed to collapse this cell
        this.IsCollapsed = true;
        this.ChosenOption = CurrentOptions[UnityEngine.Random.Range(0, count)]; 

        return true;
    }

    public void Uncollapse()
    {
        this.IsCollapsed = false;
        this.CurrentOptions.Remove(this.chosenOption);
        this.deletedOptions.Add(this.chosenOption);
        this.chosenOption = -1;
    }

    public void RestoreOptions()
    {
        this.IsCollapsed = false;
        this.chosenOption = -1;
        int count = this.deletedOptions.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            this.CurrentOptions.Add(deletedOptions[i]);
            this.deletedOptions.RemoveAt(i);
        }
    }
}
