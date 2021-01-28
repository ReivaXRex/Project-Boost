using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rb;
    AudioSource thrustAudio;

    [SerializeField] int thrustPower;
    [SerializeField] int rotateThrustPower;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        thrustAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        Thrust();
        Rotate();
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddRelativeForce(Vector3.up * thrustPower);
            
            if (!thrustAudio.isPlaying) // Prevent clip from layering.
            {
                thrustAudio.Play();
            }
        }

        else
        {
            thrustAudio.Stop();
        }      
    }

    private void Rotate()
    {
        rb.freezeRotation = true; // take manual control of rotation.
       
        float rotation = rotateThrustPower * Time.deltaTime;
        
        if (Input.GetKey(KeyCode.A))
        {
         
            transform.Rotate(Vector3.forward * rotation);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotation);
        }

        rb.freezeRotation = false; // resume physics control of rotation.

    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                Debug.Log(collision.gameObject.name);
                break;
            case "Obstacles":
                Debug.Log(collision.gameObject.name + " You died");
                break;

            default:
                break;
        }
    }
}
