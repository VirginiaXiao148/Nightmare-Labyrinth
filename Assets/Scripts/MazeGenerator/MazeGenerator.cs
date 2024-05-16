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
    public int numberOfSpiders = 10;

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
        //GenerateMaze(startX, startY);
        PlaceEnemies();

        return maze;

    }

    private List<Direction> directions = new List<Direction> {
        Direction.Up, Direction.Down, Direction.Left, Direction.Right
    };

    private List<Direction> GetRandomDirection()
    {
        List<Direction> copyDirections = new List<Direction>(directions);
        List<Direction> randomDirections = new List<Direction>();

        while (copyDirections.Count > 0)
        {
            int randomIndex = Random.Range(0, copyDirections.Count);
            randomDirections.Add(copyDirections[randomIndex]);
            copyDirections.RemoveAt(randomIndex);
        }

        return randomDirections;
    }

    bool IsCellValid(int x, int y)
    {
        if (x < 0 || x >= mazeWidth || y < 0 || y >= mazeHeight)
        {
            return false;
        }
        return !maze[x, y].visited;

    }

    Vector2Int CheckNeighbour()
    {
        // Get random directions
        List<Direction> rndDir = GetRandomDirection();

        foreach (Direction dir in rndDir)
        {
            // Set the neighbour
            Vector2Int neighbour = currentCell;

            switch (dir)
            {
                case Direction.Up:
                    neighbour.y++;
                    break;
                case Direction.Down:
                    neighbour.y--;
                    break;
                case Direction.Left:
                    neighbour.x--;
                    break;
                case Direction.Right:
                    neighbour.x++;
                    break;
            }

            // Check the cell is valid
            if (IsCellValid(neighbour.x, neighbour.y))
            {
                return neighbour;
            }
        }

        // Return the current cell if neighbour is not valid
        return currentCell;

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
        if (x < 0 || x >= mazeWidth || y < 0 || y >= mazeHeight)
        {
            x = y = 0;
        }

        currentCell = new Vector2Int(x, y);
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        path.Push(currentCell);

        while (path.Count > 0)
        {
            Vector2Int nextCell = CheckNeighbour();

            if (nextCell == currentCell)
            {
                // Backtrack if dead end is reached
                currentCell = path.Pop();
            }
            else
            {
                BreakWall(currentCell, nextCell);
                maze[currentCell.x, currentCell.y].visited = true;
                currentCell = nextCell;
                path.Push(currentCell);
            }
        }
    }

    public Vector2Int GetStartCellPosition()
    {

        // Devuelve la posición de inicio del laberinto
        return new Vector2Int(startX, startY);

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



    // Possible optimiazations:

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
                BreakWall(current, neighbor);
                Debug.Log($"Removing wall between ({current.x}, {current.y}) and ({neighbor.x}, {neighbor.y})");

                // Mark neighbor as visited
                maze[neighbor.x, neighbor.y].visited = true;
                stack.Push(neighbor);
            }
        }

        Debug.Log("Maze generation complete.");
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

}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
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