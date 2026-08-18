using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectibleData", menuName = "Daniel/Coletavel")]
public class CollectibleData : ScriptableObject
{
    public CollectibleType CollectibleType;

    public int valueCollectible;

    public Material material;
}

[Serializable]
public enum CollectibleType { Score, Ammo, Heal }