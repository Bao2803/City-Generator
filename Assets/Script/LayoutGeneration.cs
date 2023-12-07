using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Generate the layout for the City
/// </summary>
public class LayoutGeneration : MonoBehaviour
{
	public int dimension = 2;   // the dimension (dimension x dimension) of the generated city
	public Tile[] tileObjects;  // the pre-defined tiles prefab
	
	private int iteration = 0;
	private int errors = 0;
	private List<Cell> grid;    // the internal grid used for WFC
	private Stack<List<int>>[] history;
    private int prob = 20;
    private int maxHeight = 10;
	public int minHeight;

    private void Awake()
	{
        // Generate adjacency rules for tiles
        for (int i = 0; i < this.tileObjects.Length; i++)
        {
            this.tileObjects[i].CreateRule(this.tileObjects);
        }
    }

	private void Start()
	{
        // Initialize the Grid and fill the cells with all available options
        this.grid = new List<Cell>(dimension * dimension);
        this.history = new Stack<List<int>>[dimension * dimension];
        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                // Graph start at position 0,0,0. +Z is to the right and +X is to the bottom.
                Cell newCell = new(Tuple.Create(row, col), this.tileObjects.Length);
                this.grid.Add(newCell);
                this.history[row * dimension + col] = new Stack<List<int>>();
            }
        }

        List<Cell> sortedGrid = new(grid.Count);                     // act like a Priority Queue    
		for (int i = 0; i < grid.Count; i++)
		{
			sortedGrid.Add(grid[i]);
		}

		// Set 50% cells to be buildings
		int totalBuilding = prob * dimension * dimension / 100;
		Debug.Log(totalBuilding);
		for (int i = 0; i < totalBuilding; i++)
		{
			Cell buildingCell = sortedGrid[UnityEngine.Random.Range(0, sortedGrid.Count)];
			sortedGrid.Remove(buildingCell);
			buildingCell.Height = UnityEngine.Random.Range(1, maxHeight);
			buildingCell.ChosenOption = 0;
			buildingCell.IsCollapsed = true;
			Propagate(buildingCell);
		}

		bool resturnVal = WaveFunctionCollapse(sortedGrid);
		resturnVal = resturnVal && (sortedGrid.Count == 0);

        Debug.Log("WFC: " + resturnVal);
		Debug.Log("Iteration: " + iteration);
        Debug.Log("Undo: " + errors);
        Debug.Log("Count: " + sortedGrid.Count);

		if (resturnVal)
		{
			foreach (Cell cell in grid)
			{
				Debug.Assert(cell.ChosenOption != -1);
				Instantiate(
                    this.tileObjects[cell.ChosenOption],
                    new Vector3(cell.Index.Item1, 0, cell.Index.Item2),
					this.tileObjects[cell.ChosenOption].transform.rotation
                    );
			}
		}
	}

    /// <summary>
    /// Perform Wave Function Collapse Algorithm on the given grid
    /// </summary>
    /// <param name="sortedGrid">a "Priority Queue" to select which tile to collapse</param>
    /// <returns></returns>
    private bool WaveFunctionCollapse(List<Cell> sortedGrid)
	{
		iteration++;
		if (sortedGrid.Count == 0) return true;

		Cell cellToCollapse = PickCell(sortedGrid);
		while (cellToCollapse.Collapse())
		{
			Propagate(cellToCollapse);
			if (WaveFunctionCollapse(sortedGrid))
			{
				/*if (cellToCollapse.ChosenOption == 0)
				{
					cellToCollapse.Height = UnityEngine.Random.Range(1, maxHeight);
				}*/
				return true;
			}
			cellToCollapse.Uncollapse();
			RevertPropogate(cellToCollapse);
		}

		cellToCollapse.RestoreOptions();
		sortedGrid.Add(cellToCollapse);
		iteration--;

		return false;
	}

	/// <summary>
	/// Select the cell with the lowest entropy (random if more than 1) that is not collapsed in the grid
	/// </summary>
	/// <param name="sortedGrid"> The sortedGrid in Start</param>
	/// <returns>A cell with the lowest entropy in the current grid</returns>
	private Cell PickCell(List<Cell> sortedGrid)
	{
		//sortedGrid.Sort((a, b) => { return a.CurrentOptions.Count - b.CurrentOptions.Count; });

		// Find all least entropy cells
		/*int stopIndex = 0;
		int minIndex = 0;
		while (stopIndex < sortedGrid.Count)
		{
			if (sortedGrid[stopIndex].CurrentOptions.Count < sortedGrid[minIndex].CurrentOptions.Count)
			{
				minIndex = stopIndex;
			}
			stopIndex++;
		}*/

		// Pick a cell from the least entropy set
		//int collapseIndex = UnityEngine.Random.Range(0, stopIndex);
		/*Cell pickedCell = sortedGrid[collapseIndex];
		sortedGrid.RemoveAt(collapseIndex);*/
		//sortedGrid.RemoveAt(minIndex);

		Debug.Assert(sortedGrid.Count > 0);
		
		return sortedGrid[0];
	}

    /// <summary>
	/// Propagate the 4 neighbors cell of the collapsedCell with new constrants
	/// </summary>
	/// <param name="collapsedCell">The cell the is collapsed currently</param>
    private void Propagate(Cell collapsedCell)
	{
		int row = collapsedCell.Index.Item1;
		int col = collapsedCell.Index.Item2;
        Tile chosenTile = this.tileObjects[collapsedCell.ChosenOption];
		
		// Update top
		if (row > 0)
		{
			ValidateCell(
                this.grid[(row - 1) * dimension + col], 
				chosenTile.Top
				);
		}

		// Update right
		if (col < dimension - 1)
		{
			ValidateCell(
				this.grid[row * dimension + (col + 1)],
				chosenTile.Right
				);
		}

		// Update bottom
		if (row < dimension - 1)
		{
            ValidateCell(
                this.grid[(row + 1) * dimension + col],
                chosenTile.Bottom
                );
        }

		// Update left
		if (col > 0)
		{
			ValidateCell(
				this.grid[row * dimension + (col - 1)],
				chosenTile.Left
				);
		}
	}

    /// <summary>
    /// Remove any option in targetCell.currentOptions that does not exist in the validOption
    /// Update history with the deleted options
    /// </summary>
    /// <param name="targetCell">The cell to be validated</param>
    /// <param name="validOptions">The opitons that the targetCell allowed to have</param>
    private void ValidateCell(Cell targetCell, List<int> validOptions)
    {
        if (!targetCell.IsCollapsed)
        {
            List<int> options = targetCell.CurrentOptions;
            List<int> editHistory = new();
            for (int x = options.Count - 1; x >= 0; x--)
            {
                int element = options[x];
                if (!validOptions.Contains(element))
                {
                    editHistory.Add(element);
                    targetCell.CurrentOptions.RemoveAt(x);
                }
            }

            // Update history
            int row = targetCell.Index.Item1;
            int col = targetCell.Index.Item2;
            this.history[row * dimension + col].Push(editHistory);
        }
    }

    private void RevertPropogate(Cell collapsedCell)
    {
        int row = collapsedCell.Index.Item1;
        int col = collapsedCell.Index.Item2;

        // Update top
        if (row > 0)
        {
			RevertCell(row - 1, col);
        }

        // Update right
        if (col < dimension - 1)
        {
            RevertCell(row, col + 1);
        }

        // Update bottom
        if (row < dimension - 1)
        {
            RevertCell(row + 1, col);
        }

        // Update left
        if (col > 0)
        {
            RevertCell(row, col -1);
        }
    }

	private void RevertCell(int row, int col)
	{
        int index = row * dimension + col;
        if (!this.grid[index].IsCollapsed)
		{
			List<int> lastEdit = this.history[index].Pop();

			while (lastEdit.Count > 0)
			{
				this.grid[index].CurrentOptions.Add(lastEdit[lastEdit.Count - 1]);
				lastEdit.RemoveAt(lastEdit.Count - 1);
			}
		}
	}
}
