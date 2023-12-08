using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to be attached to the pre-made road prefabs. 
/// </summary>
public class Tile : MonoBehaviour
{
    public List<int> Top { get; set; }
    public List<int> Right { get; set; }
    public List<int> Bottom { get; set; }
    public List<int> Left { get; set; }

    public int[] edgeLabel = new int[4]; // [top, right, bottom, left]
    public void CreateRule(int thisIndex, Tile[] prefabTiles)
    {
        // Replace old lists
        this.Top = new();
        this.Right = new();
        this.Bottom = new();
        this.Left = new();

        // Rules
        int thisTotal = 0;                                                                      // total edges with road for this tile
        foreach (int i in this.edgeLabel)
        {
            thisTotal += i;
        }
        int thisIntersect = thisTotal >= 3 ? 1 : 0;                                             // whether this tile is an intersection
        int thisTurn = (thisTotal == 2 && (this.edgeLabel[0] != this.edgeLabel[2])) ? 1 : 0;    // whether this tile is a turn

        for (int index = 0; index < prefabTiles.Length; index++)
        {
            Tile tile = prefabTiles[index];

            int total = 0;                                                                      // total edges with road for current tile
            foreach (int i in tile.edgeLabel)
            { 
                total += i;
            }
            int intersect = total >= 3 ? 1 : 0;                                                 // whether current tile is an intersection
            int turn = (total == 2 && (tile.edgeLabel[0] != tile.edgeLabel[2])) ? 1 : 0;        // whether current tile is a turn

            bool ok = thisIntersect + intersect + thisTurn + turn < 2;

            // top
            if (this.edgeLabel[0] == tile.edgeLabel[2] && ok)
            {
                this.Top.Add(index);
            }

            // right
            if (this.edgeLabel[1] == tile.edgeLabel[3] && ok)
            {
                this.Right.Add(index);
            }

            // bottom
            if (this.edgeLabel[2] == tile.edgeLabel[0] && ok)
            {
                this.Bottom.Add(index);
            }

            // left
            if (this.edgeLabel[3] == tile.edgeLabel[1] && ok)
            {
                this.Left.Add(index);
            }
        }
    }

    public override string ToString()
    {
        string tops = "[" + string.Join(", ", this.Top) + "]";
        string rights = "[" + string.Join(", ", this.Right) + "]";
        string bottoms = "[" + string.Join(", ", this.Bottom) + "]";
        string lefts = "[" + string.Join(", ", this.Left) + "]";
        return $"Tile: {{\n" +
            $"\ttop: {tops}\n" +
            $"\tright: {rights}\n" +
            $"\tbottom: {bottoms}\n" +
            $"\tleft: {lefts}\n" +
            $"}}\n";
    }
}
