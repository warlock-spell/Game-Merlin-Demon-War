using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    public List<CardData> cardDatas = new List<CardData>();

    public void Create()
    {
        // create all the cards from a deck
        List<CardData> allCardDatas = new List<CardData>();
        foreach(CardData cardData in GameController.instance.cards)
        {
            for(int i=0; i<cardData.numberInDeck; i++)
            {
                allCardDatas.Add(cardData);
            }
        }

        // randomize the created cards
        while (allCardDatas.Count > 0)
        {
            int rand = UnityEngine.Random.Range(0, allCardDatas.Count);
            cardDatas.Add(allCardDatas[rand]);
            allCardDatas.RemoveAt(rand);
        }
    }

    private CardData GetCardFromDeck()
    {
        CardData result = null;
        if (cardDatas.Count == 0)
            Create();
        result = cardDatas[0];
        cardDatas.RemoveAt(0);
        return result;
    }

    private Card CreateNewCard(Vector3 position, string animation)
    {
        GameObject newCard = GameObject.Instantiate(GameController.instance.cardPefab,
                                                    GameController.instance.canvas.gameObject.transform);
        newCard.transform.position = position;
        Card card = newCard.GetComponent<Card>();
        if (card)
        {
            card.cardData = GetCardFromDeck();
            card.Initialise();
            Animator animator = newCard.GetComponentInChildren<Animator>();
            if (animator)
            {
                animator.CrossFade(animation, 0);
            }
            else
            {
                Debug.LogError("No animator found.");
            }

            return card;
        }
        else
        {
            Debug.LogError("No card component found.");
            return null;
        }
    }

    internal void DealCard(Hand hand)
    {
        for(int i = 0; i < 3; i++)
        {
            if(hand.cards[i] == null)
            {
                if (hand.isPlayers)
                    GameController.instance.player.PlayDealSound();
                else
                    GameController.instance.enemy.PlayDealSound();
                hand.cards[i] = CreateNewCard(hand.positions[i].position, hand.animations[i]);
                return;
            }
        }
    }
}
