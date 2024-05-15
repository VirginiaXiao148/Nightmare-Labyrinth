using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellOptimized : MonoBehaviour
{
    public bool visited;
    public int x, y;

    [SerializeField] GameObject topWallOptimized;
    [SerializeField] GameObject rightWallOptimized;
    [SerializeField] GameObject bottomWallOptimized;
    [SerializeField] GameObject leftWallOptimized;

    public void Init( bool top, bool right, bool bottom, bool left)
    {
        topWallOptimized.SetActive(top);
        rightWallOptimized.SetActive(right);
        bottomWallOptimized.SetActive(bottom);
        leftWallOptimized.SetActive(left);
    }


    public Vector2Int position
    {
        get
        {
            return new Vector2Int(x, y);
        }
    }

    // Unity's Awake method for initialization
    void Awake()
    {
        visited = false;
        Init(true, true, true, true); // Initialize all walls as active
    }

    public void RemoveWall(MazeCellOptimized neighbor)
    {
        // Determine direction of neighbor and remove corresponding walls
        if (neighbor.transform.position.x > transform.position.x) // Neighbor is to the right
        {
            rightWallOptimized.SetActive(false);
            neighbor.leftWallOptimized.SetActive(false);
        }
        else if (neighbor.transform.position.x < transform.position.x) // Neighbor is to the left
        {
            leftWallOptimized.SetActive(false);
            neighbor.rightWallOptimized.SetActive(false);
        }
        else if (neighbor.transform.position.z > transform.position.z) // Neighbor is above
        {
            topWallOptimized.SetActive(false);
            neighbor.bottomWallOptimized.SetActive(false);
        }
        else if (neighbor.transform.position.z < transform.position.z) // Neighbor is below
        {
            bottomWallOptimized.SetActive(false);
            neighbor.topWallOptimized.SetActive(false);
        }
    }

}