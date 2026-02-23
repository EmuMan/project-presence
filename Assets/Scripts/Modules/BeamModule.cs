using UnityEngine;

public class BeamModule : Module
{
    [SerializeField] private DamagingHitbox beamMeshAndHitbox;

    protected override void StartPerformingAction(Vector3 _direction)
    {
        beamMeshAndHitbox.gameObject.SetActive(true);
        beamMeshAndHitbox.EnableHitbox();
    }

    protected override void StopPerformingAction(Vector3 _direction)
    {
        beamMeshAndHitbox.DisableHitbox();
        beamMeshAndHitbox.gameObject.SetActive(false);
    }
}
