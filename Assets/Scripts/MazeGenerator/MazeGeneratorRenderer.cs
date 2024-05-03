using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorRenderer : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] GameObject mazeCellPrefab;

    public float cellSize = 1f;

    // Start is called before the first frame update
    private void Start(){

        MazeCell[,] maze = mazeGenerator.GetMaze();

        for(int x = 0; x < mazeGenerator.mazeWidth; x++){

            for(int y = 0; y < mazeGenerator.mazeHeight; y++){

                GameObject newCell = Instantiate(mazeCellPrefab, new Vector3((float)x * cellSize, 0f, (float)y * cellSize), Quaternion.identity, transform);
                MazeCellObjects mazeCell = newCell.GetComponent<MazeCellObjects>();

                bool top = maze[x,y].topWall;
                bool left = maze[x,y].leftWall;
                

                bool right = false;
                bool bottom = false;
                
                if(x == mazeGenerator.mazeWidth - 1){

                    right = true;

                }

                if(y == 0){

                    bottom = true;
                    
                }

                mazeCell.Init(top, right, bottom, left);
            }
        }
    }
}
