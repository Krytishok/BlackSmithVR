using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class CoinGrabLimiter : MonoBehaviour
{
    [Tooltip("Максимальная скорость после отпускания")]
    public float maxReleaseSpeed = 3f;

    [Tooltip("Максимальная угловая скорость после отпускания")]
    public float maxReleaseAngular = 10f;

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    Rigidbody rb;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnSelectEntered);
            grab.selectExited.AddListener(OnSelectExited);
        }
    }

    void OnDisable()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnSelectEntered);
            grab.selectExited.RemoveListener(OnSelectExited);
        }
    }

    // Срабатывает, когда начинаешь хватать монету
    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (rb == null) return;

        // Если тело кинематическое, скорости всё равно не используются, и сеттеры вызовут ошибку
        if (!rb.isKinematic)
        {
            // Убираем всю текущую инерцию, чтобы монету не "выстреливало" в момент захвата
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // Срабатывает, когда отпускаешь монету
    void OnSelectExited(SelectExitEventArgs args)
    {
        if (rb == null) return;

        if (rb.isKinematic)
            return; // для кинематического тела нет смысла ограничивать скорости

        if (rb.linearVelocity.magnitude > maxReleaseSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxReleaseSpeed;

        if (rb.angularVelocity.magnitude > maxReleaseAngular)
            rb.angularVelocity = rb.angularVelocity.normalized * maxReleaseAngular;
    }
}
