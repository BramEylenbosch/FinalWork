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
        FlipBack();
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
        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);
    }

    public void FlipBack()
    {
        isFlipped = false;
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
    }
}
