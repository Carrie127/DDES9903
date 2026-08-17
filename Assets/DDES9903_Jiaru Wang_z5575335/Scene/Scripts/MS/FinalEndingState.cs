public static class FinalEndingState
{
    public enum EndingRoute
    {
        None,
        Reject,
        Rewrite
    }

    public static EndingRoute CurrentEnding { get; private set; }
        = EndingRoute.None;

    public static void SetReject()
    {
        CurrentEnding = EndingRoute.Reject;
        UnityEngine.Debug.Log("FINAL ENDING STATE → REJECT");
    }

    public static void SetRewrite()
    {
        CurrentEnding = EndingRoute.Rewrite;
        UnityEngine.Debug.Log("FINAL ENDING STATE → REWRITE");
    }

    public static bool IsReject()
    {
        return CurrentEnding == EndingRoute.Reject;
    }

    public static bool IsRewrite()
    {
        return CurrentEnding == EndingRoute.Rewrite;
    }

    public static void ResetEnding()
    {
        CurrentEnding = EndingRoute.None;
    }
}