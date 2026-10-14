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

        if(CurrentHP <= 0)
        {
            Debug.Log("Scene Restart");
            SceneManager.LoadScene("Gameplay");    
        }
    }
}