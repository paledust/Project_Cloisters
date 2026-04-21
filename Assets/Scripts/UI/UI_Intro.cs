using UnityEngine;
using UnityEngine.UI;

public class UI_Intro : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster raycaster;
    public void Btn_DisableAllCanvas()
    {
        raycaster.enabled = false;
    }
}
