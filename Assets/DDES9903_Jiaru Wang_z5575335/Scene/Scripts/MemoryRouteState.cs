public static class MemoryRouteState
{
    public enum MemoryRoute
    {
        None,
        OrphanageFirst,
        HospitalFirst
    }

    public static MemoryRoute CurrentRoute { get; private set; }
        = MemoryRoute.None;

    public static void SetOrphanageFirst()
    {
        CurrentRoute = MemoryRoute.OrphanageFirst;
        UnityEngine.Debug.Log("ROUTE SET: Orphanage First");
    }

    public static void SetHospitalFirst()
    {
        CurrentRoute = MemoryRoute.HospitalFirst;
        UnityEngine.Debug.Log("ROUTE SET: Hospital First");
    }

    public static bool IsOrphanageFirst()
    {
        return CurrentRoute == MemoryRoute.OrphanageFirst;
    }

    public static bool IsHospitalFirst()
    {
        return CurrentRoute == MemoryRoute.HospitalFirst;
    }

    public static void ResetRoute()
    {
        CurrentRoute = MemoryRoute.None;
        UnityEngine.Debug.Log("ROUTE RESET");
    }
}