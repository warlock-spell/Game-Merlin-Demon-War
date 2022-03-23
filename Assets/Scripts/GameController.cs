using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    static public GameController instance = null;

    public Deck playerDeck = new Deck();
    public Deck enemyDeck = new Deck();

    public List<CardData> cards = new List<CardData>();

    public Sprite[] healthNumbers = new Sprite[10];
    public Sprite[] damageNumbers = new Sprite[10];

    public Hand playersHand = new Hand();
    public Hand enemysHand = new Hand();

    public GameObject cardPefab = null;
    public Canvas canvas = null;

    public bool isPlayable = false;

    public Player player = null;
    public Player enemy = null;

    public GameObject effectLeftPrefab = null;
    public GameObject effectRightPrefab = null;
    public Sprite fireball = null;
    public Sprite iceball = null;
    public Sprite multiFireball = null;
    public Sprite multiIceball = null;
    public Sprite fireAndIce = null;
    public Sprite destruct = null;

    public bool playersTurn = true;

    public Text turnText = null;

    public Image enemySkipTurn = null;

    public Sprite fireDemon = null;
    public Sprite iceDemon = null;

    public Text scoreText = null;

    public int score = 0, kills = 0;

    public AudioSource playerDieAudio = null;
    public AudioSource enemyDieAudio = null;

    private void Awake()
    {
        instance = this;
        SetUpEnemy();
        playerDeck.Create();
        enemyDeck.Create();
        StartCoroutine(DealHands());
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void SkipTurn()
    {
        if(playersTurn && isPlayable)
            SetNextPlayerTurn();
    }

    internal IEnumerator DealHands()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 3; i++)
        {
            playerDeck.DealCard(playersHand);
            enemyDeck.DealCard(enemysHand);
            yield return new WaitForSeconds(1);
        }
        isPlayable = true;
    }

    //IN : playerOn - the player on which the card is
    //     hand - hand from which hand the card is
    internal bool UseCard(Card card, Player playerOn, Hand hand)
    {
        if (!IsCardValid(card, playerOn, hand)) return false;
        isPlayable = false;
        CastCard(card, playerOn, hand);
        player.glowImage.gameObject.SetActive(false);
        enemy.glowImage.gameObject.SetActive(false);
        hand.RemoveCard(card);
        return false;
    }

    internal bool IsCardValid(Card playedCard, Player playerOn, Hand hand)
    {
        bool valid = false;
        if (playedCard == null) return false;
        if (hand.isPlayers)
        {
            if (playedCard.cardData.cost <= player.mana)
            {
                if (playerOn.isPlayer && playedCard.cardData.isDefenceCard)
                    valid = true; 
                if (!playerOn.isPlayer && !playedCard.cardData.isDefenceCard)
                    valid = true;
            }
        }
        else
        {
            if (playedCard.cardData.cost <= enemy.mana)
            {
                if (!playerOn.isPlayer && playedCard.cardData.isDefenceCard)
                    valid = true;
                if (playerOn.isPlayer && !playedCard.cardData.isDefenceCard)
                    valid = true;
            }
        }
        return valid;
    }

    internal void CastCard(Card card, Player playerOn, Hand hand)
    {
        if (card.cardData.isMirrorCard)
        {
            playerOn.SetMirror(true);
            playerOn.PlayMirrorSound();
            SetNextPlayerTurn();
            isPlayable = true;
        }
        else
        {
            if (card.cardData.isDefenceCard) // health card
            {
                playerOn.health += card.cardData.damage;
                playerOn.PlayHealSound();
                if (playerOn.health > playerOn.maxHealth)
                    playerOn.health = playerOn.maxHealth;
                UpdateHealth();
                StartCoroutine(CastHealEffect(player));
            }
            else // attack
            {
                CastAttackEffect(card, playerOn);
            }
            if (hand.isPlayers)
                score += card.cardData.damage;
            UpdateScore();
        }
        if (hand.isPlayers)
        {
            player.mana -= card.cardData.cost;
            player.UpdateMana();
        }
        else
        {
            enemy.mana -= card.cardData.cost;
            enemy.UpdateMana();
        }
    }

    private IEnumerator CastHealEffect(Player playerOn)
    {
        yield return new WaitForSeconds(0.5f);
        SetNextPlayerTurn();
        isPlayable = true;
    }

    internal void CastAttackEffect(Card card, Player playerOn)
    {
        GameObject effectGO = null;
        if (playerOn.isPlayer)
            effectGO = Instantiate(effectRightPrefab, canvas.gameObject.transform);
        else
            effectGO = Instantiate(effectLeftPrefab, canvas.gameObject.transform);
        Effect effect = effectGO.GetComponent<Effect>();
        if (effect)
        {
            effect.targetPlayer = playerOn;
            effect.sourceCard = card;
            switch (card.cardData.damageType)
            {
                case CardData.DamageType.fire:
                    if (card.cardData.isMulti)
                        effect.effectImage.sprite = multiFireball;
                    else
                        effect.effectImage.sprite = fireball;
                    effect.PlayFireSound();
                    break;
                case CardData.DamageType.ice:
                    if (card.cardData.isMulti)
                        effect.effectImage.sprite = multiIceball;
                    else
                        effect.effectImage.sprite = iceball;
                    effect.PlayIceSound();
                    break;
                case CardData.DamageType.destruct:
                    effect.effectImage.sprite = destruct;
                    effect.PlayDestructSound();
                    break;
                case CardData.DamageType.both:
                    effect.effectImage.sprite = fireAndIce;
                    effect.PlayIceSound();
                    effect.PlayFireSound();
                    break;
            }
        }
    }

    internal void UpdateHealth()
    {
        player.UpdateHealth();
        enemy.UpdateHealth();

        if(player.health <= 0)
        {
            StartCoroutine(GameOver());
        }
        if(enemy.health <= 0)
        {
            kills++;
            score += 100;
            UpdateScore();
            StartCoroutine(NewEnemy());
        }
    }

    private IEnumerator NewEnemy()
    {
        enemy.gameObject.SetActive(false);
        enemysHand.ClearHand();
        yield return new WaitForSeconds(0.75f);
        SetUpEnemy();
        enemy.gameObject.SetActive(true);
        StartCoroutine(DealHands());
    }

    private void SetUpEnemy()
    {
        enemy.mana = 0;
        enemy.health = 5;
        enemy.UpdateHealth();
        enemy.isFire = true;
        if (UnityEngine.Random.Range(0, 2) == 1)
            enemy.isFire = false;
        if (enemy.isFire)
            enemy.playerImage.sprite = fireDemon;
        else
            enemy.playerImage.sprite = iceDemon;
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(2);
    }

    internal void SetNextPlayerTurn()
    {
        playersTurn = !playersTurn;
        bool enemyIsDead = false;
        if (playersTurn)
        {
            if(player.mana < 5)
                player.mana++;
        }
        else
        {
            if (enemy.health > 0)
            {
                if (enemy.mana < 5)
                    enemy.mana++;
            }
            else
                enemyIsDead = true;
        }
        if (enemyIsDead)
        { 
            playersTurn = !playersTurn;
            if (player.mana < 5)
                player.mana++;
        }
        else
        {
            SetTurnText();
            if (!playersTurn)
                MonstersTurn();
        }

        player.UpdateMana();
        enemy.UpdateMana();
    }

    private void SetTurnText()
    {
        if (playersTurn)
            turnText.text = "Merlin's turn";
        else
            turnText.text = "Enemy's turn";
    }

    private void MonstersTurn()
    {
        Card card = AIChooseCard();
        StartCoroutine(CastEnemyCard(card));

    }

    private Card AIChooseCard()
    {
        List<Card> available = new List<Card>();
        for (int i = 0; i < 3; i++)
        {
            if (IsCardValid(enemysHand.cards[i], enemy, enemysHand))
                available.Add(enemysHand.cards[i]);
            else if (IsCardValid(enemysHand.cards[i], player, enemysHand))
                available.Add(enemysHand.cards[i]);
        }
        if(available.Count == 0)
        {
            SetNextPlayerTurn();
            return null;
        }
        int choice = UnityEngine.Random.Range(0, available.Count);
        return available[choice];
    }

    private IEnumerator CastEnemyCard(Card card)
    {
        yield return new WaitForSeconds(0.5f);
        if (card)
        {
            TurnCard(card);
            yield return new WaitForSeconds(2);
            if (card.cardData.isDefenceCard)
            {
                UseCard(card, enemy, enemysHand);
            }
            else
            {
                UseCard(card, player, enemysHand);
            }
            yield return new WaitForSeconds(1);
            enemyDeck.DealCard(enemysHand);
            yield return new WaitForSeconds(1);

        }
        else
        {
            enemySkipTurn.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            enemySkipTurn.gameObject.SetActive(false);
        }
    }

    internal void TurnCard(Card card)
    {
        Animator animator = card.GetComponentInChildren<Animator>();
        if (animator)
        {
            animator.SetTrigger("Flip");
        }
        else
        {
            Debug.LogError("No animator found.");
        }
    }

    private void UpdateScore()
    {
        scoreText.text = "Demons killed: " + kills.ToString() + "  Score: " + score.ToString();
    }

    internal void PlayPlayerDieSound()
    {
        playerDieAudio.Play();
    }

    internal void PlayEnemyDieSound()
    {
        enemyDieAudio.Play();
    }
}
