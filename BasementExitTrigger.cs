using UnityEngine;

public class BasementExitTrigger : MonoBehaviour
{
    [Header("PLAYER")]
    public Transform player;

    [Header("CLOWN")]
    public GameObject clown;

    // Manually choose exactly where clown spawns
    public Transform spawnPoint;

    [Header("CHASE - BEFORE STAIRS")]
    public float startingSpeed = 0.5f;

    [Header("CHASE - AFTER STAIRS")]
    public float speedAfterStairs = 2.0f;
    public float maximumSpeed = 2.7f;

    [Header("CHASE")]
    public float acceleration = 0.12f;
    public float catchDistance = 0.7f;

    [Header("CAUGHT ENDING")]
    public Transform caughtReturnPoint;

    // Existing final ending system
    public Section1ReturnGate finalEndingGate;

    private bool passedFirstTime = false;
    private bool chaseStarted = false;
    private bool playerDead = false;
    private bool playerEscaped = false;

    private bool playerReachedTop = false;

    private float clownSpeed;

    private StarterAssets.FirstPersonController playerController;

    private void Start()
    {
        // Find player automatically
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        // Get player controller
        if (player != null)
        {
            playerController =
                player.GetComponent<StarterAssets.FirstPersonController>();
        }

        // Hide clown at beginning
        if (clown != null)
            clown.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // =====================================
        // FIRST PASS
        // =====================================

        if (!passedFirstTime)
        {
            passedFirstTime = true;

            Debug.Log("FIRST PASS — NOTHING HAPPENS.");

            return;
        }

        // =====================================
        // SECOND PASS
        // START CHASE
        // =====================================

        if (!chaseStarted && !playerEscaped)
        {
            chaseStarted = true;

            Debug.Log("🔥🔥 CHASE STARTED!");

            StartChase();
        }
    }

    // =========================================
    // START CHASE
    // =========================================

    void StartChase()
    {
        if (player == null || clown == null)
            return;

        playerReachedTop = false;
        playerEscaped = false;
        playerDead = false;

        Vector3 spawnPosition;

        // =====================================
        // MANUAL SPAWN POINT
        // =====================================

        if (spawnPoint != null)
        {
            spawnPosition = spawnPoint.position;

            Debug.Log(
                "👹 CLOWN SPAWN POINT: " +
                spawnPosition
            );
        }
        else
        {
            spawnPosition =
                player.position -
                player.forward * 6f;

            Debug.Log(
                "⚠️ NO SPAWN POINT ASSIGNED — USING BACKUP POSITION!"
            );
        }

        // Put clown at spawn position
        clown.transform.position = spawnPosition;

        // =====================================
        // FACE PLAYER
        // =====================================

        Vector3 direction =
            player.position -
            clown.transform.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            clown.transform.rotation =
                Quaternion.LookRotation(direction);
        }

        // Activate clown
        clown.SetActive(true);

        // Start very slowly
        clownSpeed = startingSpeed;

        // Player keeps normal sprint speed
        if (playerController != null)
        {
            playerController.SprintSpeed = 5f;
        }

        Debug.Log(
            "👹 CLOWN SPAWNED — VERY SLOW START!"
        );
    }

    // =========================================
    // CALLED BY ClownSpeedGate
    // =========================================

    public void PlayerReachedTop()
    {
        if (!chaseStarted)
            return;

        if (playerReachedTop)
            return;

        playerReachedTop = true;

        clownSpeed = speedAfterStairs;

        Debug.Log(
            "🔥 PLAYER REACHED TOP OF STAIRS!"
        );

        Debug.Log(
            "👹 CLOWN SPEED INCREASED TO: " +
            speedAfterStairs
        );
    }

    // =========================================
    // CALLED BY ClownEscapeGate
    // =========================================

    public void PlayerEscaped()
    {
        if (playerDead)
            return;

        if (!chaseStarted)
            return;

        Debug.Log(
            "🏃 PLAYER ESCAPED THE CLOWN!"
        );

        playerEscaped = true;
        chaseStarted = false;

        // Hide clown
        if (clown != null)
        {
            clown.SetActive(false);
        }

        Debug.Log(
            "👹 CLOWN CHASE STOPPED — PLAYER SAFE!"
        );
    }

    // =========================================
    // CHASE
    // =========================================

    private void Update()
    {
        if (!chaseStarted)
            return;

        if (playerDead)
            return;

        if (playerEscaped)
            return;

        if (player == null || clown == null)
            return;

        // Accelerate clown
        clownSpeed +=
            acceleration *
            Time.deltaTime;

        if (playerReachedTop)
        {
            clownSpeed =
                Mathf.Clamp(
                    clownSpeed,
                    speedAfterStairs,
                    maximumSpeed
                );
        }
        else
        {
            clownSpeed =
                Mathf.Clamp(
                    clownSpeed,
                    startingSpeed,
                    speedAfterStairs
                );
        }

        // Find player
        Vector3 direction =
            player.position -
            clown.transform.position;

        float distance =
            direction.magnitude;

        // =====================================
        // PLAYER CAUGHT
        // =====================================

        if (distance <= catchDistance)
        {
            PlayerCaught();
            return;
        }

        // =====================================
        // MOVE TOWARD PLAYER
        // =====================================

        if (direction != Vector3.zero)
        {
            direction.Normalize();

            clown.transform.position +=
                direction *
                clownSpeed *
                Time.deltaTime;

            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            clown.transform.rotation =
                Quaternion.Slerp(
                    clown.transform.rotation,
                    targetRotation,
                    Time.deltaTime * 8f
                );
        }
    }

    // =========================================
    // PLAYER CAUGHT
    // =========================================

    void PlayerCaught()
    {
        playerDead = true;
        chaseStarted = false;

        Debug.Log(
            "💀 CLOWN CAUGHT PLAYER!"
        );

        // Hide basement clown
        if (clown != null)
        {
            clown.SetActive(false);
        }

        // =====================================
        // TELEPORT PLAYER
        // =====================================

        if (caughtReturnPoint != null && player != null)
        {
            CharacterController controller =
                player.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
            }

            player.position =
                caughtReturnPoint.position;

            player.rotation =
                caughtReturnPoint.rotation;

            if (controller != null)
            {
                controller.enabled = true;
            }

            Debug.Log(
                "🏭 PLAYER TELEPORTED TO FINAL ENDING POINT!"
            );
        }
        else
        {
            Debug.LogError(
                "⚠️ CAUGHT RETURN POINT IS NOT ASSIGNED!"
            );
        }

        // =====================================
        // START EXISTING FINAL ENDING
        // =====================================

        if (finalEndingGate != null)
        {
            finalEndingGate.StartFinalEndingFromCaught(player);

            Debug.Log(
                "🎬 FINAL ENDING SEQUENCE STARTED!"
            );
        }
        else
        {
            Debug.LogError(
                "⚠️ FINAL ENDING GATE IS NOT ASSIGNED!"
            );
        }
    }
}