using UnityEngine;
using TMPro; // Required for TextMeshPro

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 5f;
    public float maxJumpHeight = 8f; // Maximum Y position the player can jump to, no cheating

    // Default position is weird so I respawn them a bit higher??? on restart to kind of match the position from the initial round, can't be bothered to fix this properly
    public Vector3 defaultPlayerPosition = new Vector3(0, 5.66f, 0);

    private Rigidbody playerRb;
    public bool isGameOver = false;  // Check if player failed
    private bool isGamePaused = true; // Game state paused???

    // Score Variables
    public int score; // Variable to store the player's score
    public TextMeshProUGUI scoreText; // Reference to the score text UI element

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        LockRotation();
        // Initially pause the game
        PauseGame();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Check keyboard input for jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProcessJump();
        }

        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch

            if (touch.phase == TouchPhase.Began) // Detect when the touch begins
            {
                ProcessJump();
            }
        }
    }

    void ProcessJump()
    {
        if (isGameOver)
        {
            RestartGame();
        }
        else if (isGamePaused)
        {
            StartGame();
        }
        else
        {
            // Allow jumping if the game is not over and not paused
            if (transform.position.y < maxJumpHeight)
            {
                playerRb.velocity = Vector3.up * jumpForce;
            }
        }
    }

    public void IncrementScore()
    {
        score += 1; // Increment by 1 for each obstacle passed
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        // Update the Score text
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void LockPosition()
    {
        // Lock the player's X and Z because only allowed to move up and down
        float fixedX = transform.position.x;
        float fixedZ = transform.position.z;
        transform.position = new Vector3(fixedX, transform.position.y, fixedZ);
    }

    void LockRotation()
    {
        // Lock rotation on all axes to player model from rolling and other shenanigans
        playerRb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void StopPlayerMovement()
    {
        // Stop player movement and ignore gravity
        playerRb.velocity = Vector3.zero;
        playerRb.useGravity = false;

        // Lock the player’s position
        LockPosition();
    }

    void StartGame()
    {
        score = 0;
        UpdateScoreText();
        isGamePaused = false; // Unpause the game
        playerRb.useGravity = true; // Enable gravity
        Debug.Log("Game Started!");
        // Remove existing obstacles just to be sure
        MoveObstacle[] obstacles = FindObjectsOfType<MoveObstacle>();
        foreach (MoveObstacle obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.StartSpawning(); // Start obstacle spawning
        }
    }

    void RestartGame()
    {
        // Reset the game state
        isGameOver = false;
        isGamePaused = false;
        score = 0;
        UpdateScoreText();

        // Reset player position and state
        transform.position = defaultPlayerPosition; // Move player to default position, as said before it's a bit weird
        playerRb.velocity = Vector3.zero;
        playerRb.useGravity = true;

        // Ensure rotation constraints are applied
        LockRotation();

        // Remove existing obstacles
        MoveObstacle[] obstacles = FindObjectsOfType<MoveObstacle>();
        foreach (MoveObstacle obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }

        // Restart spawning and obstacle movement
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.StopSpawning(); // Ensure no new obstacles are spawning before we start spawning new ones
            spawnManager.StartSpawning(); // Restart obstacle spawning
        }

        Debug.Log("Game Restarted!");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Ground"))
        {
            GameOver();
        }
    }

    void GameOver()
    {
        // Set the game over flag
        isGameOver = true;

        // Stop all obstacles from moving
        MoveObstacle[] obstacles = FindObjectsOfType<MoveObstacle>();
        foreach (MoveObstacle obstacle in obstacles)
        {
            obstacle.StopObstacle();
        }

        // Stop spawning new obstacles
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.StopSpawning();
        }

        StopPlayerMovement();

        // Game over message for debug
        Debug.Log("Game Over! The game has been stopped. Press space to restart.");
    }

    void PauseGame()
    {
        isGamePaused = true;
        StopPlayerMovement();
        // Stop spawning and moving new obstacles
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.StopSpawning();
        }
        MoveObstacle[] obstacles = FindObjectsOfType<MoveObstacle>();
        foreach (MoveObstacle obstacle in obstacles)
        {
            obstacle.StopObstacle();
        }

        // Debug game hint
        Debug.Log("Press Space to Start.");
    }
}
