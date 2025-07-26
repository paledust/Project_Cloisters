using UnityEngine;

public class IC_Evocative : IC_Basic
{
    [SerializeField] private Transform restartPos;
    [SerializeField] private BounceBall bounceBall;
    [SerializeField] private Clickable_BallLauncher ballLauncher;
    public void RespawnGame()
    {
        bounceBall.ResetAtPos(restartPos.position);
        ballLauncher.gameObject.SetActive(true);
    }
}
