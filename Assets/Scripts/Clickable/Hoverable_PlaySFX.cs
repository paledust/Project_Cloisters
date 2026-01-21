using UnityEngine;

public class Hoverable_PlaySFX : MonoBehaviour
{
    [SerializeField] private string hoverSFX;
    [SerializeField] private Vector2 volumeRange = new Vector2(0, 1);
    [SerializeField] private float speedToVolume = 1f; 
    private Basic_Clickable self;
    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Basic_Clickable>();
        self.onHover += PlayHoverSFX;
    }
    void OnDestroy()
    {
        self.onHover -= PlayHoverSFX;
    }
    void PlayHoverSFX(PlayerController player)
    {
        StylizedDrumController.Instance.QueueBeat(hoverSFX, Mathf.Clamp(player.PointerDelta.magnitude * speedToVolume, volumeRange.x, volumeRange.y));
    }
}
