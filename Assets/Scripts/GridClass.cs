using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine;

public class Grid<TGridObject>
{
	// Changed Event Class Handler
	public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
	public class OnGridObjectChangedEventArgs : EventArgs {
		public int x;
		public int y;
	}


	// Global Grid Parameters
	private int width;
	private int height;
	private TGridObject[,] gridArray;

	// Grid Debug Parameters
	private bool debugMode;
	private float cellSize;
	private Vector3 origin;
	private TextMesh[,] debugTextArray;

	public Grid(int width, int height, float cellSize, Vector3 origin, bool debugMode, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
	{
		this.width = width;
		this.height = height;
		this.cellSize = cellSize;
		this.origin = origin;
		this.debugMode = debugMode;

		gridArray = new TGridObject[width, height];

		for (int x = 0; x < gridArray.GetLength(0); x++)
        {
			for (int y = 0; y < gridArray.GetLength(1); y++)
            {
				gridArray[x, y] = createGridObject(this, x, y);
            }
        }

		// Debug Visuals
		if (debugMode == true)
		{
			TextMesh[,] debugTextArray = new TextMesh[width, height];

			// Iterating through the 2-D Grid Array
			for (int x = 0; x < gridArray.GetLength(0); x++)
			{
				for (int y = 0; y < gridArray.GetLength(1); y++)
				{
					debugTextArray[x, y] = CreateWorldText(null, gridArray[x, y]?.ToString(), GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0),
														   14, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center, 5);

					// Drawing Dividers between the Grid Cells
					Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
					Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
				}
			}

			// Closing the boxes
			Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
			Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

			OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
			{
				debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
			};
		}

	}
	/////////////////////////////////////////////////////
	/////////////////////////////////////////////////////
	// Set Cell Value
	public void SetGridObject(int x, int y, TGridObject value)
	{
		if (x >= 0 && y >= 0 && x < width && y < height)
		{
			gridArray[x, y] = value;
			if (debugMode == true)
			{
				debugTextArray[x, y].text = gridArray[x, y].ToString();
				if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
			}
		}
	}

	public void SetGridObject(Vector3 worldPosition, TGridObject value)
	{
		int x, y;
		GetXY(worldPosition, out x, out y);

		SetGridObject(x, y, value);
	}

	// 14:16
	public void TriggerGridObjectChanged(int x, int y)
    {
		if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }
	/////////////////////////////////////////////////////
	/////////////////////////////////////////////////////
	// Get Cell Value
	public TGridObject GetGridObject(int x, int y)
	{
		if (x >= 0 && y >= 0 && x < width && y < height)
		{
			return gridArray[x, y];
		}
		else
		{
			return default(TGridObject);
		}
	}

	public TGridObject GetGridObject(Vector3 worldPosition)
	{
		int x, y;
		GetXY(worldPosition, out x, out y);
		return GetGridObject(x, y);
	}
	/////////////////////////////////////////////////////
	/////////////////////////////////////////////////////
	// Converts a Grid Cordinate to a World Cordinate
	private Vector3 GetWorldPosition(int x, int y)
	{
		return new Vector3(x, y) * cellSize + origin;
	}
	/////////////////////////////////////////////////////
	/////////////////////////////////////////////////////
	// Converts a World Cordinate to a Grid Cordinate
	private void GetXY(Vector3 worldPosition, out int x, out int y)
	{
		x = Mathf.FloorToInt((worldPosition - origin).x / cellSize);
		y = Mathf.FloorToInt((worldPosition - origin).y / cellSize);
	}
	/////////////////////////////////////////////////////
	/////////////////////////////////////////////////////
	// Draws text
	public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
	{
		GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));

		Transform transform = gameObject.transform;
		transform.SetParent(parent, false);
		transform.localPosition = localPosition;

		TextMesh textMesh = gameObject.GetComponent<TextMesh>();
		textMesh.anchor = textAnchor;
		textMesh.alignment = textAlignment;
		textMesh.text = text;
		textMesh.fontSize = fontSize;
		textMesh.color = color;
		textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

		return textMesh;
	}
	
}
