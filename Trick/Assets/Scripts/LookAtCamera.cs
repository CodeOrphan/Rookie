using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        float y = Camera.main.transform.eulerAngles.y;
        transform.eulerAngles = new Vector3( transform.eulerAngles.x, y, transform.eulerAngles.z);
    }
}
