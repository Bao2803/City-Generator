using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generate the layout for the City
/// </summary>
public class LayoutGeneration : MonoBehaviour
{
    public int dimension = 2;   // the dimension (dimension x dimension) of the generated city
    public Tile[] tileObjects;  // the pre-defined tiles prefab
    public Cell cellObj;        // the pre-defined cell prefab
    private List<Cell> grid;    // the internal grid used for WFC

    private int prob = 10;
    private int maxHeight = 10;
    public BuildingSettings settings;

    private void Awake()
    {
        // Generate adjacency rules for tiles
        for (int i = 0; i < tileObjects.Length; i++)
        {
            tileObjects[i].CreateRule(i, tileObjects);
        }

    }

    private void Start()
    {
        List<Cell> sortedGrid;
        bool returnVal = false;
        int totalBuilding = prob * dimension * dimension / 100;

        while (!returnVal)
        {
            // Initialize the Grid and fill the cells with all available options
            grid = new List<Cell>(dimension * dimension);
            for (int row = 0; row < dimension; row++)
            {

                for (int col = 0; col < dimension; col++)
                {
                    // Graph start at position 0,0,0. +Z is to the right and +X is to the bottom.
                    Cell newCell = new(Tuple.Create(row, col), this.tileObjects.Length);
                    grid.Add(newCell);
                }
            }

            sortedGrid = new(grid);  // keep track of the running of Wave Function Collapse

            // Place some buildings first to adapt the number of building
            
            for (int i = 0; i < totalBuilding; i++)
            {
                Cell buildingCell = sortedGrid[UnityEngine.Random.Range(0, sortedGrid.Count)];
                sortedGrid.Remove(buildingCell);
                buildingCell.Height = UnityEngine.Random.Range(1, maxHeight);
                buildingCell.Options = new List<int> { 0 };
                buildingCell.IsCollapsed = true;
            }
            Propagate();

            returnVal = true;

            while (returnVal && sortedGrid.Count > 0)
            {
                Cell cellToCollapse = PickCell(sortedGrid);
                returnVal = returnVal && Collapse(cellToCollapse);
                Propagate();
            }
        }

        foreach (Cell cell in grid)
        {
            if (cell.Options[0] != 0)
            {
                Tile curr = Instantiate(
                    this.tileObjects[cell.Options[0]],
                    new Vector3(cell.Index.Item1 * 3 + (float) 1.5, 0, cell.Index.Item2 * 3 + (float)1.5),
                    this.tileObjects[cell.Options[0]].transform.rotation
                    );
                curr.transform.localScale = new Vector3(3f, 3f, 3f);
            }
            else
            {
                Building b = BuildingGenerator.Generate(
                    settings,
                    new Vector3(cell.Index.Item1 * 3 + 3f, 0, cell.Index.Item2 * 3 + 3f)
                    );
                GetComponent<BuildingRenderer>().Render(b);
                Debug.Log(b.ToString());
            }
        }

    }

    /// <summary>
    /// Select the cell with the lowest entropy (random if more than 1) that is not collapsed in the grid
    /// </summary>
    /// <param name="sortedGrid"> The sortedGrid in Start</param>
    /// <returns>A cell with the lowest entropy in the current grid</returns>
    private Cell PickCell(List<Cell> sortedGrid)
    {
        sortedGrid.Sort((a, b) => { return a.Options.Count - b.Options.Count; }); 

        // Find all least entropy cells
        int stopIndex = 1;

        while (stopIndex < sortedGrid.Count &&
                sortedGrid[stopIndex].Options.Count == sortedGrid[0].Options.Count)
        {
            stopIndex++;
        }

        // Pick a cell from the least entropy set
        int collapseIndex = UnityEngine.Random.Range(0, stopIndex);
        Cell pickedCell = sortedGrid[collapseIndex];
        sortedGrid.RemoveAt(collapseIndex);
        
        return pickedCell;
    }

    /// <summary>
    /// Collapse a cell
    /// </summary>
    /// <param name="cellToCollapse"> A cell that is picked by PickCell</param>
    private bool Collapse(Cell cellToCollapse)
    {
        if (cellToCollapse.Options.Count <= 0) return false;

        // Collapse
        cellToCollapse.IsCollapsed = true;
        int pickedTileIndex = cellToCollapse.Options[UnityEngine.Random.Range(0, cellToCollapse.Options.Count)];
        cellToCollapse.Options = new List<int> { pickedTileIndex };

        return true;
    }

    /// <summary>
    /// Propogate new constraints that are the resulted of collapsing a cell to the whole grid
    /// </summary>
    private void Propagate()
    {
        for (int y = 0; y < dimension; y++)
        {
            for (int x = 0; x < dimension; x++)
            {
                int index = y * dimension + x;

                // Collapsed
                if (grid[index].IsCollapsed) { continue; }

                // Not collapsed
                List<int> options = new(tileObjects.Length);
                for (int i = 0; i < tileObjects.Length; i++)
                {
                    options.Add(i);
                }

                // Update top
                if (y > 0)
                {
                    Cell topCell = grid[(y - 1) * dimension + x];
                    List<int> validOptions = new();

                    foreach (int possibleOption in topCell.Options)
                    {
                        List<int> valid = tileObjects[possibleOption].Bottom;
                        validOptions = validOptions.Concat(valid).ToList();
                    }

                    ValidateOptions(options, validOptions);
                }

                // Update right
                if (x < dimension - 1)
                {
                    Cell rightCell = grid[y * dimension + (x + 1)];
                    List<int> validOptions = new();

                    foreach (int possibleOption in rightCell.Options)
                    {
                        List<int> valid = tileObjects[possibleOption].Left;
                        validOptions = validOptions.Concat(valid).ToList();
                    }

                    ValidateOptions(options, validOptions);
                }

                // Update bottom
                if (y < dimension - 1)
                {
                    Cell bottomCell = grid[(y + 1) * dimension + x];
                    List<int> validOptions = new();

                    foreach (int possibleOption in bottomCell.Options)
                    {
                        List<int> valid = tileObjects[possibleOption].Top;
                        validOptions = validOptions.Concat(valid).ToList();
                    }

                    ValidateOptions(options, validOptions);
                }

                // Update left
                if (x > 0)
                {
                    Cell leftCell = grid[y * dimension + (x - 1)];
                    List<int> validOptions = new();

                    foreach (int possibleOption in leftCell.Options)
                    {
                        List<int> valid = tileObjects[possibleOption].Right;
                        validOptions = validOptions.Concat(valid).ToList();
                    }

                    ValidateOptions(options, validOptions);
                }

                grid[index].Options = options;
            }
        }
    }

    /// <summary>
    /// Remove any option in optionList that is not exist in validOption
    /// </summary>
    /// <param name="options">Options to be validate</param>
    /// <param name="validOptions">Rule to be validate against</param>
    void ValidateOptions(List<int> options, List<int> validOptions)
    {
        for (int x = options.Count - 1; x >= 0; x--)
        {
            int element = options[x];
            if (!validOptions.Contains(element))
            {
                options.RemoveAt(x);
            }
        }
    }
}
