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
        movingDir = Vector3.ClampMagnitude(movingDir, 8);
        transform.DOScale(Vector3.one*Random.Range(0.5f, 1f), 1f).SetEase(Ease.OutBack);
        transform.DOMove(transform.position + movingDir*Random.Range(0.25f, 0.5f), 1.5f).SetEase(Ease.OutQuad);
        anime.Play();
        Destroy(gameObject,2f);
    }
    public void AssignSprite(Sprite txtSprite)=>txtRender.sprite = txtSprite;
}
