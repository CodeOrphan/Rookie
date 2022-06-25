using UnityEngine;
 
public class WindowsMode : MonoBehaviour
{
    public int width = 1920;
    public int height = 1080;
    
    void Awake()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Screen.SetResolution (width, height, false, 60);
        }
    }
}