using UnityEngine;

public class NextScene : MonoBehaviour
{
    public GameObject thisScene;
    public GameObject nextScene;
    public void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController)
        {
            // FadeInOut.FadeInOutInstance.BackGroundControl(true);
            if(nextScene)
                nextScene.SetActive(true);
            if(thisScene)
                thisScene.SetActive(false);
        }
    }
}
