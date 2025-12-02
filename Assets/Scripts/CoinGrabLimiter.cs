using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class CoinGrabLimiter : MonoBehaviour
{
    [Tooltip("Максимальная скорость после отпускания")]
    public float maxReleaseSpeed = 12f;

    [Tooltip("Максимальная угловая скорость после отпускания")]
    public float maxReleaseAngular = 15f;

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    Rigidbody rb;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        if (grab != null) grab.selectExited.AddListener(OnSelectExited);
    }

    void OnDisable()
    {
        if (grab != null) grab.selectExited.RemoveListener(OnSelectExited);
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        if (rb == null) return;

        if (rb.linearVelocity.magnitude > maxReleaseSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxReleaseSpeed;

        if (rb.angularVelocity.magnitude > maxReleaseAngular)
            rb.angularVelocity = rb.angularVelocity.normalized * maxReleaseAngular;
    }
}
