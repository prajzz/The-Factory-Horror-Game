using UnityEngine;

public class EscapeRouteTrigger : MonoBehaviour
{
    public GameObject backDoor;
    public GameObject outsideLight;

    private int passCount = 0;
    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("EscapeRouteTrigger: Player entered.");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        passCount++;

        Debug.Log("EscapeRouteTrigger: Player exited. Pass = " + passCount);

        // First exit
        if (passCount == 1)
        {
            if (outsideLight != null)
            {
                outsideLight.SetActive(true);
            }

            Debug.Log("Escape light turned ON.");
        }

        // Second exit
        if (passCount == 2 && !activated)
        {
            activated = true;

            if (backDoor != null)
            {
                backDoor.SetActive(false);
            }

            Debug.Log("BACK DOOR DISAPPEARED. PLAYER CAN ESCAPE!");
        }
    }
}