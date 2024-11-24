using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Puzzles")]
    public int rows = 2;
    public int cols = 3;
    public Image puzzleImage;
    public Puzzle[] puzzles;
    public Transform[] puzzlePoints;

    [Header("Hearts")]
    public int health = 3;
    public Transform heartsContainer;

    [Header("Timer")]
    public int startTimer = 40;
    public TextMeshProUGUI timerText;

    [Header("Game Over")]
    public int coinsToWin = 15;

    private bool isGameOver = false;

    private Sprite puzzleSprite;
    private float timer;

    private BallData selectedBallData;
    private Bowl selectedBowl;

    private List<BallData> ballDataList = new List<BallData>();

    private void Awake()
    {
        ballDataList = BallDataManager.LoadBallData();

        int selectedBallDataIndex = PlayerPrefs.GetInt("SelectedBall", 0);
        int selectedBowlIndex = PlayerPrefs.GetInt("SelectedBowl", 0);

        selectedBallData = ballDataList[selectedBallDataIndex];

        selectedBowl = Bowls.GetBowl(selectedBowlIndex);

        health = selectedBowl.lives;

        startTimer = selectedBowl.time + (selectedBallData.level * selectedBallData.levelTimeIncreaser);

        int puzzleIndex = Random.Range(0, 5);
        puzzleSprite = Resources.Load<Sprite>($"Puzzles/{selectedBallData.ballIndex}/{puzzleIndex}");
        puzzleImage.sprite = puzzleSprite;
        UpdateTimerText();
        UpdateHearts();
        SplitIntoParts();
        ShufflePuzzles();
    }

    void Start()
    {
        timer = startTimer;
        StartCoroutine(StartTimer());
    }

    void SplitIntoParts()
    {
        Texture2D texture = puzzleSprite.texture;
        int partWidth = texture.width / cols;
        int partHeight = texture.height / rows;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                if (index < puzzles.Length)
                {
                    Rect rect = new Rect(col * partWidth, texture.height - (row + 1) * partHeight, partWidth, partHeight);
                    Sprite partSprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

                    puzzles[index].SetUp(puzzlePoints[index], partSprite);
                }
            }
        }
    }

    void ShufflePuzzles()
    {
        for (int i = 0; i < puzzles.Length; i++)
        {
            // Случайное число в диапазоне от i до конца массива
            int randomIndex = Random.Range(i, puzzles.Length);

            // Обмен местами элементов
            Puzzle temp = puzzles[i];
            puzzles[i] = puzzles[randomIndex];
            puzzles[randomIndex] = temp;

            // Также можно поменять их начальные позиции, чтобы они случайно появились на экране
            Vector3 tempPos = puzzles[i].transform.position;
            puzzles[i].transform.position = puzzles[randomIndex].transform.position;
            puzzles[randomIndex].transform.position = tempPos;
        }
    }

    public void TakeDamage()
    {
        health--;

        if(health <= 0)
        {
            health = 0;
            GameOver(false);
            SoundManager.Instance.PlayClip(SoundManager.Instance.gameoverSound);
        }
        else
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.failSound);
        }

        UpdateHearts();
    }

    private void UpdateHearts()
    {
        foreach (Transform t in heartsContainer)
        {
            t.gameObject.SetActive(false);
        }

        for (int i = 0; i < health; i++)
        {
            heartsContainer.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void GameOver(bool isWin)
    {
        if(isGameOver)
            return;

        isGameOver = true;

        if(isWin)
        {
            int coins = Coins.GetCoins();
            coins += coinsToWin;
            Coins.SaveCoins(coins);
            Debug.Log("Win");          
        }
        else
        {
            Handheld.Vibrate();
            Debug.Log("Game Over");
        }       
    }

    public void CheckWin()
    {
        foreach (Puzzle puzzle in puzzles)
        {
            if (!puzzle.isPlaced)
            {
                SoundManager.Instance.PlayClip(SoundManager.Instance.scoreSound);
                return;
            }
                
        }
        SoundManager.Instance.PlayClip(SoundManager.Instance.winSound);
        GameOver(true);
    }

    // Логика таймера
    IEnumerator StartTimer()
    {
        while (timer > 0 && !isGameOver)
        {
            // Секунды уменьшаются каждую секунду
            yield return new WaitForSeconds(1f);
            timer--;

            // Обновляем текст таймера
            UpdateTimerText();
        }

        // Когда время закончилось — игра окончена
        GameOver(false);
    }

    private void UpdateTimerText()
    {
        // Форматируем таймер в m:ss
        int minutes = Mathf.FloorToInt(timer / 60); // Рассчитываем минуты
        int seconds = Mathf.FloorToInt(timer % 60); // Рассчитываем секунды
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds); // Формат m:ss
    }
}
