using System;
using UnityEngine;


    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;

        public void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, Time.deltaTime * 500f);
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                _rigidbody2D.velocity = Vector2.zero;
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, -Time.deltaTime * 500f);
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                _rigidbody2D.velocity = Vector2.zero;
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                _rigidbody2D.velocity = new Vector2(-Time.deltaTime * 500f, _rigidbody2D.velocity.y);
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                _rigidbody2D.velocity = Vector2.zero;
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                _rigidbody2D.velocity = new Vector2(Time.deltaTime * 500f,  _rigidbody2D.velocity.y);
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                _rigidbody2D.velocity = Vector2.zero;
            }
        }
    }
