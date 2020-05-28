using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTester : MonoBehaviour
{
    public Transform reference;
    private float distance;
    public GameObject arrow;
    Vector3 destination;
    public NavMeshAgent agent;
    public LineRenderer referenceLine;
    public LineRenderer lineRenderer;
    NavMeshPath path;
    NavMeshPath referencePath;
    public GameObject player;
    private string condition;

    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        referencePath = new NavMeshPath();
        destination = VROption.endPos;
        condition = VROption.condition;
    }

    public void CallGenerateArrow()
    {
        InvokeRepeating("UpdateArrow", 0, 5);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDestinationPath();
    }

    public void UpdateArrow()
    {
        
        if (condition == "B")
        {
            CleanArrow();
            GenerateArrow();
        }
    }

    private void CalculateDestinationPath()
    {
        // 計算往目的地的路線
        transform.position = player.transform.position;
        
        agent.CalculatePath(destination, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
            for(int i=0; i<lineRenderer.positionCount; i++)
            {
                Vector3 pos = lineRenderer.GetPosition(i);
                pos.y = 1f;
                lineRenderer.SetPosition(i, pos);
            }
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void CleanArrow()
    {
        // 清除場景中的AR箭頭
        GameObject[] arrows;

        arrows = GameObject.FindGameObjectsWithTag("Arrow");

        foreach (GameObject arrow in arrows)
        {
            Destroy(arrow);
        }
    }

    private void GenerateArrow()
    {
        // 產生AR模式中的箭頭
        for (int i = 0; i < path.corners.Length - 1; i++)
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
        for (int j = 0; j <= Mathf.Floor(distance); j += 6)
        {
            Vector3 pos = new Vector3(transform.position.x, 0.2f, transform.position.z);
            GameObject go = Instantiate(arrow, pos + transform.forward * j, transform.rotation);
            go.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            go.name = "Arrow";
            go.tag = "Arrow";
        }
    }

    float Distance(Vector3 p1, Vector3 p2)
    {
        return (float)Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.z - p1.z, 2));
    }
}
