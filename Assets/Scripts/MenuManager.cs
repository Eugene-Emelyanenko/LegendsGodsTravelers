using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Bowl
{
    public int bowlIndex;
    public int time;
    public int lives;

    public Bowl(int bowlIndex, int time, int lives)
    {
        this.bowlIndex = bowlIndex;
        this.time = time;
        this.lives = lives;
    }
}

public static class Bowls
{
    public static Bowl GetBowl(int index)
    {
        switch (index)
        {
            case 0:
                return new Bowl(0, 40, 3);

            case 1:
                return new Bowl(1, 35, 3);

            case 2:
                return new Bowl(2, 30, 2);

            case 3:
                return new Bowl(3, 25, 2);

            default:
                return new Bowl(4, 20, 1);
        }
    }
}

[Serializable]
public class BallData
{
    public int ballIndex;
    public int level;
    public int price;
    public bool isDefault;
    public int maxLevel = 10;
    public int levelTimeIncreaser = 2;

    public BallData(int ballIndex, int level, int price, bool isDefault)
    {
        this.ballIndex = ballIndex;
        this.level = level;
        this.price = price;
        this.isDefault = isDefault;
    }
}

public static class BallDataManager
{
    public static readonly string ballDataKey = "BallData";

    public static void SaveBallData(List<BallData> ballDataList)
    {
        BallDataWrapper wrapper = new BallDataWrapper(ballDataList);

        string json = JsonUtility.ToJson(wrapper);

        PlayerPrefs.SetString(ballDataKey, json);

        PlayerPrefs.Save();
    }

    public static List<BallData> LoadBallData()
    {
        if (PlayerPrefs.HasKey(ballDataKey))
        {
            string json = PlayerPrefs.GetString(ballDataKey);

            BallDataWrapper wrapper = JsonUtility.FromJson<BallDataWrapper>(json);

            return wrapper.ballDataList;
        }

        return new List<BallData>();
    }
}

[Serializable]
public class BallDataWrapper
{
    public List<BallData> ballDataList = new List<BallData>();

    public BallDataWrapper(List<BallData> ballDataList)
    {
        this.ballDataList = ballDataList;
    }
}

public class MenuManager : MonoBehaviour
{
    [Header("Market")]
    [SerializeField] private GameObject marketPanel;
    [SerializeField] private Transform ballContainer;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private int ballsCount = 5;

    [Header("Game")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private Image ballIcon;
    [SerializeField] private Image bowlIcon;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private float timeToWait = 3f;
    [SerializeField] private RectTransform gameBall;
    [SerializeField] private float ballSpeed = 10f;
    [SerializeField] private Vector2 startBallPosition;
    [SerializeField] private RectTransform[] bowls;

    [Header("Super Game")]
    [SerializeField] private GameObject superGamePanel;
    [SerializeField] private Transform bowlButtonsContainer;
    private int selectedBowlIndex = 0;
    [SerializeField] private RectTransform coinsRectTransform;
    [SerializeField] private Vector2 leftCoinsPosition;
    [SerializeField] private Vector2 rightCoinsPosition;
    [SerializeField] private Image selectedBallImage;
    private int selectedBallIndex = 0;
    [SerializeField] private int superGamePrice = 50;

    [Header("Instructions")]
    [SerializeField] private GameObject instructionsPanel;

    private List<BallData> ballDataList = new List<BallData>();
    private Image gameBallImage;

    private void Awake()
    {
        gameBallImage = gameBall.GetComponent<Image>();
        ballDataList = BallDataManager.LoadBallData();
        if (ballDataList.Count == 0)
            CreateDefaultBallData();
    }

    private void Start()
    {
        UpdateBallDisplay();

        int index = 0;
        foreach (Transform t in bowlButtonsContainer)
        {
            Button button = t.GetComponentInChildren<Button>();
            GameObject isSelected = t.Find("IsSelected").gameObject;

            int currentIndex = index;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                selectedBowlIndex = currentIndex;
                isSelected.SetActive(true);

                foreach (Transform t2 in bowlButtonsContainer)
                {
                    if (t2 != t)
                    {
                        GameObject otherIsSelected = t2.Find("IsSelected").gameObject;
                        otherIsSelected.SetActive(false);
                    }
                }
            });

            index++;
        }

        selectedBowlIndex = 0;
        bowlButtonsContainer.GetChild(selectedBowlIndex).transform.Find("IsSelected").gameObject.SetActive(true);

        dialogPanel.SetActive(false);
        EnableInstructionsPanel(false);
        EnableMarket(false);
        EnableGamePanel(false);
        EnableSuperGamePanel(false);
        UpdateCoinsText();
    }

    public void EnableMarket(bool isOpen)
    {
        marketPanel.SetActive(isOpen);

        if(isOpen)
        {
            DisplayBalls();
            MoveCoins(false);
        }
        else
        {
            MoveCoins(true);
        }
    }

    public void EnableGamePanel(bool isOpen)
    {
        gamePanel.SetActive(isOpen);

        if (isOpen)
        {
            gameBall.anchoredPosition = startBallPosition;
            int ballDataIndex = Random.Range(0, ballDataList.Count);
            PlayerPrefs.SetInt("SelectedBall", ballDataIndex);
            BallData ballData = ballDataList[ballDataIndex];
            gameBallImage.sprite = Resources.Load<Sprite>($"Balls/{ballData.ballIndex}");

            int bowlIndex = Random.Range(0, bowls.Length);
            Bowl selectedBowl = Bowls.GetBowl(bowlIndex);
            PlayerPrefs.SetInt("SelectedBowl", bowlIndex);

            gameBall.DOAnchorPos(bowls[bowlIndex].anchoredPosition, ballSpeed).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameBall.gameObject.SetActive(false);
                dialogPanel.SetActive(true);
                ballIcon.sprite = Resources.Load<Sprite>($"Balls/{ballData.ballIndex}");
                string theme = ballData.ballIndex == 0 ? "zues" : ballData.ballIndex == 1 ? "clock" : ballData.ballIndex == 2 ? "city" : ballData.ballIndex == 3 ? "hieroglyph" : "statue";
                string hole = bowlIndex == 0 ? "first" : bowlIndex == 1 ? "second" : bowlIndex == 2 ? "third" : bowlIndex == 3 ? "fourth" : "fifth";
                bowlIcon.sprite = Resources.Load<Sprite>($"Bowls/{bowlIndex}");
                dialogText.text = $"You got a ball with a {theme} theme! " +
                $"The ball hits the {hole} hole - you " +
                $"are given {selectedBowl.time} seconds and " +
                $"{selectedBowl.lives} lives to play.";
                StartCoroutine(GameWait());
            });

            PlayerPrefs.Save();
        }
        else
        {
            gameBall.DOKill();
            gameBall.gameObject.SetActive(true);
            gameBall.anchoredPosition = startBallPosition;
        }
    }

    public void EnableSuperGamePanel(bool isOpen)
    {
        superGamePanel.SetActive(isOpen);

        if(isOpen)
        {
            MoveCoins(false);
        }
        else
        {
            MoveCoins(true);
        }
    }

    public void EnableInstructionsPanel(bool isOpen)
    {
        instructionsPanel.SetActive(isOpen);
        EnableSuperGamePanel(false);
        EnableMarket(false);

        if (isOpen)
        {
            MoveCoins(false);
        }
        else
        {
            MoveCoins(true);
        }
    }

    private void MoveCoins(bool isLeft)
    {
        Vector2 targetPosition = isLeft ? leftCoinsPosition : rightCoinsPosition;

        if (isLeft)
        {
            coinsRectTransform.anchorMin = new Vector2(0, 1);
            coinsRectTransform.anchorMax = new Vector2(0, 1);

            coinsRectTransform.anchoredPosition = targetPosition;
        }
        else
        {
            coinsRectTransform.anchorMin = new Vector2(1, 1);
            coinsRectTransform.anchorMax = new Vector2(1, 1);

            coinsRectTransform.anchoredPosition = targetPosition;
        }
    }

    IEnumerator GameWait()
    {
        yield return new WaitForSeconds(timeToWait);
        SceneManager.LoadScene("Game");
    }

    private void CreateDefaultBallData()
    {
        Debug.Log("Creating default ball data");

        for (int i = 0; i < ballsCount; i++)
        {
            ballDataList.Add(new BallData(i, 1, 15, i == 0));
        }

        BallDataManager.SaveBallData(ballDataList);
    }

    private void DisplayBalls()
    {
        for (int i = 0; i < ballContainer.childCount; i++)
        {
            GameObject ballObject = ballContainer.GetChild(i).gameObject;
            BallData data = ballDataList[i];
            ballObject.name = $"Ball_{data.ballIndex}";
            BallUI ballUI = ballObject.GetComponent<BallUI>();
            ballUI.SetUp(data);
            ballUI.button.onClick.RemoveAllListeners();
            ballUI.button.onClick.AddListener(() =>
            {
                if (data.level == data.maxLevel)
                    return;

                int coins = Coins.GetCoins();
                if(coins >= data.price)
                {
                    coins -= data.price;
                    Coins.SaveCoins(coins);
                    SoundManager.Instance.PlayClip(SoundManager.Instance.buySound);
                    UpdateCoinsText();
                    data.level++;
                    BallDataManager.SaveBallData(ballDataList);
                    DisplayBalls();
                }
            });
        }
    }

    private void UpdateBallDisplay()
    {
        BallData currentBall = ballDataList[selectedBallIndex];
        selectedBallImage.sprite = Resources.Load<Sprite>($"Balls/{currentBall.ballIndex}");

        Debug.Log("Selected Ball: " + currentBall.ballIndex + " (Level: " + currentBall.level + ")");
    }

    public void Scroll(bool isRight)
    {
        if (isRight)
        {
            selectedBallIndex++;

            if (selectedBallIndex >= ballDataList.Count)
                selectedBallIndex = 0;
        }
        else
        {
            selectedBallIndex--;

            if (selectedBallIndex < 0)
                selectedBallIndex = ballDataList.Count - 1;
        }

        UpdateBallDisplay();
    }

    private void UpdateCoinsText()
    {
        coinsText.text = Coins.GetCoins().ToString();
    }

    public void StartSuperGame()
    {
        int coins = Coins.GetCoins();

        if(coins >= superGamePrice)
        {
            coins -= superGamePrice;
            Coins.SaveCoins(coins);
            SoundManager.Instance.PlayClip(SoundManager.Instance.buySound);
            UpdateCoinsText();
            PlayerPrefs.SetInt("SelectedBall", selectedBallIndex);
            PlayerPrefs.SetInt("SelectedBowl", selectedBowlIndex);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Game");
        }
    }
}
