using System.Collections;
using UnityEngine;

public class ShapeColorChanger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flickerDuration = 0.8f;
    [SerializeField] private int flickerTime = 4;
    [SerializeField] private Color[] colorSheet;

    private CoroutineExcuter colorChanger;
    private int colorIndex = 0;

    void Awake()
    {
        colorChanger = new CoroutineExcuter(this);
    }
    public void BlinkColor()
    {
        colorChanger.Excute(coroutineChangeColor(flickerDuration+Random.Range(-0.2f,0.2f), flickerTime));
    }
    IEnumerator coroutineChangeColor(float duration, int _flickerTime)
    {
        Color originalCol = spriteRenderer.color;
        Color color = originalCol;
        int currentIndex = colorIndex;
        yield return new WaitForLoop(duration, (t)=>
        {
            colorIndex = currentIndex+Mathf.CeilToInt(t*_flickerTime);
            colorIndex = colorIndex % colorSheet.Length;
            spriteRenderer.color = colorSheet[colorIndex];
        });
        colorIndex ++;
        colorIndex = colorIndex % colorSheet.Length;
        spriteRenderer.color = colorSheet[colorIndex];
    }
}