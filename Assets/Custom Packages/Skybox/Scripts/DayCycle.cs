using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class DayCycle : MonoBehaviour
{
    [Header("Time")]
    public float speed = 0.017f;
    [Range(0, 96)] public int speedMultiplier = 1;

    [Range(0, 24)] public float dayTime;

    Vector3 trueAngle = new Vector3(0, -90, 0);
    Quaternion trueRot;

    [Header("Lighting")]
    public Light sun;
    public Gradient lightColor;
    public Gradient lightIntensity;

    [Header("Fog")]
    public bool UseFog;
    public Gradient fogColor;
    public Gradient fogDensity;
    [Range(0f, 500f)] public float fogDistance;

    private void Awake()
    {
        sun = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            dayTime += speed * speedMultiplier * Time.deltaTime;
            if (dayTime >= 24)
            {
                dayTime = 0;
            }
        }

        LightingUpdate();
    }

    private void LightingUpdate()
    {
        trueAngle.x = 360 * (dayTime / 24);

        trueRot = Quaternion.Euler(trueAngle);

        transform.eulerAngles = trueRot.eulerAngles;

        RenderSettings.fog = UseFog;
        RenderSettings.fogColor = fogColor.Evaluate(dayTime / 24);
        RenderSettings.fogDensity = fogDensity.Evaluate(dayTime / 24).r;

        sun.color = lightColor.Evaluate(dayTime / 24);
        sun.intensity = lightIntensity.Evaluate(dayTime / 24).r * 10;
    }
}
