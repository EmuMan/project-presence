using UnityEngine;

public class BeamModule : Module
{
    [Header("Beam Settings")]
    [SerializeField] private DamagingHitbox beamMeshAndHitbox;
    [SerializeField] private ParticleSystem beamParticles;

    protected override void StartPerformingAction(Vector3 _direction)
    {
        beamMeshAndHitbox.gameObject.SetActive(true);
        beamMeshAndHitbox.EnableHitbox();
        if (beamParticles != null)
        {
            var emission = beamParticles.emission;
            emission.enabled = true;
        }
    }

    protected override void StopPerformingAction(Vector3 _direction)
    {
        beamMeshAndHitbox.DisableHitbox();
        beamMeshAndHitbox.gameObject.SetActive(false);
        if (beamParticles != null)
        {
            var emission = beamParticles.emission;
            emission.enabled = false;
        }
    }
}
