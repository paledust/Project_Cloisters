using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleParticleController : MonoBehaviour
{
    [System.Serializable]
    public struct ParticlePlay{
        public ParticleSystem particle;
        public Rect playRect;
        public Vector2Int playAmountRange;
        public Vector2 cycleRange;
        private float playTime;
        private float nextCycle;
        public void UpdateParticlePlay(float time){
            if(time - playTime > nextCycle){
                nextCycle = cycleRange.GetRndValueInVector2Range();
                playTime = time;

                int count = playAmountRange.GetRndValueInVector2Range();
                for(int i=0; i<count; i++){
                    particle.transform.localPosition = GetPointWithinRect();
                    particle.Play();
                }
            }
        }
        Vector3 GetPointWithinRect(){
            return new Vector3(playRect.x + Random.Range(-playRect.width*0.5f, playRect.width*0.5f), playRect.y + Random.Range(-playRect.height*0.5f, playRect.height*0.5f), 0);
        }
    }
    [SerializeField] private ParticlePlay[] particles;
    void Update()
    {
        for(int i=0; i<particles.Length; i++){
            particles[i].UpdateParticlePlay(Time.time);
        }
    }
}
