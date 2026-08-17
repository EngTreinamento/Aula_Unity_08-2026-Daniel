using UnityEngine;

[CreateAssetMenu(fileName = "Arma", menuName = "Daniel/Arma")]
public class WeaponData : ScriptableObject
{
    public int damage = 10;
    public int cadence = 1;
    public float range = 10;
    public int ammoCapacity = 30;

    public Material weaponMaterial;
}

