using UnityEngine;

public class ControlPanel : MonoBehaviour
{
    public TelephoneController telephone;
    public Transform player;
    public AudioSource switchSound;

    public MissionProgressController missionController;

    public float interactionDistance = 3f;

    private bool activated = false;

    private void Start()
    {
        // Automatically find the player
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError(
                    "ControlPanel: Player with Tag 'Player' not found!"
                );
            }
        }

        // Automatically find the telephone
        if (telephone == null)
        {
            telephone =
                FindFirstObjectByType<TelephoneController>();

            if (telephone != null)
            {
                Debug.Log(
                    "ControlPanel: Telephone found automatically!"
                );
            }
            else
            {
                Debug.LogError(
                    "ControlPanel: TelephoneController not found in scene!"
                );
            }
        }

        // Automatically find the mission system
        if (missionController == null)
        {
            missionController =
                FindFirstObjectByType<MissionProgressController>();

            if (missionController != null)
            {
                Debug.Log(
                    "ControlPanel: Mission system found automatically!"
                );
            }
            else
            {
                Debug.LogError(
                    "ControlPanel: MissionProgressController not found in scene!"
                );
            }
        }
    }

    private void Update()
    {
        // Switch already activated
        if (activated)
            return;

        // Required references missing
        if (player == null || telephone == null)
            return;

        // Check distance from player to switch
        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        // Player must be close enough
        if (distance <= interactionDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Activate ONCE
                activated = true;

                // Play switch sound
                if (switchSound != null)
                {
                    switchSound.Play();
                }

                Debug.Log("CONTROL PANEL ACTIVATED!");

                // Start telephone ringing
                telephone.StartRinging();

                // Change mission
                if (missionController != null)
                {
                    missionController.ShowMission2();
                }

                Debug.Log("PHONE SHOULD NOW BE RINGING!");
                Debug.Log("MISSION CHANGED TO: ANSWER THE TELEPHONE");
            }
        }
    }
}