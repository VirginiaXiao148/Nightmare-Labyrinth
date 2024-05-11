using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellObjects : MonoBehaviour{
    [SerializeField] GameObject topWall;
    [SerializeField] GameObject rightWall;
    [SerializeField] GameObject bottomWall;
    [SerializeField] GameObject leftWall;

    public void Init (bool top, bool right, bool bottom, bool left){
        topWall.SetActive(top);
        rightWall.SetActive(right);
        bottomWall.SetActive(bottom);
        leftWall.SetActive(left);
    }

}