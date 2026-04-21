using UnityEngine;
using UnityEngine.UI;

public class UI_Intro : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster raycaster;
    public void DisableAllCanvas()
    {
        raycaster.enabled = false;
    }
}
