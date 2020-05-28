using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPointTask : MonoBehaviour
{
    private LineRenderer lineRenderer = null;
    public Camera FixedMapCamera;
    Vector2 endPoint;
    Vector2 startPoint;
    Vector2 destinationPoint;
    float distanceDiff;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        destinationPoint = new Vector2(VROption.endPos.x, VROption.endPos.z);
    }

    private void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        // Use default or distance
        RaycastHit hit = CreateRaycast();

        // If nothing is hit, set do default length
        float colliderDistance = hit.distance;
        float targetLength = colliderDistance;

        // Default
        Vector3 endPosition = new Vector3(FixedMapCamera.transform.position.x, 0, FixedMapCamera.transform.position.z);
        endPoint = new Vector2(endPosition.x, endPosition.z);
        startPoint = new Vector2(FixedMapCamera.transform.position.x, FixedMapCamera.transform.position.z);

        distanceDiff = Vector2.Distance(endPoint, destinationPoint);
     

        // Set linerenderer
        //lineRenderer.SetPosition(0, FixedMapCamera.transform.position);
        //lineRenderer.SetPosition(1, new Vector3(FixedMapCamera.transform.position.x, 0, FixedMapCamera.transform.position.z));
        //lineRenderer.SetPosition(2, VROption.endPos);
    }

    public float GetDistanceDiff()
    {
        Debug.Log(distanceDiff);
        return distanceDiff;
    }

    private RaycastHit CreateRaycast()
    {
        RaycastHit hit;
        Ray ray = new Ray(FixedMapCamera.transform.position, new Vector3(FixedMapCamera.transform.position.x, 0, FixedMapCamera.transform.position.z));
        Physics.Raycast(ray, out hit, 200);

        return hit;
    }
}
