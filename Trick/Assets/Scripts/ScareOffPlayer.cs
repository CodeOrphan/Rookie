using UnityEngine;

public class ScareOffPlayer : MonoBehaviour
{
    public GameObject player;
    public GameObject ball;
    public GameObject badGirl;
    public float speed = 10f;
    public float time = 0.5f;
    public bool isScareOff = false;
    private float m_time = 0f;

    void Update()
    {
        if (isScareOff)
        {
            m_time = m_time + Time.deltaTime;
            player.transform.position = new Vector3(player.transform.position.x - Time.deltaTime * speed,
                player.transform.position.y, player.transform.position.z);
            if (m_time > time)
            {
                m_time = 0;
                isScareOff = false;
                UnfreezePlayer();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.body.gameObject == ball)
        {
            UnfreezePlayer();
            PlayBall();
            GetComponent<Collider>().enabled = false;
            this.enabled = false;
            badGirl.GetComponent<Collider>().enabled = false;
            ball.GetComponent<Collider>().enabled = false;
            return;
        }

        if (collision.body.gameObject == player)
        {
            Meow();
            FreezePlayer();
            isScareOff = true;
        }
    }

    private void UnfreezePlayer()
    {
        player.GetComponent<PlayerController>().enabled = true;
    }

    private void FreezePlayer()
    {
        player.GetComponent<PlayerController>().enabled = false;
    }

    private void PlayBall()
    {
    }

    private void Meow()
    {
    }
}