using UnityEngine;
using UnityEngine.UI;

public class UIImageSwitcher : MonoBehaviour
{
    public Image displayImage;
    public Sprite[] images;
    public int currentIndex = 0;

    public void NextImage()
    {
        currentIndex++;
        if (currentIndex >= images.Length) currentIndex = 0;
        displayImage.sprite = images[currentIndex];
    }

    public void PreviousImage()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = images.Length - 1;
        displayImage.sprite = images[currentIndex];
    }
}
