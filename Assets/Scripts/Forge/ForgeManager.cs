using UnityEngine;
using System.Collections;

public class ForgeManager : MonoBehaviour
{
    [SerializeField] private ThermometerNeedle thermometerNeedle;
    [SerializeField] private float updateInterval = 0.2f;

    private BlankTrigger currentBlank;
    private Coroutine updateRoutine;

    public void BlankEnteredForge(BlankTrigger blank)
    {
        currentBlank = blank;

        thermometerNeedle.gameObject.SetActive(true);

        if (updateRoutine != null)
            StopCoroutine(updateRoutine);

        updateRoutine = StartCoroutine(UpdateTherometer());
    }

    public void BlankLeftForge(BlankTrigger blank)
    {
        if (blank != currentBlank) return;

        if (updateRoutine != null)
            StopCoroutine(updateRoutine);

        updateRoutine = null;
        currentBlank = null;

        thermometerNeedle.SetHeat(0f);
        thermometerNeedle.gameObject.SetActive(false);
    }

    private IEnumerator UpdateTherometer()
    {
        while (currentBlank != null)
        {
            thermometerNeedle.SetHeat(currentBlank.GetHeatLevel());
            yield return new WaitForSeconds(updateInterval);
        }
    }
}
