using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        if (Camera.main == null)
        {
            return;
        }
        float y = Camera.main.transform.eulerAngles.y;
        transform.eulerAngles = new Vector3( transform.eulerAngles.x, y, transform.eulerAngles.z);
    }
}
