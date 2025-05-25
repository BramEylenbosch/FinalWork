using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MemoryGameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardGrid;
    public Sprite[] cardImages; // 8 afbeeldingen voor 4x4 grid
    private List<MemoryCard> activeCards = new();
    private MemoryCard firstCard, secondCard;
    private int totalPairs;
    private int pairsFound = 0;
    public GameObject winPanel; // Sleep je WinPanel hier naartoe in de Inspector
    public List<Sprite> personalPhotos = new List<Sprite>();
    private bool usePersonalPhotos = true; // Zorg dat je dit op true zet als je deze modus wilt
    public GameObject photoSetupPanel;
    public Transform photoPreviewGrid;
    public GameObject photoThumbnailPrefab;
void Start()
{
    string sceneName = SceneManager.GetActiveScene().name;

    if (sceneName == "MemoryCustom")
    {
        usePersonalPhotos = true;
        cardGrid.gameObject.SetActive(false);
        photoSetupPanel.SetActive(true);
    }
    else
    {
        usePersonalPhotos = false;
        CreateCards(); // standaard kaarten
        cardGrid.gameObject.SetActive(true);
        if (photoSetupPanel != null) photoSetupPanel.SetActive(false);
    }
}


    void CreateCards()
    {
        List<int> ids = new();
        for (int i = 0; i < cardImages.Length; i++)
        {
            ids.Add(i);
            ids.Add(i); // dubbel voor paren
        }

        totalPairs = cardImages.Length; // ✅ vul hier het totaal aantal paren in (bv. 8)
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
            CardMatchFound();
        }
        else
        {
            firstCard.FlipBack();
            secondCard.FlipBack();
        }
        firstCard = null;
        secondCard = null;
    }

    public void CardMatchFound()
    {
        pairsFound++;

        if (pairsFound >= totalPairs) // bv. 8 voor 16 kaarten
        {
            ShowWinPanel();
        }
    }

    private void ShowWinPanel()
    {
        winPanel.SetActive(true);
        // Je kunt hier ook geluid afspelen of andere effecten
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void AddPersonalPhoto(Sprite photo)
    {
        if (!personalPhotos.Contains(photo))
        {
            personalPhotos.Add(photo);
            Debug.Log("Foto toegevoegd: " + photo.name);

            // Optioneel: herstart het spel om nieuwe kaarten te maken met deze foto’s
            RestartWithNewPhotos();
        }
    }

    void RestartWithNewPhotos()
    
    {
        cardGrid.gameObject.SetActive(true); // nu pas tonen
        // Ruim oude kaarten op
        foreach (Transform child in cardGrid)
            Destroy(child.gameObject);

        // Zet totalPairs afhankelijk van personalPhotos (aantal paren)
        totalPairs = personalPhotos.Count;

        // Maak id list met dubbele paren
        List<int> ids = new List<int>();
        for (int i = 0; i < totalPairs; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        Shuffle(ids);

        foreach (int id in ids)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGrid);
            MemoryCard card = newCard.GetComponent<MemoryCard>();
            card.Setup(personalPhotos[id], id);
        }
    }
    public void StartGameWithPersonalPhotos()
    {
            if (cardGrid.gameObject.activeSelf)
        return; 
        if (personalPhotos.Count < 4)
        {
            Debug.Log("Je moet minstens 4 foto's toevoegen.");
            return;
        }

        usePersonalPhotos = true;
        photoSetupPanel.SetActive(false); // verberg setup UI
        RestartWithNewPhotos();           // start spel
    }
}
