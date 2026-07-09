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
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool isMenuOpen = false;
    [SerializeField] private float menuTime = .3f;
    [SerializeField] private Volume menuVolume;
    [SerializeField] private CanvasGroup groupSetting;

[Header("BlackBar")]
    [SerializeField] private Image blackBar_Top;
    [SerializeField] private Image blackBar_Bottom;
    [SerializeField] private AudioData_SO sfx_click;

[Header("UI Sound Muffle")]
    [SerializeField] private Vector2 CutOffRange = new Vector2(700f, 22000.00f);
    public bool IsMenuOpen => isMenuOpen;
    private Sequence pauseTween;

    public void Start()
    {
        isMenuOpen = false;
        UpdateMenuImmediately();
    }
    void Awake(){
        menuAction["menu"].performed += MenuAction_performed;
        menuAction.Enable();
    }
    void OnDestroy(){
        menuAction["menu"].performed -= MenuAction_performed;
        menuAction.Disable();
    }
    public void EnableCanvas()
    {
        isMenuOpen = true;
        blackBar_Top.rectTransform.anchoredPosition = new Vector2(0, 100);
        blackBar_Bottom.rectTransform.anchoredPosition = new Vector2(0, -100);

        pauseTween.Kill();
        pauseTween = DOTween.Sequence();
        pauseTween.Join(blackBar_Top.rectTransform.DOAnchorPosY(0, menuTime).SetEase(Ease.OutQuad))
                    .Join(blackBar_Bottom.rectTransform.DOAnchorPosY(0, menuTime).SetEase(Ease.OutQuad))
                    .Join(DOTween.To(() => menuVolume.weight, x => menuVolume.weight = x, 1, menuTime).SetEase(Ease.OutQuad))
                    .Join(DOTween.To(() => AudioManager.Instance.GetCutOff(), x => AudioManager.Instance.ChangeCutOff(x), CutOffRange.x, menuTime))
                    .Join(canvasGroup.DOFade(1, menuTime).OnComplete(()=>canvasGroup.interactable = true))
                    .SetUpdate(true);

        GameManager.Instance.PauseTheGame();
    }
    public void DisableCanvas()
    {
        GameManager.Instance.ResumeTheGame();

        isMenuOpen = false;
        canvasGroup.interactable = false;

        pauseTween.Kill();
        pauseTween = DOTween.Sequence();
        pauseTween.Join(blackBar_Top.rectTransform.DOAnchorPosY(100, menuTime).SetEase(Ease.OutQuad))
                    .Join(blackBar_Bottom.rectTransform.DOAnchorPosY(-100, menuTime).SetEase(Ease.OutQuad))
                    .Join(DOTween.To(() => menuVolume.weight, x => menuVolume.weight = x, 0, menuTime).SetEase(Ease.OutQuad))
                    .Join(DOTween.To(() => AudioManager.Instance.GetCutOff(), x => AudioManager.Instance.ChangeCutOff(x), CutOffRange.y, menuTime))
                    .Join(canvasGroup.DOFade(0, menuTime))
                    .SetUpdate(true);
    }
    public void Btn_Settings()
    {
        groupSetting.gameObject.SetActive(true);
        groupSetting.interactable = true;
        groupSetting.blocksRaycasts = true;
        groupSetting.DOFade(1, 0.15f).SetUpdate(true);
    }
    public void Btn_RestartGame()
    {
        raycaster.enabled = false;
        menuAction.Disable();
        gameControl.RestartLevel();
        AudioManager.Instance.PlaySFX(sfx_click.AudioKey, 1);
    }
    public void Btn_BackToMainMenu()
    {
        raycaster.enabled = false;
        menuAction.Disable();
        gameControl.GoBackToMainMenu();
        AudioManager.Instance.PlaySFX(sfx_click.AudioKey, 1);
    }
    public void Btn_QuitGame()
    {
        raycaster.enabled = false;
        menuAction.Disable();
        GameManager.Instance.EndGame();
        AudioManager.Instance.PlaySFX(sfx_click.AudioKey, 1);
    }
    void UpdateMenuImmediately()
    {
        blackBar_Top.rectTransform.anchoredPosition = new Vector2(0, isMenuOpen?0:100);
        blackBar_Bottom.rectTransform.anchoredPosition = new Vector2(0, isMenuOpen?0:-100);
        canvasGroup.alpha = isMenuOpen?1:0;
        canvasGroup.interactable = isMenuOpen;
    }
    void MenuAction_performed(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton())
        {
            EventHandler.Call_OnFlushInput();
            if(!isMenuOpen)
                EnableCanvas();
            else
                DisableCanvas();
            interactionBlocker.SetActive(isMenuOpen);
        }
    }
#if UNITY_EDITOR
    [ContextMenu("Switch Menu State")]
    public void SwitchMenuState()
    {
        isMenuOpen = !isMenuOpen;
        UpdateMenuImmediately();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}