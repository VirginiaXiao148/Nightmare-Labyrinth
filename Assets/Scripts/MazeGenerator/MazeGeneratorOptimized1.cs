using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeGeneratorOptimized1 : MonoBehaviour
{

    [Range(5, 100)]
    public int mazeWidth = 10, mazeHeight = 10;  // the dimensions of the maze
    
    public GameObject beaceletPrefab;
    public GameObject spider;
    public int numberOfSpiders = 3;

    public float cellSize = 1f;

    public MazeCellOptimized mazeCellPrefab;
    MazeCellOptimized[,] maze;

    void Start()
    {
        InitializeMaze();
        GenerateMaze(0, 0); // Start generation at the top-left corner of the maze
        PlaceExit();        // Place the maze exit
        PlaceEnemies();     // Place enemies within the maze
    }

    void InitializeMaze()
    {
        maze = new MazeCellOptimized[mazeWidth, mazeHeight];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                // Instantiate the prefab at the correct position
                GameObject newCell = Instantiate(mazeCellPrefab, new Vector3((float)x * cellSize, 0f, (float)y * cellSize), Quaternion.identity, transform);
                MazeCellOptimized mazeCell = newCell.GetComponent<MazeCellOptimized>();

                // Initialize the cell with its position and all walls active
                mazeCell.Init(true, true, true, true);

                // Store the maze cell in the array for later use
                maze[x, y] = mazeCell;
            }
        }
    }

    void GenerateMaze(int startX, int startY)
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int start = new Vector2Int(startX, startY);
        maze[start.x, start.y].visited = true;
        stack.Push(start);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count == 0)
            {
                // No unvisited neighbors, backtrack
                stack.Pop();
            }
            else
            {
                // Visit a random unvisited neighbor
                Vector2Int neighbor = neighbors[Random.Range(0, neighbors.Count)];
                // Break the wall between current and neighbor
                maze[current.x, current.y].RemoveWall(maze[neighbor.x, neighbor.y]);

                // Mark neighbor as visited
                maze[neighbor.x, neighbor.y].visited = true;
                stack.Push(neighbor);
            }
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        Vector2Int[] possibleNeighbours = {
            new Vector2Int(cell.x, cell.y + 1), // North
            new Vector2Int(cell.x + 1, cell.y), // East
            new Vector2Int(cell.x, cell.y - 1), // South
            new Vector2Int(cell.x - 1, cell.y)  // West
        };

        foreach (var n in possibleNeighbours)
        {
            if (n.x >= 0 && n.x < mazeWidth && n.y >= 0 && n.y < mazeHeight && !maze[n.x, n.y].visited)
            {
                neighbours.Add(n);
            }
        }

        return neighbours;
    }

    void PlaceExit()
    {
        int exitX = mazeWidth - 1;
        int exitY = mazeHeight - 1;

        Vector3 exitPosition = new Vector3(exitX * cellSize, 0f, exitY * cellSize);
        Instantiate(beaceletPrefab, exitPosition, Quaternion.identity);
    }

    public void PlaceEnemies()
    {
        // List to store positions occupied by enemies
        List<Vector2Int> occupiedPositions = new List<Vector2Int>();

        // Generate unique positions for each enemy
        for (int i = 0; i < numberOfSpiders; i++)
        {
            Vector2Int enemyPosition = GenerateUniqueEnemyPosition(occupiedPositions);

            // Instantiate enemy at the generated position
            Instantiate(spider, new Vector3(enemyPosition.x, 0, enemyPosition.y), Quaternion.identity);

            // Add the position occupied by the enemy to the list
            occupiedPositions.Add(enemyPosition);
        }
    }

    Vector2Int GenerateUniqueEnemyPosition(List<Vector2Int> occupiedPositions)
    {
        int mazeWidth = maze.GetLength(0);
        int mazeHeight = maze.GetLength(1);

        // Generate a random position within the maze
        Vector2Int position = new Vector2Int(Random.Range(0, mazeWidth), Random.Range(5, mazeHeight));

        // Check if the position is occupied by another enemy
        while (occupiedPositions.Contains(position))
        {
            // Generate a new random position if the current one is occupied
            position = new Vector2Int(Random.Range(0, mazeWidth), Random.Range(5, mazeHeight));
        }
        return position;
    }
    
}