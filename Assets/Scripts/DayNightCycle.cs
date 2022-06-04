using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)] public float time;

    public float fullDayLenght;

    public float startTime = 0.4f;

    public Vector3 noon;

    [Header("Sun")] public Light sun;

    public Gradient sunColor;
    public AnimationCurve sunIntencity;

    [Header("Moon")] public Light moon;

    public Gradient moonColor;
    public AnimationCurve moonIntencity;

    [Header("Other Lightning")] public AnimationCurve lightningIntensityMultiplier;
    public AnimationCurve reflectionsIntensityMultiplier;

    private float timeRate;

    private void Start()
    {
        timeRate = 1.0f / fullDayLenght;
        time = startTime;
    }

    private void Update()
    {
        //increment time

        time += timeRate * Time.deltaTime;
        if (time >= 1.0f)
        {
            time = 0.0f;
        }

        //light rotation

        sun.transform.eulerAngles = (time - 0.25f) * noon * 4.0f;
        moon.transform.eulerAngles = (time - 0.75f) * noon * 4.0f;

        //light intensity

        sun.intensity = sunIntencity.Evaluate(time);
        moon.intensity = moonIntencity.Evaluate(time);

        //change colors

        sun.color = sunColor.Evaluate(time);
        moon.color = moonColor.Evaluate(time);

        //enable / disable sun

        if (sun.intensity == 0 && sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(false);
        }
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(true);
        }

        //enable / disable moon

        if (moon.intensity == 0 && moon.gameObject.activeInHierarchy)
        {
            moon.gameObject.SetActive(false);
        }
        else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
        {
            moon.gameObject.SetActive(true);
        }

        //lighting and reflection intensity

        RenderSettings.ambientIntensity = lightningIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionsIntensityMultiplier.Evaluate(time);
    }
}