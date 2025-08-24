using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemoryCard : MonoBehaviour
{
    public int cardId;
    public Image frontImage;
    public Image backImage;
    public TMP_Text naamText; // label

    private bool isFlipped = false;

    public void Setup(Sprite image, int id, string naam = "")
    {
        frontImage.sprite = image;
        cardId = id;
        if (naamText != null)
            naamText.text = naam;

        FlipBackInstant();
    }

    public void OnClick()
    {
        if (!isFlipped)
            FindObjectOfType<MemoryGameManager>().CardSelected(this);
    }

    public void FlipCard()
    {
        isFlipped = true;
        LeanTween.scaleX(gameObject, 0f, 0.15f).setOnComplete(() =>
        {
            frontImage.gameObject.SetActive(true);
            backImage.gameObject.SetActive(false);
            LeanTween.scaleX(gameObject, 1f, 0.15f);
        });
    }

    public void FlipBack()
    {
        isFlipped = false;
        LeanTween.scaleX(gameObject, 0f, 0.15f).setOnComplete(() =>
        {
            frontImage.gameObject.SetActive(false);
            backImage.gameObject.SetActive(true);
            LeanTween.scaleX(gameObject, 1f, 0.15f);
        });
    }

    private void FlipBackInstant()
    {
        isFlipped = false;
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
        transform.localScale = Vector3.one;
    }
}
