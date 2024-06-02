using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour
{
    [Range(5, 100)]
    public int mazeWidth = 10, mazeHeight = 10;  // the dimensions of the maze
    public int startX, startY;  // the starting position

    public GameObject wallPrefab;
    public GameObject spider;
    public GameObject demon;
    public GameObject PortalPrefab;
    public int numberOfSpiders = 10;
    public int numberOfDemons = 10;

    MazeCell[,] maze;
    Vector2Int currentCell;

    public MazeCell[,] GetMaze()
    {
        maze = new MazeCell[mazeWidth, mazeHeight];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = new MazeCell(x, y);
            }
        }

        CarvePath(startX, startY);
        PlaceExit();
        PlaceEnemies();

        return maze;
    }

    bool IsCellValid(int x, int y)
    {
        // Check if the coordinates are within the bounds of the maze and if the cell has not been visited.
        return x >= 0 && x < mazeWidth && y >= 0 && y < mazeHeight && !maze[x, y].visited;
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] possibleNeighbors = {
            new Vector2Int(cell.x, cell.y + 1), // North
            new Vector2Int(cell.x + 1, cell.y), // East
            new Vector2Int(cell.x, cell.y - 1), // South
            new Vector2Int(cell.x - 1, cell.y)  // West
        };

        foreach (var n in possibleNeighbors)
        {
            if (n.x >= 0 && n.x < mazeWidth && n.y >= 0 && n.y < mazeHeight && !maze[n.x, n.y].visited)
            {
                neighbors.Add(n);
            }
        }

        return neighbors;
    }

    void BreakWall(Vector2Int currentCell, Vector2Int nextCell)
    {
        if (currentCell.x > nextCell.x)
        {
            // Remove left wall of currentCell
            maze[currentCell.x, currentCell.y].leftWall = false;
        }
        else if (currentCell.x < nextCell.x)
        {
            // Remove left wall of nextCell
            maze[nextCell.x, nextCell.y].leftWall = false;
        }
        else if (currentCell.y < nextCell.y)
        {
            // Remove top wall of currentCell
            maze[currentCell.x, currentCell.y].topWall = false;
        }
        else if (currentCell.y > nextCell.y)
        {
            // Remove top wall of nextCell
            maze[nextCell.x, nextCell.y].topWall = false;
        }
    }

    void CarvePath(int x, int y)
    {
        // Ensure the start position is within the maze bounds
        if (x < 0 || x >= mazeWidth || y < 0 || y >= mazeHeight)
        {
            x = y = 0; // Reset to the starting point of the maze
        }

        currentCell = new Vector2Int(x, y);
        maze[currentCell.x, currentCell.y].visited = true;

        Stack<Vector2Int> path = new Stack<Vector2Int>();
        path.Push(currentCell);

        while (path.Count > 0)
        {
            Vector2Int current = path.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count == 0)
            {
                path.Pop(); // No unvisited neighbors, backtrack
            }
            else
            {
                Vector2Int neighbor = neighbors[Random.Range(0, neighbors.Count)];
                BreakWall(current, neighbor);

                maze[neighbor.x, neighbor.y].visited = true;
                path.Push(neighbor);
            }
        }
    }

    public Vector2Int GetStartCellPosition()
    {
        return new Vector2Int(startX, startY);
    }

    void PlaceExit()
    {
        float cellSize = 1f;

        Vector3 exitPosition = new Vector3(Random.Range(3, mazeWidth) * cellSize, 0.5f, Random.Range(3, mazeHeight) * cellSize);
        Instantiate(PortalPrefab, exitPosition, Quaternion.identity);
    }

    public void PlaceEnemies()
    {
        List<Vector2Int> occupiedPositions = new List<Vector2Int>();

        for (int i = 0; i < numberOfSpiders; i++)
        {
            Vector2Int enemyPosition = GenerateUniqueEnemyPosition(occupiedPositions);
            Instantiate(spider, new Vector3(enemyPosition.x, 0, enemyPosition.y), Quaternion.identity);
            occupiedPositions.Add(enemyPosition);
        }
        for (int i = 0; i < numberOfSpiders; i++)
        {
            Vector2Int enemyPosition = GenerateUniqueEnemyPosition(occupiedPositions);
            Instantiate(demon, new Vector3(enemyPosition.x, 0, enemyPosition.y), Quaternion.identity);
            occupiedPositions.Add(enemyPosition);
        }
        /*
         * 
         * for (int i = 0; i < numberOfSpiders; i++)
        {
            Vector2Int enemyPosition = GenerateUniqueEnemyPosition(occupiedPositions);
            Instantiate(illusion, new Vector3(enemyPosition.x, 0, enemyPosition.y), Quaternion.identity);
            occupiedPositions.Add(enemyPosition);
        }
         * 
         * **/
    }

    Vector2Int GenerateUniqueEnemyPosition(List<Vector2Int> occupiedPositions)
    {
        int mazeWidth = maze.GetLength(0);
        int mazeHeight = maze.GetLength(1);

        Vector2Int position = new Vector2Int(Random.Range(5, mazeWidth), Random.Range(5, mazeHeight));

        while (occupiedPositions.Contains(position))
        {
            position = new Vector2Int(Random.Range(5, mazeWidth), Random.Range(5, mazeHeight));
        }

        return position;
    }
}

public class MazeCell
{
    public bool visited;
    public int x, y;
    public bool topWall;
    public bool leftWall;

    public Vector2Int position
    {
        get
        {
            return new Vector2Int(x, y);
        }
    }

    public MazeCell(int x, int y)
    {
        this.x = x;
        this.y = y;
        visited = false;
        topWall = leftWall = true;
    }
}
