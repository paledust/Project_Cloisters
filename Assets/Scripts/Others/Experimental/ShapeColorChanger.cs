using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ShapeColorChanger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PerRendererExpShape perRendererExpShape;
    [SerializeField] private float flickerDuration = 0.8f;
    [SerializeField] private int flickerTime = 4;

    private CoroutineExcuter colorChanger;

    void Awake()
    {
        colorChanger = new CoroutineExcuter(this);
    }
    public void BlinkColor(Color blinkColor1, Color blinkColor2)
    {
        colorChanger.Excute(coroutineChangeColor(flickerDuration+Random.Range(-0.2f,0.2f), flickerTime, blinkColor1, blinkColor2 ));
    }
    public void ChangeToActivateColor()
    {
        spriteRenderer.transform.DOPunchScale(0.05f * Vector3.one, 0.2f);
        DOTween.To(()=>perRendererExpShape.emissive, x => perRendererExpShape.emissive = x, 3, 0.2f);
    }
    public void ChangeToInActivateColor()
    {
        DOTween.To(()=>perRendererExpShape.emissive, x => perRendererExpShape.emissive = x, 2f, 0.2f);
    }
    IEnumerator coroutineChangeColor(float duration, int _flickerTime, Color blinkColor1, Color blinkColor2)
    {
        Color originalCol = spriteRenderer.color;
        Color color = originalCol;
        yield return new WaitForLoop(duration, (t)=>
        {
            int index = Mathf.CeilToInt(t*_flickerTime);
            spriteRenderer.color = index%2==0?blinkColor1:blinkColor2;
        });
        spriteRenderer.color = originalCol;
    }
}