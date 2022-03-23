using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    public Player targetPlayer = null;
    public Card sourceCard = null;
    public Image effectImage = null;

    public AudioSource iceAudio = null;
    public AudioSource fireAudio = null;
    public AudioSource destructAudio = null;

    public void EndTrigger()
    {
        bool bounce = false;
        if (targetPlayer.HasMirror())
        {
            bounce = true;
            targetPlayer.SetMirror(false);
            targetPlayer.PlaySmashSound();
            if (targetPlayer.isPlayer)
                GameController.instance.CastAttackEffect(sourceCard, GameController.instance.enemy);
            else
                GameController.instance.CastAttackEffect(sourceCard, GameController.instance.player);
        }
        else
        {
            int damage = sourceCard.cardData.damage;
            if (!targetPlayer.isPlayer) // targe player is enemy
            {
                if (sourceCard.cardData.damageType == CardData.DamageType.fire && targetPlayer.isFire)
                    damage = damage / 2;
                if (sourceCard.cardData.damageType == CardData.DamageType.ice && !targetPlayer.isFire)
                    damage = damage / 2;
            }
            targetPlayer.health -= damage;
            targetPlayer.PlayHitAnim();
            GameController.instance.UpdateHealth();
            if (!bounce)
            {
                GameController.instance.SetNextPlayerTurn();
            }
            if(targetPlayer.health <= 0)
            {
                targetPlayer.health = 0;
                if (targetPlayer.isPlayer)
                    GameController.instance.PlayPlayerDieSound();
                else
                    GameController.instance.PlayEnemyDieSound();
            }
            GameController.instance.isPlayable = true;
        }
        Destroy(gameObject);
    }

    internal void PlayIceSound()
    {
        iceAudio.Play();
    }

    internal void PlayFireSound()
    {
        fireAudio.Play();
    }
    internal void PlayDestructSound()
    {
        destructAudio.Play();
    }
}
