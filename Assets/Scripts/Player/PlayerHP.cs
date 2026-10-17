using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBaseHP : MonoBehaviour, IHealth
{
    [SerializeField] float maxHP = 20f;
    [SerializeField] HealthUI hpUI;
    public float CurrentHP { get; private set; }
    public float MaxHP => maxHP;

    void Awake()
    {
        CurrentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        CurrentHP = Mathf.Max(CurrentHP - damage, 0f);
        
        hpUI.UpdateHP();
        hpUI.UpdateGhostHP();

        CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();

        if (cameraShake != null)
            cameraShake.Shake();

        SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.baseHit, 0.8f);    

        if(CurrentHP <= 0)
        {
            GameManager.Instance.LoseGame();  
            
        }
    }
}