using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!GameController.instance.isPlayable) return;
        transform.position += (Vector3)eventData.delta;
        Card card = GetComponent<Card>();
        bool overCard = false;
        foreach (GameObject gameObject in eventData.hovered)
        {
            Player playerCard = gameObject.GetComponent<Player>();
            if (playerCard)
            {
                if (GameController.instance.IsCardValid(card, playerCard, GameController.instance.playersHand))
                {
                    playerCard.glowImage.gameObject.SetActive(true);
                    overCard = true;
                }
            }

            BurnZone burnZone = gameObject.GetComponent<BurnZone>();
            if (burnZone)
            {
                card.burnImage.gameObject.SetActive(true);
            }
            else
            {
                card.burnImage.gameObject.SetActive(false);
            }
        }
        if (!overCard)
        {
            GameController.instance.player.glowImage.gameObject.SetActive(false);
            GameController.instance.enemy.glowImage.gameObject.SetActive(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = originalPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
