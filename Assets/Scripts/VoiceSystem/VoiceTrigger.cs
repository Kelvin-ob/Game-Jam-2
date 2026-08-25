using UnityEngine;

public class VoiceTrigger : MonoBehaviour
{
    [TextArea(2, 5)]
    [SerializeField] private string voiceText;

    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private VoiceTrigger requiredTrigger;


    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggerOnce && triggered)
            return;

        if (requiredTrigger == null)
        {
            triggered = true;

            VoiceManager.Instance.ShowVoice(voiceText);

            return;
        }

        if (!requiredTrigger.triggered)
            return;

        triggered = true;

        VoiceManager.Instance.ShowVoice(voiceText);
    }
}