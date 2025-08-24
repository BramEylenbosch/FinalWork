using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MemoryGameManager : MonoBehaviour
{
    [Header("Cards")]
    public GameObject cardPrefab;
    public Transform cardGrid;

    [Header("Standard Card Images (auto)")]
    public Sprite[] cardImages;

    [Header("Photo Setup (custom)")]
    public GameObject photoSetupPanel;
    public Transform photoPreviewGrid;
    public GameObject photoThumbnailPrefab;
    public Button startGameButton;
    public TMP_Text uitlegTekst;

    [Header("Win Panel")]
    public GameObject winPanel;

    [HideInInspector]
    public List<PersonalPhotoData> personalPhotosData = new List<PersonalPhotoData>();

    private List<MemoryCard> activeCards = new List<MemoryCard>();
    private MemoryCard firstCard, secondCard;
    private int totalPairs;
    private int pairsFound = 0;
    private bool usePersonalPhotos = false;

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MemoryCustom")
        {
            usePersonalPhotos = true;
            cardGrid.gameObject.SetActive(false);
            photoSetupPanel.SetActive(true);

            startGameButton.interactable = personalPhotosData.Count >= 2;
        }
        else
        {
            usePersonalPhotos = false;
            photoSetupPanel.SetActive(false);
            CreateStandardCards();
            cardGrid.gameObject.SetActive(true);
        }
    }

    // ---------- CARD CREATION ----------

    private void CreateStandardCards()
    {
        cardGrid.gameObject.SetActive(true);
        foreach (Transform child in cardGrid)
            Destroy(child.gameObject);

        totalPairs = cardImages.Length;

        List<int> ids = new List<int>();
        for (int i = 0; i < cardImages.Length; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        Shuffle(ids);

        foreach (int id in ids)
        {
            GameObject newCard = Instantiate(cardPrefab, cardGrid);
            MemoryCard card = newCard.GetComponent<MemoryCard>();
            card.Setup(cardImages[id], id); // Naam leeg voor auto kaarten
        }
    }

    public void CreateCardsFromPersonalPhotos()
    {
        cardGrid.gameObject.SetActive(true);

        foreach (Transform child in cardGrid)
            Destroy(child.gameObject);

        totalPairs = personalPhotosData.Count;

        List<int> ids = new List<int>();
        for (int i = 0; i < totalPairs; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        Shuffle(ids);

        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];
            GameObject newCard = Instantiate(cardPrefab, cardGrid);
            MemoryCard card = newCard.GetComponent<MemoryCard>();

            PersonalPhotoData data = personalPhotosData[id];
            string label = data.naam + (string.IsNullOrEmpty(data.functie) ? "" : " – " + data.functie);

            card.Setup(data.photo, id, label);
        }
    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }

    // ---------- CARD SELECTION ----------

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

    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);

        if (firstCard.cardId == secondCard.cardId)
        {
            pairsFound++;
            CheckWin();
        }
        else
        {
            firstCard.FlipBack();
            secondCard.FlipBack();
        }

        firstCard = null;
        secondCard = null;
    }

    private void CheckWin()
    {
        if (pairsFound >= totalPairs)
        {
            if (winPanel != null)
                winPanel.SetActive(true);
        }
    }

    // ---------- PERSONAL PHOTO MANAGEMENT ----------

    public void AddPersonalPhotoData(PersonalPhotoData data)
    {
        if (personalPhotosData.Count >= 8)
        {
            Debug.Log("Maximaal 8 foto's toegestaan.");
            return;
        }

        personalPhotosData.Add(data);

        GameObject thumbnail = Instantiate(photoThumbnailPrefab, photoPreviewGrid);
        thumbnail.GetComponent<Image>().sprite = data.photo;

        Button removeBtn = thumbnail.transform.Find("RemoveButton").GetComponent<Button>();
        removeBtn.onClick.AddListener(() => RemovePhotoData(data, thumbnail));

        startGameButton.interactable = personalPhotosData.Count >= 2 && personalPhotosData.Count <= 8;
    }

    public void RemovePhotoData(PersonalPhotoData data, GameObject thumbnail)
    {
        if (personalPhotosData.Contains(data))
        {
            personalPhotosData.Remove(data);
            Destroy(thumbnail);

            startGameButton.interactable = personalPhotosData.Count >= 2 && personalPhotosData.Count <= 8;
        }
    }

    public void StartGameWithPersonalPhotos()
    {
        if (personalPhotosData.Count < 2)
        {
            Debug.Log("Je moet minstens 2 foto's toevoegen.");
            return;
        }

        if (uitlegTekst != null) uitlegTekst.gameObject.SetActive(false);
        photoSetupPanel.SetActive(false);

        CreateCardsFromPersonalPhotos();
    }

    // ---------- RESTART ----------

    public void RestartGame()
    {
        pairsFound = 0;
        firstCard = null;
        secondCard = null;

        if (usePersonalPhotos)
        {
            CreateCardsFromPersonalPhotos();
        }
        else
        {
            CreateStandardCards();
        }

        if (winPanel != null)
            winPanel.SetActive(false);
    }
}
