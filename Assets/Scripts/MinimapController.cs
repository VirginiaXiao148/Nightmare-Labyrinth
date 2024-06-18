using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public Transform player; // Asigna el transform del jugador
    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        AddPoint(player.position); // Agregar la posición inicial del jugador
    }

    void Update()
    {
        if (Vector3.Distance(player.position, points[points.Count - 1]) > 0.1f)
        {
            AddPoint(player.position);
        }
    }

    private void AddPoint(Vector3 point)
    {
        points.Add(point);
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}

