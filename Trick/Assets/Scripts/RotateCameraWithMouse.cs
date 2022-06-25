using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RotateCameraWithMouse : MonoBehaviour
{
    private const float m_none = -999999;
    private float m_lastX = m_none;
    private float m_initAngle = 0;

    private void OnEnable()
    {
        m_initAngle = transform.eulerAngles.y;
    }

    void Update()
    {
        // 不在屏幕内
        if (!new Rect(0, 0, Screen.width, Screen.height).Contains(Input.mousePosition)) return;

        float x = Input.mousePosition.x;
        if (m_lastX == m_none || x - m_lastX > 200)
        {
            m_lastX = x;
            return;
        }

        float distance = x - m_lastX;
        m_lastX = x;
        float angle = distance / Screen.currentResolution.width / 2 * 360;
        angle = transform.eulerAngles.y + angle;
        // angle = math.clamp(angle, m_initAngle - 90, m_initAngle + 90);
        Vector3 target = new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);
        transform.eulerAngles = target;
    }
}