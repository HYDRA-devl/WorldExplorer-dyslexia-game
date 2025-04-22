using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class birdmove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 5f;        // How powerful the jump/flap is
    public float maxVelocity = 8f;      // Prevents falling too fast
    public float rotationSpeed = 10f;   // How quickly the bird rotates when jumping/falling

    [Header("References")]
    public AudioClip flapSound;         // Sound when flapping

    // Private variables
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private bool isAlive = true;

    void Start()
    {
        // Get component references
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // Make sure gravity is applied
        if (rb != null)
        {
            rb.gravityScale = 2f;
        }
    }

    void Update()
    {
        if (!isAlive) return;

        // Jump when player presses space or left mouse button
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Jump();
        }

        // Rotate bird based on velocity
        if (rb.velocity.y > 0)
        {
            // Point up when rising
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 0, 30),
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // Point down when falling
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 0, -30),
                rotationSpeed * Time.deltaTime
            );
        }

        // Limit fall speed
        if (rb.velocity.y < -maxVelocity)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxVelocity);
        }
    }

    void Jump()
    {
        // Reset y velocity and apply upward force
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Play flap sound
        if (audioSource != null && flapSound != null)
        {
            audioSource.PlayOneShot(flapSound);
        }
    }

    // Call this when the bird collides with something that should end the game
    public void Die()
    {
        isAlive = false;
        rb.velocity = Vector2.zero;

        // Optional: Disable collider to prevent further collisions
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // For now, we'll just print a message
        // You can add your GameManager class later
        Debug.Log("Game Over!");
    }

    // Handle collisions
    void OnCollisionEnter2D(Collision2D collision)
    {
        // For a learning game, you might not want to die on all collisions
        // For example, colliding with correct letters might be rewarded
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
    }
}