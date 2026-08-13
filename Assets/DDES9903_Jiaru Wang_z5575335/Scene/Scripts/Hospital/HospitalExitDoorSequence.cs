using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HospitalExitDoorSequence : MonoBehaviour
{
    [Header("Left Door")]
    public Transform leftDoorTransform;
    public float leftOpenYRotation = -90f;

    [Header("Right Door")]
    public Transform rightDoorTransform;
    public float rightOpenYRotation = 90f;

    [Header("Timing")]
    public float delayBeforeOpen = 0.2f;
    public float openDuration = 1.5f;

    [Header("Door Audio - Optional For Later")]
    public AudioSource doorAudioSource;
    public AudioClip doorOpenClip;

    [Header("After Doors Open")]
    public UnityEvent onDoorsOpened;

    private bool hasOpened = false;

    public void StartDoorOpen()
    {
        if (hasOpened)
            return;

        hasOpened = true;

        StartCoroutine(OpenDoorsSequence());
    }

    private IEnumerator OpenDoorsSequence()
    {
        Debug.Log("HOSPITAL EXIT DOUBLE DOOR SEQUENCE STARTED");

        if (delayBeforeOpen > 0f)
        {
            yield return new WaitForSeconds(delayBeforeOpen);
        }

        if (leftDoorTransform == null ||
            rightDoorTransform == null)
        {
            Debug.LogWarning(
                "HospitalExitDoorSequence: One or both door transforms are missing!"
            );

            yield break;
        }

        // ---------------------------------------------
        // Optional door-opening sound
        // ---------------------------------------------

        if (doorAudioSource != null &&
            doorOpenClip != null)
        {
            doorAudioSource.clip = doorOpenClip;
            doorAudioSource.Play();
        }

        // ---------------------------------------------
        // Remember current closed rotations
        // ---------------------------------------------

        Quaternion leftStartRotation =
            leftDoorTransform.localRotation;

        Quaternion rightStartRotation =
            rightDoorTransform.localRotation;

        // ---------------------------------------------
        // Build target rotations
        // ---------------------------------------------

        Vector3 leftTargetEuler =
            leftDoorTransform.localEulerAngles;

        leftTargetEuler.y =
            leftOpenYRotation;

        Quaternion leftTargetRotation =
            Quaternion.Euler(leftTargetEuler);

        Vector3 rightTargetEuler =
            rightDoorTransform.localEulerAngles;

        rightTargetEuler.y =
            rightOpenYRotation;

        Quaternion rightTargetRotation =
            Quaternion.Euler(rightTargetEuler);

        // ---------------------------------------------
        // Open both doors together
        // ---------------------------------------------

        float timer = 0f;

        while (timer < openDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / openDuration
                );

            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );

            leftDoorTransform.localRotation =
                Quaternion.Slerp(
                    leftStartRotation,
                    leftTargetRotation,
                    smoothT
                );

            rightDoorTransform.localRotation =
                Quaternion.Slerp(
                    rightStartRotation,
                    rightTargetRotation,
                    smoothT
                );

            yield return null;
        }

        // ---------------------------------------------
        // Ensure final rotations
        // ---------------------------------------------

        leftDoorTransform.localRotation =
            leftTargetRotation;

        rightDoorTransform.localRotation =
            rightTargetRotation;

        Debug.Log("HOSPITAL EXIT DOUBLE DOORS OPENED");

        onDoorsOpened?.Invoke();
    }
}