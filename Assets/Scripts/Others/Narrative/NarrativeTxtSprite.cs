using DG.Tweening;
using UnityEngine;

public class NarrativeTxtSprite : MonoBehaviour
{
    [SerializeField] private FloatingMotion floatingMotion;
    [SerializeField] private AngleMotion angleMotion;
    [SerializeField] private Animation anime;
    [SerializeField] private SpriteRenderer txtRender;

    public void OnShowingText(Vector3 movingDir)
    {
        Destroy(floatingMotion);
        Destroy(angleMotion);
        if(movingDir == Vector3.zero) 
            movingDir = Vector3.up;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -14, 14);
        pos.y = Mathf.Clamp(pos.y, -7, 7);
        transform.position = pos;
        
        movingDir = Quaternion.Euler(0,0,Random.Range(-30,30)) * movingDir;
        movingDir = Vector3.ClampMagnitude(movingDir, 4);
        Vector3 finalPos = pos + movingDir;
        finalPos.x = Mathf.Clamp(finalPos.x, -11.5f, 11.5f);
        finalPos.y = Mathf.Clamp(finalPos.y, -4.5f, 4.5f);
        
        float scale = transform.localScale.x * 2.2f;
        scale = Mathf.Max(scale, 1);
        transform.DOScale(scale, .25f).SetEase(Ease.OutBack);
        transform.DOMove(finalPos, Random.Range(2f, 2.2f)).SetEase(Ease.OutCubic);
        anime.Play();
        Destroy(gameObject,2f);
    }
    public void AssignSprite(Sprite txtSprite)=>txtRender.sprite = txtSprite;
}
