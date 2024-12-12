using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BaseScript : MonoBehaviour
{
    private int _health = 300; 
    private int _maxHealth = 300; 

    public Transform LifeSlider;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            GameManager.instance.enemiesRemaining--;
            Destroy(other.gameObject);
            StartCoroutine(TakeDamageEnemy());
        }
        else
        {
            return;
        }
    }

    public IEnumerator TakeDamageEnemy()
    {
        int damage = 30;
        if (GameManager.instance.Score >= damage)
        {
            GameManager.instance.Score -= damage;
        }
        else
        {
            GameManager.instance.Score = 0; 
        }
        LifeSlider.GetComponent<Slider>().value -= 30;
        yield return null;

        if(GameManager.instance.Score < 0)
        {
            GameManager.instance.Score = 0;
        }
    }

    public void Dead()
    {

        if (LifeSlider.GetComponent<Slider>().value <= 0)
        {
            Debug.Log("loadScene");
        }
    }

    private void Update()
    {
        Dead();
    }
}
