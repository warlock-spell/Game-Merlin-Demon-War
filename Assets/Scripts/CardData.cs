using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "card", menuName = "CardGame/Card")]
public class CardData : ScriptableObject
{
    public enum DamageType
    {
        fire,
        ice,
        both, 
        destruct
    }
    public string cardTitle,
                  description;
    public int cost,
               damage;
    public DamageType damageType;
    public Sprite cardImage,
                  frameImage;
    public int numberInDeck;
    public bool isDefenceCard = false,
                isMirrorCard = false,
                isMulti = false,
                isDestruct = false;

}
