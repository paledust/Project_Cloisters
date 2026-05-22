using DG.Tweening;
using SimpleAudioSystem;
using UnityEngine;

public class RotateBar : MonoBehaviour
{
    [SerializeField] private float rotateSpeedMulti = 10;
    [SerializeField] private Transform rotateTarget;

    [Header("Audio")]
    [SerializeField] private string sfxFlip;

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out BounceBall ball))
        {
            AudioManager.Instance.PlaySFX(sfxFlip, 1);
            float speed = ball.m_currentSpeed * rotateSpeedMulti;
            float angle = rotateTarget.localEulerAngles.z + 360*5;
            rotateTarget.DOKill();
            rotateTarget.DORotate(new Vector3(0, 0, angle), 360*5/speed, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
        }
    }
}
