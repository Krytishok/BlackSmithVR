using UnityEngine;

public class ThermometerBinder : MonoBehaviour
{
    [SerializeField] private ThermometerNeedle needle;
    [SerializeField] private BlankTrigger blank;

    private void OnEnable()
    {
        blank.OnHeatChanged += needle.SetHeat;
    }

    private void OnDisable()
    {
        blank.OnHeatChanged -= needle.SetHeat;
    }
}
