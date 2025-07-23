using UnityEngine;

public class Clickable_BallLauncher : Basic_Clickable
{
    [SerializeField] private BounceBall bounceBall;
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        bounceBall.Bounce(Random.insideUnitCircle, 10, AttributeModifyType.Add);
        gameObject.SetActive(false);
    }
}
