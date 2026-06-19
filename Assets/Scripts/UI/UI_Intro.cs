using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#endif

public class UI_Intro : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private CanvasGroup groupCredit;

    void Awake()
    {
        groupCredit.gameObject.SetActive(false);
        groupCredit.alpha = 0;
        groupCredit.interactable = false;
    }
    public void DisableAllCanvas()
    {
        raycaster.enabled = false;
    }
    public void Btn_Credit()
    {
        groupCredit.gameObject.SetActive(true);
        groupCredit.interactable = true;
        groupCredit.DOFade(1, 0.15f);
    }
#if UNITY_EDITOR
    [Button("Switch Credit Preview")]
    public void SwitchCreditPreview()
    {
        bool lastState = groupCredit.gameObject.activeSelf;

        groupCredit.gameObject.SetActive(!lastState);
        groupCredit.interactable = !lastState;
        groupCredit.alpha = !lastState?1:0;

        var activeScene = SceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(activeScene);
    }
#endif
}