using UnityEngine;

public class ParticleManager : MonoBehaviour
{
   public static ParticleManager Instance {get; private set;}

   [Header("Particle Prefab")]
   [SerializeField]private ParticleSystem pixelBurstPrefab;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlayEatEffect(Vector3 position,Color color)
    {
        if(pixelBurstPrefab == null) return;

        ParticleSystem ps = Instantiate(pixelBurstPrefab,position,Quaternion.identity);
        var main = ps.main;
        main.startColor = color;

        ps.Play();

        Destroy(ps.gameObject,main.duration + main.startLifetime.constantMax);
    }

}
