using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
	private Tile[] spawnTile;
	private int iteration = 0;
	private int errors = 0;

	private void Awake()
	{
		// Generate adjacency rules for tiles
		for (int i = 0; i < this.tileObjects.Length; i++)
		{
			this.tileObjects[i].CreateRule(i, this.tileObjects);
		}

		// Initialize the Grid and fill the cells with all available options
		this.grid = new List<Cell>(dimension * dimension);
		this.spawnTile = new Tile[dimension * dimension];
		for (int y = 0; y < dimension; y++)
		{
			for (int x = 0; x < dimension; x++)
			{
				// Graph start at position 0,0,0. +Z is to the right and +X is to the bottom.
				Cell newCell = Instantiate(cellObj, new Vector3(y, 0, x), Quaternion.identity);
				newCell.Setup(y * dimension + x, this.tileObjects.Length);
				grid.Add(newCell);
			}
		}
	}

	private void Start()
	{
		iteration = 0;
		errors = 0;
		List<Cell> sortedGrid = new(grid);                     // act like a Priority Queue    
        Debug.Log("WFC: " + WaveFunctionCollapse(sortedGrid));
		Debug.Log("Iteration: " + iteration);
        Debug.Log("Undo: " + errors);
        Debug.Log("Count: " + sortedGrid.Count);
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
		List<int> previousOptions = new(cellToCollapse.Options);

		while (Collapse(cellToCollapse))
		{
			Propagate();
			if (WaveFunctionCollapse(sortedGrid)) return true;
			Uncollapse(cellToCollapse, previousOptions);
		}

		return false;
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
		int stopIndex = 0;
		for (int i = 1; i < sortedGrid.Count; i++)
		{
			if (sortedGrid[i].Options.Count > sortedGrid[0].Options.Count)
			{
				stopIndex = i;
				break;
			}
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
		int pickedTileIndex;
		try
		{
			pickedTileIndex = cellToCollapse.Options[UnityEngine.Random.Range(0, cellToCollapse.Options.Count)];  
		}

		// Failed to collapse this cell
		catch (ArgumentOutOfRangeException)
		{
			return false;
		}

		// Succeed to collapse this cell
		cellToCollapse.IsCollapsed = true;
		cellToCollapse.Options = new List<int> { pickedTileIndex };
		//Debug.Log("Index: " + cellToCollapse.Index);				// TODO: Delete after debug
		this.spawnTile[cellToCollapse.Index] = Instantiate(
			this.tileObjects[pickedTileIndex], 
			cellToCollapse.transform.position, 
			this.tileObjects[pickedTileIndex].transform.rotation
			);
		return true;
	}

    /// <summary>
    /// Uncollapse a cell (revert of Collapse())
    /// </summary>
    /// <param name="collapsedCell"> A cell that is already collapsed</param>
    private void Uncollapse(Cell collapsedCell, List<int> previousOptions)
    {
		errors++;
		Debug.Assert(collapsedCell.Options.Count == 1);				// TODO: Delete after debug
		previousOptions.Remove(collapsedCell.Options[0]);
		collapsedCell.Options = previousOptions;
		collapsedCell.IsCollapsed = false;
		Destroy(spawnTile[collapsedCell.Index]);					// unspawn tile
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
				if (this.grid[index].IsCollapsed) { continue; }

				// Not collapsed
				List<int> options = new(this.tileObjects.Length);
				for (int i = 0; i < this.tileObjects.Length; i++)
				{
					options.Add(i);
				}

				// Update top
				if (y > 0)
				{
					Cell topCell = this.grid[(y - 1) * dimension + x];
					List<int> validOptions = new();

					foreach (int possibleOption in topCell.Options)
					{
						List<int> valid = this.tileObjects[possibleOption].Bottom;
						validOptions = validOptions.Concat(valid).ToList();
					}

					ValidateOptions(options, validOptions);
				}

				// Update right
				if (x < dimension - 1)
				{
					Cell rightCell = this.grid[y * dimension + (x + 1)];
					List<int> validOptions = new();

					foreach (int possibleOption in rightCell.Options)
					{
						List<int> valid = this.tileObjects[possibleOption].Left;
						validOptions = validOptions.Concat(valid).ToList();
					}

					ValidateOptions(options, validOptions);
				}

				// Update bottom
				if (y < dimension - 1)
				{
					Cell bottomCell = this.grid[(y + 1) * dimension + x];
					List<int> validOptions = new();

					foreach (int possibleOption in bottomCell.Options)
					{
						List<int> valid = this.tileObjects[possibleOption].Top;
						validOptions = validOptions.Concat(valid).ToList();
					}

					ValidateOptions(options, validOptions);
				}

				// Update left
				if (x > 0)
				{
					Cell leftCell = this.grid[y * dimension + (x - 1)];
					List<int> validOptions = new();

					foreach (int possibleOption in leftCell.Options)
					{
						List<int> valid = this.tileObjects[possibleOption].Right;
						validOptions = validOptions.Concat(valid).ToList();
					}

					ValidateOptions(options, validOptions);
				}

				this.grid[index].Options = options;
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
