using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BurnZone : MonoBehaviour, IDropHandler
{
    public AudioSource burnAudio = null;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerDrag;
        Card card = obj.GetComponent<Card>();
        if (card)
        {
            PlayBurnSound();
            GameController.instance.playersHand.RemoveCard(card);
            GameController.instance.SetNextPlayerTurn();
        }
    }

    internal void PlayBurnSound()
    {
        burnAudio.Play();
    }

}
