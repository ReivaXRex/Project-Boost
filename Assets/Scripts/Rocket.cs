using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    private Rigidbody rb;
    private AudioSource audioSource;

    [SerializeField] int thrustPower;
    [SerializeField] int rotateThrustPower;
    [SerializeField] float levelLoadDelay = 1f;
    
    [SerializeField] AudioClip rocketEngine;
    [SerializeField] AudioClip deathAudio;

    [SerializeField] ParticleSystem engineSFX;
    [SerializeField] ParticleSystem deathSFX;
    [SerializeField] ParticleSystem winSFX;

    private enum State
    { Alive, Dying, Transcending }

    [SerializeField] private State state = State.Alive;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (state != State.Dying)
        {
            RespondToInput();
            Rotate();
        }
    }

    private void RespondToInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyForce();
        }
        else
        {
            StopThrustEffects();
        }
    }

    private void StopThrustEffects()
    {
        audioSource.Stop();
        engineSFX.Stop();
    }

    private void ApplyForce()
    {
        rb.AddRelativeForce(Vector3.up * thrustPower * Time.deltaTime);

        if (!audioSource.isPlaying) // Prevent clip from layering.
        {
            audioSource.PlayOneShot(rocketEngine);
        }

        engineSFX.Play();
    }

    private void Rotate()
    {
        rb.angularVelocity = Vector3.zero; // remove rotation due to Physics.

        float rotation = rotateThrustPower * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotation);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotation);
        }

       
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
        {
            return;
        }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
              //  Debug.Log(collision.gameObject.name);
                break;

            case "Obstacles":
              //  Debug.Log(collision.gameObject.name + " You died");
                StartDeathSequence();
                break;

            case "Finish":
               // Debug.Log(collision.gameObject.name + " You won!");
                StartLoadSequence();
                break;

            default:
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathAudio);
        deathSFX.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartLoadSequence()
    {
        state = State.Transcending;
        //   audioSource.Stop();
        //   audioSource.PlayOneShot(loadAudio);
        winSFX.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        if (state == State.Dying)
        {
            SceneManager.LoadScene(0);
            state = State.Alive;
        }
        else if (state == State.Transcending)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = 0;

            switch (currentSceneIndex)
            {
                case 0:
                    nextSceneIndex = 1;
                    break;
                case 1:
                    nextSceneIndex = 2;
                    break;
                case 2:
                    nextSceneIndex = 0;
                    break;
                default:
                    break;
            }
          
            SceneManager.LoadScene(nextSceneIndex);
            state = State.Alive;
        }
    }
}