using UnityEngine;

public class TrackablePlayer : MonoBehaviour
{
    private const int MATERIAL_OPAQUE = 0;
    private const int MATERIAL_TRANSPARENT = 1;

    [SerializeField] private Color cloakedColor = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent white

    private PlayerModules playerModules;
    private Renderer playerRenderer;
    private Color originalColor;

    void Start()
    {
        playerModules = GetComponent<PlayerModules>();
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
    }

    void Update()
    {
        if (playerModules != null && playerRenderer != null)
        {
            if (playerModules.IsCloaked())
            {
                playerRenderer.material.color = cloakedColor;
                SetMaterialTransparent(playerRenderer.material, true);
            }
            else
            {
                playerRenderer.material.color = originalColor;
                SetMaterialTransparent(playerRenderer.material, false);
            }
        }
    }

    public bool IsCloaked()
    {
        return playerModules != null && playerModules.IsCloaked();
    }

    // from https://discussions.unity.com/t/changing-lightweight-shader-surface-type-in-c/720000/4
    private void SetMaterialTransparent(Material material, bool enabled)
    {
        material.SetFloat("_Surface", enabled ? MATERIAL_TRANSPARENT : MATERIAL_OPAQUE);
        material.SetShaderPassEnabled("SHADOWCASTER", !enabled);
        material.renderQueue = enabled ? 3000 : 2000;
        material.SetFloat("_DstBlend", enabled ? 10 : 0);
        material.SetFloat("_SrcBlend", enabled ? 5 : 1);
        material.SetFloat("_ZWrite", enabled ? 0 : 1);
    }
}
