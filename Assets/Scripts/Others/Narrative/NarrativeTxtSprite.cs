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

        movingDir = Quaternion.Euler(0,0,Random.Range(-30,30)) * movingDir;
        movingDir = Vector3.ClampMagnitude(movingDir, 4);
        Vector3 finalPos = transform.position + movingDir;
        finalPos.x = Mathf.Clamp(finalPos.x, -13, 13);
        finalPos.y = Mathf.Clamp(finalPos.y, -7, 7);
        
        float scale = transform.localScale.x * 2.2f;
        scale = Mathf.Max(scale, 1);
        transform.DOScale(scale, .25f).SetEase(Ease.OutBack);
        transform.DOMove(finalPos, Random.Range(2f, 2.2f)).SetEase(Ease.OutCubic);
        anime.Play();
        Destroy(gameObject,2f);
    }
    public void AssignSprite(Sprite txtSprite)=>txtRender.sprite = txtSprite;
}
