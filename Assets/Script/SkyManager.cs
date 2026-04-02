using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    private const string LastSkyIndexKey = "SkyManager.LastSkyIndex";

    [Header("Skybox Materials (size = 5)")]
    [SerializeField] private List<Material> skyMaterials = new List<Material>(5);
    [SerializeField] private bool avoidRepeatLast = true;

    public void ApplyRandomSky()
    {
        if (skyMaterials == null || skyMaterials.Count == 0)
            return;

        int count = skyMaterials.Count;
        int lastIndex = PlayerPrefs.GetInt(LastSkyIndexKey, -1);

        int index = Random.Range(0, count);
        if (avoidRepeatLast && count > 1 && index == lastIndex)
            index = (index + 1 + Random.Range(0, count - 1)) % count;

        ApplySkyByIndex(index);
    }

    public void ApplySkyByIndex(int index)
    {
        if (skyMaterials == null || skyMaterials.Count == 0)
            return;

        index = Mathf.Clamp(index, 0, skyMaterials.Count - 1);
        Material mat = skyMaterials[index];
        if (mat == null)
            return;

        RenderSettings.skybox = mat;
        PlayerPrefs.SetInt(LastSkyIndexKey, index);

        // Refresh environment lighting/reflections if used.
        DynamicGI.UpdateEnvironment();
    }
}

