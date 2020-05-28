using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PolylineController : MonoBehaviour {

    public GameObject arrow;
    private float distance;
    NavMeshPath path;
    NavMeshPath lastPath;
    public Transform destination;
    public NavMeshAgent agent;
    private LineRenderer lineRenderer;

    // Use this for initialization
    void Start () {
        path = new NavMeshPath();
        lastPath = new NavMeshPath();
        lineRenderer = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        CalculatePath();
    }

    public void CalculatePath()
    {
        agent.CalculatePath(destination.position, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
            lineRenderer.enabled = true;
            Debug.Log(path.corners);
        }
        else
        {
            // lineRenderer.enabled = false;
        }
    }

    public void DrawArrow(NavMeshPath path)
    {
        Debug.Log(path.corners);
        for(int i = 0; i < path.corners.Length - 1; i++)
        {
            Vector3 startPosition = path.corners[i];
            Vector3 endPosition = path.corners[i + 1];
            Vector3 dir = endPosition - startPosition;
            dir = dir.normalized;
            transform.position = startPosition;
            transform.forward = dir;
            distance = Distance(startPosition, endPosition);
            NormalRoute();
        }
    }

    public void NormalRoute()
    {
        Debug.Log("hihi");
        for (int j = 0; j <= Mathf.Floor(distance); j+=6)
        {
            Vector3 pos = new Vector3(transform.position.x, 0.2f, transform.position.z);
            GameObject go = Instantiate(arrow, pos + transform.forward * j, transform.rotation);
            go.transform.localScale = new Vector3(0.8f, 0.8f, 1);
        }
    }

    float Distance(Vector3 p1, Vector3 p2)
    {
        return (float)Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.z - p1.z, 2));
    }
}
