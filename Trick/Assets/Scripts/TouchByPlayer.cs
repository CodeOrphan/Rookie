using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchByPlayer : MonoBehaviour
{
    public AudioSource audio;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.body.GetComponent<PlayerController>())
        {
            if (audio)
            {
                audio.Play();
            }
        }
    }
}
