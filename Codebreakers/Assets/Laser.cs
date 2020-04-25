using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform hitPoint;
    public Transform shootingPoint;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
    }

    private void Update()
    {
        transform.position = shootingPoint.position;
        transform.rotation = shootingPoint.rotation;
    }

    public void EnableLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right);

        if (hit.transform != null)
            hitPoint.position = hit.point;
        else
            hitPoint.position = transform.position * 100f;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hitPoint.position);
        lineRenderer.enabled = true;

    }

    public void DisableLaser()
    {
        lineRenderer.enabled = false;

    }
}
