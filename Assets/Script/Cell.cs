using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each cell in the defined grid in LayoutGeneration
/// </summary>
public class Cell : MonoBehaviour
{
    public int Index { get; set; }
    public bool IsCollapsed { get; set; }
    public List<int> Options { get; set; }

    public Cell()
    {
        this.IsCollapsed = false;
        this.Options = new();
    }

    public void Setup(int index, int tileOptions)
    {
        this.Index = index;
        this.IsCollapsed = false;
        for (int i = 0; i < tileOptions; i++)
        {
            Options.Add(i);
        }
    }

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
