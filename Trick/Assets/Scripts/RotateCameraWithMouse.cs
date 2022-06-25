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
        m_initAngle = NormalizeEuler(transform.eulerAngles.y);
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
        float angle = distance / Screen.currentResolution.width * 2 * 360;
        angle = NormalizeEuler(transform.eulerAngles.y + angle);

        if (angle < m_initAngle - 90)
        {
            return;
        }

        if (angle > m_initAngle + 90)
        {
            return;
        }

        Vector3 target = new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);
        transform.eulerAngles = target;
    }

    private float NormalizeEuler(float y)
    {
        if (y < -180f)
            y = y + 360f;
        else if (y > 180f)
            y = y - 360f;
        return y;
    }
}