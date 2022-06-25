using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPositionOnEnable : MonoBehaviour
{
    public Vector3 position;
    private bool m_isInit = false;

    void Start()
    {
        position = transform.position;
        m_isInit = true;
    }

    private void OnEnable()
    {
        if (m_isInit)
        {
            transform.position = position;
        }
    }
}