using UnityEngine;

public class BallCrossDoor : MonoBehaviour
{
    public GameObject nextBall;
    public NextScene door;

    private void OnEnable()
    {
        // 通关后把限制距离干掉
        if (door.isPassed)
        {
            LimitPosition limitPosition = GetComponent<LimitPosition>();
            limitPosition.banSlider = true;
        }
    }

    public void Update()
    {
        LimitPosition limitPosition = GetComponent<LimitPosition>();
        if (door && door.isPassed && transform.position.x > 31 )
        {
            gameObject.SetActive(false);
            if (nextBall)
            {
                nextBall.gameObject.SetActive(true);
            }
        }
    }
}
