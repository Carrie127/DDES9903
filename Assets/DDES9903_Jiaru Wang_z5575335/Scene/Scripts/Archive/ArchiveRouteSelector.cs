using UnityEngine;

public class ArchiveRouteSelector : MonoBehaviour
{
    public void SelectOrphanageFirst()
    {
        MemoryRouteState.SetOrphanageFirst();

        Debug.Log(
            "Archive choice: Familiar → Orphanage First"
        );
    }

    public void SelectHospitalFirst()
    {
        MemoryRouteState.SetHospitalFirst();

        Debug.Log(
            "Archive choice: Unfamiliar → Hospital First"
        );
    }
}