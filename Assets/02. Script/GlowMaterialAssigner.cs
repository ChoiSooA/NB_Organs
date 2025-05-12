using UnityEngine;

public class GlowMaterialAssigner : MonoBehaviour
{
    public Material glowMaterial;

    void Start()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
        {
            renderer.material = glowMaterial;
        }
    }
}
