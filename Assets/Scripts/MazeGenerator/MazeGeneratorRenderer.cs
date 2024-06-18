using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorRenderer : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] GameObject mazeCellPrefab;

    public float cellSize = 1f;

    // Start is called before the first frame update
    private void Start()
    {
        MazeCell[,] maze = mazeGenerator.GetMaze();

        for (int x = 0; x < mazeGenerator.mazeWidth; x++)
        {
            for (int y = 0; y < mazeGenerator.mazeHeight; y++)
            {
                Vector3 position = new Vector3(x * cellSize, 0f, y * cellSize);
                GameObject newCell = Instantiate(mazeCellPrefab, position, Quaternion.identity, transform);
                MazeCellObjects mazeCell = newCell.GetComponent<MazeCellObjects>();

                bool top = maze[x, y].topWall;
                bool left = maze[x, y].leftWall;

                // Right wall depends on the cell to the right
                bool right = (x == mazeGenerator.mazeWidth - 1) || maze[x + 1, y].leftWall;

                // Bottom wall depends on the cell below
                bool bottom = (y == 0) || maze[x, y - 1].topWall;

                mazeCell.Init(top, right, bottom, left);
            }
        }
    }
}