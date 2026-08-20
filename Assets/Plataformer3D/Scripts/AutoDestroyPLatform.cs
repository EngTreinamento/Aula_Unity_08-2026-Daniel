using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AutoDestroyPLatform : MonoBehaviour
{
    [SerializeField] float lifeTime = 1f;

    [SerializeField] bool usarCorrotina = false;
    [SerializeField] bool usarDestroy = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(usarDestroy)
                Destroy(gameObject, lifeTime);

            if (usarCorrotina)
                StartCoroutine(DestroyPlatform());
        }
    }

    private IEnumerator DestroyPlatform()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
}
