using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DelayAnimator : MonoBehaviour
{
    public float delay = 0;
    private float m_time = 0;

    void Update()
    {
        m_time = m_time + Time.deltaTime;
        if (m_time > delay)
        {
            GetComponent<Animator>().enabled = true;
            enabled = false;
        }
    }
}
