using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
    public Button startGameButton; // Sleep hier je Start Game button in de Inspector
    public GameObject photoThumbnailPrefab;
    public static List<Sprite> savedPersonalPhotos = new();
    public TextMeshProUGUI uitlegTekst;


    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MemoryCustom")
        {
            usePersonalPhotos = true;
            cardGrid.gameObject.SetActive(false);
            photoSetupPanel.SetActive(true);

            // ✅ Herstel opgeslagen foto's
            if (savedPersonalPhotos.Count > 0)
            {
                personalPhotos = new List<Sprite>(savedPersonalPhotos);

                // Toon thumbnails opnieuw
                foreach (var photo in personalPhotos)
                {
                    GameObject thumbnail = Instantiate(photoThumbnailPrefab, photoPreviewGrid);
                    thumbnail.GetComponent<Image>().sprite = photo;

                    // Voeg ook verwijderknop listener toe bij laden
                    Button removeBtn = thumbnail.transform.Find("RemoveButton").GetComponent<Button>();
                    Sprite capturedPhoto = photo; // lokale kopie voor lambda
                    removeBtn.onClick.AddListener(() => RemovePhoto(capturedPhoto, thumbnail));
                }

                // Activeer startknop als er tussen 2 en 8 foto's zijn
                startGameButton.interactable = personalPhotos.Count >= 2 && personalPhotos.Count <= 8;
            }
            else
            {
                startGameButton.interactable = false;
            }
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

        totalPairs = cardImages.Length; // bv. 8
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

        if (pairsFound >= totalPairs)
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
        savedPersonalPhotos = new List<Sprite>(personalPhotos); // bewaar foto's
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddPersonalPhoto(Sprite photo)
    {
        if (personalPhotos.Count >= 8)
        {
            Debug.Log("Maximaal 8 foto's toegestaan.");
            return;
        }

        if (!personalPhotos.Contains(photo))
        {
            personalPhotos.Add(photo);
            Debug.Log("Foto toegevoegd: " + photo.name);

            // Toon thumbnail in setup UI
            GameObject thumbnail = Instantiate(photoThumbnailPrefab, photoPreviewGrid);
            thumbnail.GetComponent<Image>().sprite = photo;

            // Zoek de verwijderknop en voeg een listener toe
            Button removeBtn = thumbnail.transform.Find("RemoveButton").GetComponent<Button>();
            removeBtn.onClick.AddListener(() => RemovePhoto(photo, thumbnail));

            // Startknop actief tussen 2 en 8 kaarten
            startGameButton.interactable = personalPhotos.Count >= 2 && personalPhotos.Count <= 8;
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

        if (personalPhotos.Count < 2)
        {
            Debug.Log("Je moet minstens 2 foto's toevoegen.");
            return;
        }
        if (personalPhotos.Count > 8)
        {
            Debug.Log("Maximaal 8 foto's toegestaan.");
            return;
        }

        usePersonalPhotos = true;
        photoSetupPanel.SetActive(false); // verberg setup UI

        if (uitlegTekst != null)
            uitlegTekst.gameObject.SetActive(false); // verberg uitleg

        RestartWithNewPhotos();           // start spel
    }


    public void RemovePhoto(Sprite photoToRemove, GameObject thumbnail)
    {
        if (personalPhotos.Contains(photoToRemove))
        {
            personalPhotos.Remove(photoToRemove);
            Destroy(thumbnail);

            // Bewaar aangepaste lijst
            savedPersonalPhotos = new List<Sprite>(personalPhotos);

            // Check opnieuw of je genoeg foto's hebt
            startGameButton.interactable = personalPhotos.Count >= 2 && personalPhotos.Count <= 8;
        }
    }
}
