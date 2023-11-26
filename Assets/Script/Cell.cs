using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each cell in the defined grid in LayoutGeneration
/// </summary>
public class Cell : MonoBehaviour
{
    private int index;          // this cell index in the Grid
    public int Index
    {
        get { return index; }
        set { index = value; }
    }

    private bool isCollapsed;   // whether this cell is collaped
    public bool IsCollapsed
    {
        get { return isCollapsed; }
        set { isCollapsed = value; }
    }

    private List<int> options;  // availabe options
    public List<int> Options
    {
        get { return options; }
        set { options = value; }
    }

    public Cell()
    {
        isCollapsed = false;
        options = new();
    }

    public void Setup(int index, int tileOptions)
    {
        this.index = index;
        isCollapsed = false;
        for (int i = 0; i < tileOptions; i++)
        {
            options.Add(i);
        }
    }

    public override string ToString()
    {
        string options = "[" + string.Join(", ", this.options) + "]";
        return $"{{\n" +
            $"\tindex: {index}\n" +
            $"\tisCollapsed: {isCollapsed}\n" +
            $"\toptions: {options}\n" +
            $"}}";
    }
}
