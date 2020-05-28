using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRController : MonoBehaviour
{
    // HTC Vive Touchpad Input
    public SteamVR_Action_Single m_MovePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;

    private Transform m_CameraRig = null;
    private Transform m_Head = null;
    public float m_Sensitivity = 0.1f;
    public float m_MaxSpeed;
    private CharacterController m_CharacterController;
    Vector3 lastPosition;

    public static bool isWalkable;

    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_CameraRig = SteamVR_Render.Top().origin;
        m_Head = SteamVR_Render.Top().head;
    }

    // Update is called once per frame
    void Update()
    {
        HandleHead();
        HandleHeight();
        CalculateMovement();
    }

    private void HandleHead()
    {
        // Store current
        Vector3 oldPosition = m_CameraRig.position;
        Quaternion oldRotation = m_CameraRig.rotation;

        // Rotation
        transform.eulerAngles = new Vector3(0.0f, m_Head.rotation.eulerAngles.y, 0.0f);

        // Restore
        m_CameraRig.position = oldPosition;
        m_CameraRig.rotation = oldRotation;
    }

    private void CalculateMovement()
    {
        Vector3 orientationEuler = new Vector3(0, transform.eulerAngles.y, 0);
        Quaternion orientation = Quaternion.Euler(orientationEuler);
        Vector3 movement = Vector3.zero;

        Vector2 padPos = m_MoveValue.GetAxis(SteamVR_Input_Sources.RightHand);
        float triggerValue = m_MovePress.GetAxis(SteamVR_Input_Sources.RightHand);

        // If touch touchpad
        if (padPos != new Vector2(0,0))
        {
            // 讓手指不管放在觸控板哪邊都速度一致
            // 計算x跟y方向之間的夾角
            float rad =  Mathf.Atan2(padPos.y, padPos.x);
            movement = orientation * (1 * Mathf.Sin(rad) * Vector3.forward + 1 * Mathf.Cos(rad) * Vector3.right) * Time.deltaTime * m_MaxSpeed;
        }

        // Apply
        if(isWalkable)
            m_CharacterController.Move(movement);
    }

    private void HandleHeight()
    {
        // Get the head in local space
        float headHeight = Mathf.Clamp(m_Head.localPosition.y, 1, 2);
        m_CharacterController.height = headHeight;

        // Cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = m_CharacterController.height / 2;
        newCenter.y += m_CharacterController.skinWidth;

        // Move capsule in local space
        newCenter.x = m_Head.localPosition.x;
        newCenter.z = m_Head.localPosition.z;

        // Rotate
        newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;

        // Apply
        m_CharacterController.center = newCenter;
    }
}
