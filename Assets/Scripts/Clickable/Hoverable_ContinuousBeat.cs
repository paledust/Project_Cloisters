using SimpleAudioSystem;
using UnityEngine;

public class Hoverable_ContinuousBeat : MonoBehaviour
{
    [SerializeField] private string hoverLoopingSFX;
    private bool hovering = false; 
    private AudioSource audioSource;
    private Clickable_Stylized clickableStylized;

    void Awake()
    {
        clickableStylized = GetComponent<Clickable_Stylized>();
        audioSource = GetComponent<AudioSource>();
    }
    void OnEnable()
    {
        clickableStylized.onHover += OnHover;
        clickableStylized.onExitHover += OnExitHover;
    }
    void OnDisable()
    {
        clickableStylized.onHover -= OnHover;
        clickableStylized.onExitHover -= OnExitHover;
    }
    void Update()
    {
        if(hovering)
        {
            
        }
    }
    void OnHover(PlayerController player)
    {
        hovering = true;
        StylizedDrumController.Instance.QueueContinuousBeat(hoverLoopingSFX, 1f, audioSource);
    }
    void OnExitHover()
    {
        hovering = false;
    }
}
