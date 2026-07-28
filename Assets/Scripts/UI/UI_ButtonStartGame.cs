using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UI_ButtonStartGame : MonoBehaviour
{
    [SerializeField] private float transitionTime = 2.5f;
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
        LevelProgressionManager.Instance.ResetProgress();
        GameManager.Instance.SwitchingScene("Game", transitionTime, 1);
    }
}