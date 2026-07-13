using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UI_ButtonQuit : MonoBehaviour
{
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
    }
}
