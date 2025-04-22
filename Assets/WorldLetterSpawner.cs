using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldLetterSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject letterPrefab;     // Prefab with TextMeshPro component (3D version)
    public float spawnRate = 3f;        // Time between spawns
    public float moveSpeed = 3f;        // How fast letters move
    public float heightVariation = 3f;  // Random height variation

    [Header("Letter Settings")]
    public string testLetter = "A";     // Start with single test letter
    public float letterScale = 1f;      // Scale of the letter (adjust as needed)
    public Color letterColor = Color.black;

    [Header("Debug Settings")]
    public float spawnXOffset = 10f;    // How far right of camera to spawn
    public bool debugMode = true;       // Shows spawn position gizmos

    private float timer = 0;
    private string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                                  "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

    void Update()
    {
        // Simple timer for spawning
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            SpawnLetter();
            timer = 0;
        }
    }

    void SpawnLetter()
    {
        // Create new letter
        GameObject newLetter = Instantiate(letterPrefab);

        // Get the camera's right edge in world space and add offset
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, Camera.main.nearClipPlane));
        float spawnX = rightEdge.x + spawnXOffset;

        // Randomize position height around camera's center
        float cameraY = Camera.main.transform.position.y;
        float yPos = cameraY + Random.Range(-heightVariation, heightVariation);

        // Set spawn position
        newLetter.transform.position = new Vector3(spawnX, yPos, 0);

        // Debug log
        Debug.Log($"Spawning letter at: {newLetter.transform.position}");

        // Set the letter text and appearance
        TextMeshPro letterText = newLetter.GetComponentInChildren<TextMeshPro>();
        if (letterText != null)
        {
            // Pick a random letter instead of using testLetter
            letterText.text = alphabet[Random.Range(0, alphabet.Length)];
            letterText.color = letterColor;

            // Find and adjust collider to match text
            BoxCollider2D collider = newLetter.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                // Make collider match the text bounds approximately
                collider.size = new Vector2(letterText.preferredWidth / 10f, letterText.preferredHeight / 10f);
                collider.offset = Vector2.zero; // Centered
                collider.isTrigger = true; // Make it a trigger for collision detection
            }
        }

        // Set scale
        newLetter.transform.localScale = new Vector3(letterScale, letterScale, letterScale);

        // Add movement component
        LetterMover mover = newLetter.AddComponent<LetterMover>();
        mover.moveSpeed = moveSpeed;
    }

    // Debug visualization
    void OnDrawGizmos()
    {
        if (debugMode && Camera.main != null)
        {
            Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, Camera.main.nearClipPlane));
            Vector3 spawnPos = new Vector3(rightEdge.x + spawnXOffset, Camera.main.transform.position.y, 0);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnPos, 0.5f);
            Gizmos.DrawLine(rightEdge, spawnPos);
        }
    }
}

// Updated LetterMover with collision detection
public class LetterMover : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float despawnX;

    void Start()
    {
        // Set despawn position relative to camera's left edge
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, Camera.main.nearClipPlane));
        despawnX = leftEdge.x - 5f;
    }

    void Update()
    {
        // Move letter from right to left
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // Destroy when off-screen
        if (transform.position.x < despawnX)
        {
            Destroy(gameObject);
        }
    }

    // Collision detection with player
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Letter collided with player!");
            Destroy(gameObject);
        }
    }
}