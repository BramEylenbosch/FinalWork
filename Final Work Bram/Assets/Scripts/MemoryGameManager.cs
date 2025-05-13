using System.Collections.Generic;
using UnityEngine;

public class MemoryGameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardGrid;
    public Sprite[] cardImages; // 8 afbeeldingen voor 4x4 grid
    private List<MemoryCard> activeCards = new();
    private MemoryCard firstCard, secondCard;

    void Start()
    {
        CreateCards();
    }

    void CreateCards()
    {
        List<int> ids = new();
        for (int i = 0; i < cardImages.Length; i++)
        {
            ids.Add(i);
            ids.Add(i); // dubbel voor paren
        }

        Shuffle(ids);

        foreach (int id in ids)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGrid);
            MemoryCard card = newCard.GetComponent<MemoryCard>();
            card.Setup(cardImages[id], id);
        }
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }

    public void CardSelected(MemoryCard card)
    {
        if (firstCard == null)
        {
            firstCard = card;
            card.FlipCard();
        }
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;
            card.FlipCard();
            StartCoroutine(CheckMatch());
        }
    }

    System.Collections.IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);
        if (firstCard.cardId == secondCard.cardId)
        {
            // Match — kaart blijft open
        }
        else
        {
            firstCard.FlipBack();
            secondCard.FlipBack();
        }
        firstCard = null;
        secondCard = null;
    }
}
