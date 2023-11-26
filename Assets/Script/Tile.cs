using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to be attached to the pre-made road prefabs. 
/// </summary>
public class Tile : MonoBehaviour
{
    // Index of tiles (of tileObjects in LayoutGeneration) that can be place ABOVE THIS tile
    private List<int> top;
    public List<int> Top
    {
        get { return top; }
        set { top = value; }
    }

    // Index of tiles (of tileObjects in LayoutGeneration) that can be place RIGHT THIS tile
    private List<int> right;
    public List<int> Right
    {
        get { return right; }
        set { right = value; }
    }

    // Index of tiles (of tileObjects in LayoutGeneration) that can be place BOTTOM THIS tile
    private List<int> bottom;
    public List<int> Bottom
    {
        get { return bottom; }
        set { bottom = value; }
    }

    // Index of tiles (of tileObjects in LayoutGeneration) that can be place LEFT THIS tile
    private List<int> left;
    public List<int> Left
    {
        get { return left; }
        set { left = value; }
    }

    public int[] edgeLabel = new int[4]; // [top, right, bottom, left]
    public void CreateRule(IEnumerable<Tile> prefabTiles)
    {
        // Replace old lists
        top = new();
        right = new();
        bottom = new();
        left = new();

        int index = 0;
        foreach (Tile tile in prefabTiles)
        {
            // top
            if (this.edgeLabel[0] == tile.edgeLabel[2])
            {
                this.top.Add(index);
            }

            // right
            if (this.edgeLabel[1] == tile.edgeLabel[3])
            {
                this.right.Add(index);
            }

            // bottom
            if (this.edgeLabel[2] == tile.edgeLabel[0])
            {
                this.bottom.Add(index);
            }

            // left
            if (this.edgeLabel[3] == tile.edgeLabel[1])
            {
                this.left.Add(index);
            }

            index++;
        }
    }

    public override string ToString()
    {
        string tops = "[" + string.Join(", ", this.top) + "]";
        string rights = "[" + string.Join(", ", this.right) + "]";
        string bottoms = "[" + string.Join(", ", this.bottom) + "]";
        string lefts = "[" + string.Join(", ", this.left) + "]";
        return $"Tile: {{\n" +
            $"\ttop: {tops}\n" +
            $"\tright: {rights}\n" +
            $"\tbottom: {bottoms}\n" +
            $"\tleft: {lefts}\n" +
            $"}}\n";
    }
}
