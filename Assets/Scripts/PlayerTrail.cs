using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    public LineRenderer mainLineRenderer; // Referencia al LineRenderer principal
    public LineRenderer minimapLineRenderer; // Referencia al LineRenderer del minimapa
    public float pointSpacing = 0.1f; // Espaciado entre los puntos del rastro
    public Transform minimapCamera;   // Referencia a la cámara del minimapa

    private List<Vector3> points;     // Lista de puntos del rastro

    void Start()
    {
        if (mainLineRenderer == null)
        {
            Debug.LogError("Main LineRenderer is not assigned.");
        }
        if (minimapLineRenderer == null)
        {
            Debug.LogError("Minimap LineRenderer is not assigned or not found.");
        }
        if (minimapCamera == null)
        {
            Debug.LogError("Minimap Camera is not assigned.");
        }

        points = new List<Vector3>();
        AddPoint();
    }

    void Update()
    {
        if (points.Count == 0 || mainLineRenderer == null || minimapLineRenderer == null)
        {
            return; // Si no hay puntos o si los LineRenderer no están asignados, salir del método Update
        }

        if (Vector3.Distance(points[points.Count - 1], transform.position) > pointSpacing)
        {
            AddPoint();
        }

        UpdateLineRenderer(mainLineRenderer, false);
        UpdateLineRenderer(minimapLineRenderer, true);
    }

    void AddPoint()
    {
        points.Add(transform.position);
    }

    void UpdateLineRenderer(LineRenderer lineRenderer, bool isMinimap)
    {
        if (lineRenderer == null)
        {
            return; // Si el LineRenderer no está asignado, salir del método
        }

        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 point = points[i];
            if (isMinimap && minimapCamera != null)
            {
                // Ajustar la altura de los puntos para que se vean en el minimapa
                point.y = minimapCamera.position.y - 1f; // Ajusta la altura según sea necesario
            }
            lineRenderer.SetPosition(i, point);
        }
    }
}
