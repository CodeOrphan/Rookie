using System;
using System.Collections;
using UnityEngine;

public class NextScene : MonoBehaviour
{
    public GameObject thisScene;
    public GameObject nextScene;
    private PlayerController PlayerController;
    
    public bool isPassed = false;
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
            isPassed = true;
            if (thisScene.name == "Level3")
            {
                GameController.Instance.PassLevelCount++;
            }
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