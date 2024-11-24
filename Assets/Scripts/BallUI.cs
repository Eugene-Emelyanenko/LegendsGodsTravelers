using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallUI : MonoBehaviour
{
    [SerializeField] private Image ballIcon;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI levelText;
    public Button button;
    public BallData ballData;

    public void SetUp(BallData data)
    {
        ballData = data;
        ballIcon.sprite = Resources.Load<Sprite>($"Balls/{ballData.ballIndex}");
        priceText.text = ballData.price.ToString();
        levelText.text = ballData.level.ToString();      
    }
}
