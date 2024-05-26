using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    public float moveSpeed;
    Rigidbody2D rb;
    Gyroscope steerGyro;

    // Start is called before the first frame update
    void Start()
    {
        steerGyro = Input.gyro;
        steerGyro.enabled = true;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Check if gyroscope is available
        if (SystemInfo.supportsGyroscope)
        {
            // Use gyroscope input
            Quaternion gyroInput = steerGyro.attitude;
            if (gyroInput == null)
            {
                rb.velocity = Vector2.zero;
            } 
            else
            {
                if (gyroInput.x < 0)
                {
                    rb.AddForce(Vector2.left * moveSpeed);
                }
                else
                {
                    rb.AddForce(Vector2.right * moveSpeed);
                }
            }

        }
        else
        {
            // Use touch input
            if (Input.GetMouseButton(0))
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (touchPosition.x < 0)
                {
                    rb.AddForce(Vector2.left * moveSpeed);
                }
                else
                {
                    rb.AddForce(Vector2.right * moveSpeed);
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Block")
        {
            GameHandler.instance.Win(200);
        }
    }
}
