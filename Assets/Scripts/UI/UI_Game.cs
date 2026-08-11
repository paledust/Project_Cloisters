using DG.Tweening;
using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{
[Header("Menu Activation")]
    [SerializeField] private GameObject interactionBlocker;
    [SerializeField] private InputActionMap menuAction;

[Header("Menu Control")]
    [SerializeField] private Game gameControl;
    [SerializeField] private CanvasGroup canvasMenu;
    [SerializeField] private CanvasGroup canvasGame;
    
    [SerializeField] private bool isMenuOpen = false;
    [SerializeField] private float menuTime = .3f;
    [SerializeField] private Volume menuVolume;
    [SerializeField] private UI_Setting groupSetting;

[Header("BlackBar")]
    [SerializeField] private Image blackBar_Top;
    [SerializeField] private Image blackBar_Bottom;

[Header("UI Sound Muffle")]
    [SerializeField] private Vector2 CutOffRange = new Vector2(700f, 22000.00f);

    private Sequence pauseTween;

    public void Start()
    {
        isMenuOpen = false;
        canvasMenu.alpha = 0;
        canvasGame.alpha = 1;
        interactionBlocker.SetActive(false);
        SwitchCanvas(canvasMenu, false);
        SwitchCanvas(canvasGame, true);
        blackBar_Top.rectTransform.anchoredPosition = new Vector2(0, isMenuOpen?0:100);
        blackBar_Bottom.rectTransform.anchoredPosition = new Vector2(0, isMenuOpen?0:-100);
    }
    void Awake(){
        EventHandler.E_OnTransitionBegin += TransitionBeginHandler;
        EventHandler.E_OnTransitionEnd += TransitionEndHandler;
        menuAction["menu"].performed += MenuAction_performed;
        menuAction.Enable();
    }
    void OnDestroy(){
        EventHandler.E_OnTransitionBegin -= TransitionBeginHandler;
        EventHandler.E_OnTransitionEnd -= TransitionEndHandler;
        menuAction["menu"].performed -= MenuAction_performed;
        menuAction.Disable();
    }
    public void EnableCanvas()
    {
        isMenuOpen = true;
        blackBar_Top.rectTransform.anchoredPosition = new Vector2(0, 100);
        blackBar_Bottom.rectTransform.anchoredPosition = new Vector2(0, -100);

        SwitchCanvas(canvasGame, false);
        pauseTween.Kill();
        pauseTween = DOTween.Sequence();
        pauseTween.Join(blackBar_Top.rectTransform.DOAnchorPosY(0, menuTime).SetEase(Ease.OutQuad))
                    .Join(blackBar_Bottom.rectTransform.DOAnchorPosY(0, menuTime).SetEase(Ease.OutQuad))
                    .Join(DOTween.To(() => menuVolume.weight, x => menuVolume.weight = x, 1, menuTime).SetEase(Ease.OutQuad))
                    .Join(DOTween.To(() => AudioManager.Instance.GetCutOff(), x => AudioManager.Instance.ChangeCutOff(x), CutOffRange.x, menuTime))
                    .Join(canvasMenu.DOFade(1, menuTime).OnComplete(()=>SwitchCanvas(canvasMenu, true)))
                    .Join(canvasGame.DOFade(0, menuTime))
                    .SetUpdate(true);

        GameManager.Instance.PauseTheGame();
    }
    public void DisableCanvas()
    {
        GameManager.Instance.ResumeTheGame();
        isMenuOpen = false;

        SwitchCanvas(canvasMenu, false);
        pauseTween.Kill();
        pauseTween = DOTween.Sequence();
        pauseTween.Join(blackBar_Top.rectTransform.DOAnchorPosY(100, menuTime).SetEase(Ease.OutQuad))
                    .Join(blackBar_Bottom.rectTransform.DOAnchorPosY(-100, menuTime).SetEase(Ease.OutQuad))
                    .Join(DOTween.To(() => menuVolume.weight, x => menuVolume.weight = x, 0, menuTime).SetEase(Ease.OutQuad))
                    .Join(DOTween.To(() => AudioManager.Instance.GetCutOff(), x => AudioManager.Instance.ChangeCutOff(x), CutOffRange.y, menuTime))
                    .Join(canvasMenu.DOFade(0, menuTime))
                    .Join(canvasGame.DOFade(1, menuTime).OnComplete(()=>SwitchCanvas(canvasGame, true)))
                    .SetUpdate(true);
    }
    public void Btn_Settings()
    {
        groupSetting.SwitchSetting(true);
    }
    public void Btn_RestartGame()
    {
        DisableCanvas();
        menuAction.Disable();
        gameControl.RestartLevel();
    }
    public void Btn_BackToMainMenu()
    {
        DisableCanvas();
        menuAction.Disable();
        gameControl.GoBackToMainMenu();
    }
    public void Btn_QuitGame()
    {
        DisableCanvas();
        menuAction.Disable();
        GameManager.Instance.EndGame();
    }
    public void Btn_QuitMenu()
    {
        EventHandler.Call_OnFlushInput();
        DisableCanvas();
        interactionBlocker.SetActive(isMenuOpen);
    }
    public void Btn_OpenMenu()
    {
        EventHandler.Call_OnFlushInput();
        EnableCanvas();
        interactionBlocker.SetActive(isMenuOpen);
    }
    void MenuAction_performed(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton())
        {
            if(groupSetting.isSettingOpen)
                groupSetting.SwitchSetting(false);
            else
            {
                EventHandler.Call_OnFlushInput();
                if(!isMenuOpen)
                    EnableCanvas();
                else
                    DisableCanvas();
                interactionBlocker.SetActive(isMenuOpen);
            }
        }
    }
    void TransitionBeginHandler()
    {
        menuAction.Disable();
    }
    void TransitionEndHandler()
    {
        menuAction.Enable();
    }
    void SwitchCanvas(CanvasGroup canvas, bool isOn)
    {
        canvas.interactable = isOn;
        canvas.blocksRaycasts = isOn;
    }
}