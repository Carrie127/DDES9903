using UnityEngine;

public class ClockInteraction : MonoBehaviour
{
    private bool activated = false;

    public void ActivateClock()
    {
        if (activated)
            return;

        activated = true;

        Debug.Log("CLOCK 7:42 ACTIVATED!");
    }
}