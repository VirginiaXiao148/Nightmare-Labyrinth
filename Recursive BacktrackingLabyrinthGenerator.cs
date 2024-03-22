using System.Collections.Generic;
using UnityEngine;

public class LaberintoGenerator : MonoBehaviour
{
    public int width = 50;
    public int height = 50;
    public GameObject wallPrefab;

    private int[,] maze;
    private Stack<Vector2Int> stack;

    void Start()
    {
        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        maze = new int[width, height];
        stack = new Stack<Vector2Int>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = 1; // 1 representa una pared
            }
        }

        Vector2Int startCell = new Vector2Int(1, 1);
        stack.Push(startCell);
        maze[startCell.x, startCell.y] = 0; // Marcar la celda inicial como parte del laberinto

        while (stack.Count > 0)
        {
            Vector2Int currentCell = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(currentCell);

            if (neighbors.Count > 0)
            {
                Vector2Int randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
                RemoveWall(currentCell, randomNeighbor);
                stack.Push(randomNeighbor);
                maze[randomNeighbor.x, randomNeighbor.y] = 0; // Marcar la celda vecina como parte del laberinto
            }
            else
            {
                stack.Pop();
            }
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Agregar vecinos no visitados
        if (cell.x + 2 < width) neighbors.Add(new Vector2Int(cell.x + 2, cell.y));
        if (cell.x - 2 > 0) neighbors.Add(new Vector2Int(cell.x - 2, cell.y));
        if (cell.y + 2 < height) neighbors.Add(new Vector2Int(cell.x, cell.y + 2));
        if (cell.y - 2 > 0) neighbors.Add(new Vector2Int(cell.x, cell.y - 2));

        // Filtrar vecinos no visitados
        neighbors = neighbors.FindAll(n => maze[n.x, n.y] == 1);

        return neighbors;
    }

    void RemoveWall(Vector2Int current, Vector2Int neighbor)
    {
        Vector2Int wall = (current + neighbor) / 2;
        maze[wall.x, wall.y] = 0; // Marcar la pared entre las celdas como pasable
    }

    /* void DrawMaze()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (maze[i, j] == 1)
                {
                    Instantiate(wallPrefab, new Vector3(i, 0, j), Quaternion.identity);
                }
            }
        }
    } */

    void DrawMaze()
    {
        Transform wallsParent = new GameObject("Walls").transform;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (maze[i, j] == 1)
                {
                    Instantiate(wallPrefab, new Vector3(i, 0, j), Quaternion.identity, wallsParent);
                }
            }
        }
    }
}