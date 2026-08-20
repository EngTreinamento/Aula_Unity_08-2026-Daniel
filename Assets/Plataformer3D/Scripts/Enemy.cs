using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Slider lifeSlider;
    [SerializeField] int life = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lifeSlider.maxValue = life;
        lifeSlider.value = life;
    }

    public void TakeDamage(int damage)
    {
        life -= damage;
        UpdateLife();

        if(life <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateLife()
    {
        lifeSlider.value = life;
    }
}
