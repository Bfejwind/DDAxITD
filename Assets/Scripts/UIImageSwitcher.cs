using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIImageSwitcher : MonoBehaviour
{
    public Image displayImage;
    public Sprite[] images;
    public int currentIndex = 0;
    public List<string> archetypes = new List<string>();
    public TMP_Text archetypeStat;
    void Start()
    {
        archetypes.Add("Strength");
        archetypes.Add("Speed");
    }

    public void NextImage()
    {
        currentIndex++;
        if (currentIndex >= images.Length) currentIndex = 0;
        displayImage.sprite = images[currentIndex];
        archetypeStat.text = archetypes[currentIndex];
    }

    public void PreviousImage()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = images.Length - 1;
        displayImage.sprite = images[currentIndex];
        archetypeStat.text = archetypes[currentIndex];
    }
}
