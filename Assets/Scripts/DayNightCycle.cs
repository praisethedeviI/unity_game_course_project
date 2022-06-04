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

    private float timeRate;
}