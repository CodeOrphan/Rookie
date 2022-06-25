using System;
using System.Collections;
using UnityEngine;

public class NextScene : MonoBehaviour
{
    public GameObject thisScene;
    public GameObject nextScene;
    private PlayerController PlayerController;
    
    public bool isPassed = false;
    public GameObject ball;
    public void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController)
        {
            // FadeInOut.FadeInOutInstance.BackGroundControl(true);
            // if(nextScene)
            //     nextScene.SetActive(true);
            // if(thisScene)
            //     thisScene.SetActive(false);
            PlayerController = playerController;
            FadeInOut.FadeInOutInstance.ReSpawnSceneFade(thisScene, nextScene);

            if (ball)
            {
                 Rigidbody rigidbody = ball.GetComponent<Rigidbody>();
                 
            }
            isPassed = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController)
        {
            // FadeInOut.FadeInOutInstance.BackGroundControl(true);
            // if(nextScene)
            //     nextScene.SetActive(true);
            // if(thisScene)
            //     thisScene.SetActive(false);
            PlayerController = playerController;
            
        }
    }


}