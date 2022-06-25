using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraWithMouse : MonoBehaviour
{
    private float m_lastX = 0;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern int SetCursorPos(int x, int y);

    private void Start()
    {
        SetCursorPos(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2);
        m_lastX = Input.mousePosition.x;
    }

    void Update()
    {
        float x = Input.mousePosition.x;
        float distance = x - m_lastX;
        m_lastX = x;
        float angle = distance / Screen.currentResolution.width / 2 * 90;
        Vector3 target = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + angle, transform.eulerAngles.z);
        transform.eulerAngles = target;
    }
}