using UnityEngine;
using UnityEngine.UI;

public class ModuleStatus : MonoBehaviour
{
    [SerializeField] private Image resourceImage;
    [SerializeField] private Image cooldownRingImage;
    [SerializeField] private Image disabledImage;

    public void UpdateStatus(float resource, float cooldownRemaining, bool active)
    {
        UpdateResource(resource);
        UpdateCooldown(cooldownRemaining);
        SetActive(active);
    }

    public void UpdateResource(float resource)
    {
        resourceImage.fillAmount = resource;
    }

    public void UpdateCooldown(float cooldownRemaining)
    {
        cooldownRingImage.fillAmount = cooldownRemaining;
    }

    public void SetActive(bool active)
    {
        disabledImage.gameObject.SetActive(!active);
        // Fade other images if not active
        float alpha = active ? 1f : 0.5f;
        SetImageAlpha(resourceImage, alpha);
        SetImageAlpha(cooldownRingImage, alpha);
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
