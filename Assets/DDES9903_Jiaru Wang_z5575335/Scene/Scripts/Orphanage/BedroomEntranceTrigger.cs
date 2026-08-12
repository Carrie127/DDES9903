using UnityEngine;

public class BedroomEntranceTrigger : MonoBehaviour
{
    [Header("Bedroom Guide")]
    public BedroomGuideLight bedroomGuideLight;

    [Header("Teddy Memory Stage")]
    public TeddyMemoryAudio teddyMemoryAudio;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        Debug.Log("PLAYER ENTERED BEDROOM");

        // -----------------------------------------
        // 1. Entrance guide light fades away
        // -----------------------------------------

        if (bedroomGuideLight != null)
        {
            bedroomGuideLight.FadeOutGuideLight();
        }

        // -----------------------------------------
        // 2. Start Teddy memory stage
        // -----------------------------------------

        if (teddyMemoryAudio != null)
        {
            teddyMemoryAudio.StartTeddyStage();
        }
    }
}