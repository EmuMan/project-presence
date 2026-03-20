using UnityEngine;
using System.Collections;

public class EnemyTrigger : MonoBehaviour
{
    [Header("Enemies to activate")]
    public Enemy[] enemies;

    [Header("Audio")]
    [SerializeField] private AudioSource combatAudio;
    [SerializeField] private float audioFadeOutDelay = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
                enemy.Activate();
        }

        PlayAudio();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
                enemy.Deactivate();
        }

        FadeOutAudio();
    }

    private void PlayAudio()
    {
        if (combatAudio != null && !combatAudio.isPlaying)
        {
            combatAudio.Play();
        }
    }

    private void FadeOutAudio()
    {
        if (combatAudio != null && combatAudio.isPlaying)
        {
            StartCoroutine(FadeOutCoroutine());
        }
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = combatAudio.volume;

        while (combatAudio.volume > 0)
        {
            combatAudio.volume -= startVolume * Time.deltaTime / audioFadeOutDelay;
            yield return null;
        }

        combatAudio.Stop();
        combatAudio.volume = startVolume;
    }
}
