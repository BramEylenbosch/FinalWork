using UnityEngine;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    public int cardId;
    public Image frontImage;
    public Image backImage;
    private bool isFlipped = false;

    public void Setup(Sprite image, int id)
    {
        frontImage.sprite = image;
        cardId = id;
        FlipBackInstant(); // ← instant zonder animatie bij start
    }

    public void OnClick()
    {
        if (!isFlipped)
        {
            FindObjectOfType<MemoryGameManager>().CardSelected(this);
        }
    }

    public void FlipCard()
    {
        isFlipped = true;

        // Animate flip to front
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

        // Animate flip to back
        LeanTween.scaleX(gameObject, 0f, 0.15f).setOnComplete(() =>
        {
            frontImage.gameObject.SetActive(false);
            backImage.gameObject.SetActive(true);
            LeanTween.scaleX(gameObject, 1f, 0.15f);
        });
    }

    // Instant reset zonder animatie (voor Setup)
    private void FlipBackInstant()
    {
        isFlipped = false;
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
        transform.localScale = Vector3.one;
    }
}
