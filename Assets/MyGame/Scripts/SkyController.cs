using UnityEngine;

public class SkyController : MonoBehaviour
{
    public Material dayMaterial;
    public Material nightMaterial;
    public float rotationSpeed = 0.5f;
    public GameObject dayLight;
    public GameObject nightLight;

    public Color dayFogColor;
    public Color nightFogColor;

    
    void Update()
    {
        float fRotateValue = RenderSettings.skybox.GetFloat("_Rotation");
        fRotateValue += rotationSpeed * Time.deltaTime;
        if (fRotateValue > 360f)
        {
            fRotateValue -= 360f;
        }
        RenderSettings.skybox.SetFloat("_Rotation", fRotateValue);

    }
    void OnGUI()
    {
        if(GUI.Button(new Rect(5,5,80,20), "Day")){
            RenderSettings.skybox = dayMaterial;
            RenderSettings.fogColor = dayFogColor;
            RenderSettings.fog = true;
            dayLight.SetActive(true);
            nightLight.SetActive(false);
        }
        if(GUI.Button(new Rect(5,35,80,20), "Night")){
            RenderSettings.skybox = nightMaterial;
            RenderSettings.fogColor = nightFogColor;
            RenderSettings.fog = true;
            dayLight.SetActive(false);
            nightLight.SetActive(true);
        }
    }
}
