using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour{

    [Range(5, 100)]
    public int mazeWidth = 10, mazeHeight = 10;  // the dimensions of the maze
    public int startX, startY;  // the starting position

    public GameObject wallPrefab;

    public Texture[] wallTextures;  // The textures for the walls

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
        PlaceEnemies();

        return maze;

    }

    private static Direction[] directions = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

    private void ShuffleDirections()
    {
        for (int i = 0; i < directions.Length; i++)
        {
            int rand = Random.Range(i, directions.Length);
            Direction temp = directions[i];
            directions[i] = directions[rand];
            directions[rand] = temp;
        }
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
        ShuffleDirections();

        foreach (Direction dir in directions)
        {
            Vector2Int neighbour = currentCell + DirectionToVector(dir);

            if (IsCellValid(neighbour.x, neighbour.y))
            {
                return neighbour;
            }
        }

        return currentCell;  // Considerar ajustar esta parte para mejorar la lógica de retroceso
    }

    Vector2Int DirectionToVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return new Vector2Int(0, 1);
            case Direction.Down:
                return new Vector2Int(0, -1);
            case Direction.Left:
                return new Vector2Int(-1, 0);
            case Direction.Right:
                return new Vector2Int(1, 0);
        }
        return Vector2Int.zero;
    }

    void BreakWall(Vector2Int currentCell, Vector2Int nextCell){
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

            if (nextCell != currentCell)
            {
                BreakWall(currentCell, nextCell);
                maze[currentCell.x, currentCell.y].visited = true;

                if (!maze[nextCell.x, nextCell.y].visited)
                {
                    GameObject wall = Instantiate(wallPrefab, new Vector3(nextCell.x, 0, nextCell.y), Quaternion.identity);
                    Renderer wallRenderer = wall.GetComponent<Renderer>();
                    wallRenderer.material.mainTexture = wallTextures[Random.Range(0, wallTextures.Length)];
                }

                currentCell = nextCell;
                path.Push(currentCell);
            }
        }
    }

    public Vector2Int GetStartCellPosition(){

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