using UnityEngine;

public class Collectibles : MonoBehaviour
{
    [SerializeField] CollectibleData collectible;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<MeshRenderer>().material.color = collectible.color;
        Debug.Log("Total de Pontos que esse coletável vai dar: " +  collectible.score);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
