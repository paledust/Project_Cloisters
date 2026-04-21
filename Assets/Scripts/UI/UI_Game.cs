using DG.Tweening;
using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{
    [SerializeField] private IC_Manager interactionManager;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool isMenuOpen = false;
    [SerializeField] private float menuTime = .3f;
    [SerializeField] private Volume menuVolume;

[Header("BlackBar")]
    [SerializeField] private Image blackBar_Top;
    [SerializeField] private Image blackBar_Bottom;
    [SerializeField] private string sfx_click;

    public bool IsMenuOpen => isMenuOpen;

    public void Start()
    {
        isMenuOpen = false;
        UpdateMenuImmediately();
    }
    public void EnableCanvas()
    {
        isMenuOpen = true;
        canvasGroup.DOKill();
        canvasGroup.DOFade(1, menuTime).OnComplete(()=>canvasGroup.interactable = true);

        blackBar_Top.DOKill();
        blackBar_Bottom.DOKill();
        blackBar_Top.rectTransform.anchoredPosition = new Vector2(0, 100);
        blackBar_Bottom.rectTransform.anchoredPosition = new Vector2(0, -100);
        blackBar_Top.rectTransform.DOAnchorPosY(0, menuTime).SetEase(Ease.OutQuad);
        blackBar_Bottom.rectTransform.DOAnchorPosY(0, menuTime).SetEase(Ease.OutQuad);

        menuVolume.DOKill();
        DOTween.To(() => menuVolume.weight, x => menuVolume.weight = x, 1, menuTime).SetEase(Ease.OutQuad);
    }
    public void DisableCanvas()
    {
        isMenuOpen = false;
        canvasGroup.interactable = false;
        canvasGroup.DOKill();
        canvasGroup.DOFade(0, menuTime);

        blackBar_Top.DOKill();
        blackBar_Bottom.DOKill();
        blackBar_Top.rectTransform.DOAnchorPosY(100, menuTime).SetEase(Ease.OutQuad);
        blackBar_Bottom.rectTransform.DOAnchorPosY(-100, menuTime).SetEase(Ease.OutQuad);

        menuVolume.DOKill();
        DOTween.To(() => menuVolume.weight, x => menuVolume.weight = x, 0, menuTime).SetEase(Ease.OutQuad);
    }
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
    void UpdateMenuImmediately()
    {
        blackBar_Top.rectTransform.anchoredPosition = new Vector2(0, isMenuOpen?0:100);
        blackBar_Bottom.rectTransform.anchoredPosition = new Vector2(0, isMenuOpen?0:-100);
        canvasGroup.alpha = isMenuOpen?1:0;
        canvasGroup.interactable = isMenuOpen;
    }

#if UNITY_EDITOR
    [ContextMenu("Switch Menu State")]
    public void SwitchMenuState()
    {
        isMenuOpen = !isMenuOpen;
        UpdateMenuImmediately();
    }
#endif
}