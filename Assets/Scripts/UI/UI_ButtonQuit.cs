using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UI_ButtonQuit : MonoBehaviour
{
    [SerializeField] private string sfx_click;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }
    void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }
    void OnButtonClicked()
    {
        button.interactable = false;
        GameManager.Instance.EndGame();
        AudioManager.Instance.PlaySoundEffect(sfx_click, 1);
    }
}
