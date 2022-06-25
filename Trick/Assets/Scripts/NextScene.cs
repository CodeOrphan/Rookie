using UnityEngine;

public class NextScene : MonoBehaviour
{
    public GameObject thisScene;
    public GameObject nextScene;
    public bool isPassed = false;
    public GameObject ball;
    
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
            if (ball)
            {
                 Rigidbody rigidbody = ball.GetComponent<Rigidbody>();
                 
            }
            isPassed = true;
        }
    }
}
