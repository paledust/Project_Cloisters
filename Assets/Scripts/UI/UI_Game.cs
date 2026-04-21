using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{
    [SerializeField] private IC_Manager interactionManager;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private string sfx_click;
    public void Btn_RestartGame()
    {
        raycaster.enabled = false;
        interactionManager.RestartLevel();
        AudioManager.Instance.PlaySoundEffect(sfx_click, 1);
    }
    public void Btn_BackToMainMenu()
    {
        raycaster.enabled = false;
        interactionManager.GoBackToMainMenu();
        AudioManager.Instance.PlaySoundEffect(sfx_click, 1);
    }
    public void Btn_QuitGame()
    {
        raycaster.enabled = false;
        GameManager.Instance.EndGame();
        AudioManager.Instance.PlaySoundEffect(sfx_click, 1);
    }
}
