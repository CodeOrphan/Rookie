using UnityEngine;

public class PlayAudioOnceOnTouch : MonoBehaviour
{
    public AudioSource audio;
    public bool Play = true;

    void OnTriggerEnter(Collider other)
    {
        if (!Play)
        {
            return;
        }
        if (other.GetComponent<PlayerController>())
        {
            Play = false;
            audio.Play();
        }
    }
}
