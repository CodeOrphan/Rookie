using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeowPerMinute : MonoBehaviour
{
    public float targetTime = 60;
    public AudioSource audio;
    private float time = 0;
    
    void Update()
    {
        if (time > targetTime)
        {
            if (audio)
            {
                PlayerController playerController = GetComponent<PlayerController>();
                if (playerController.MoveState.CurrentState == XPlayerState.Idle)
                {
                    audio.Play();   
                    time = 0;
                }
            }
            return;
        }

        time = time + Time.deltaTime;
    }
}
