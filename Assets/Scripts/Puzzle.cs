using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour
{
    private bool isMoving = false;      // Флаг, указывающий на то, что объект перемещается
    private Vector2 mousePosition;      // Позиция мыши
    private Vector2 startPos;           // Начальная позиция объекта
    private Transform puzzlePoint;
    public bool isPlaced { get; private set; }
    private Image puzzleImage;
    private GameManager gameManager;

    private void Awake()
    {
        puzzleImage = GetComponent<Image>();
        gameManager = FindObjectOfType<GameManager>();        
    }

    private void Start()
    {
        isPlaced = false;
        startPos = transform.position;
    }

    private void OnMouseDown()
    {
        // Когда нажата кнопка мыши, включаем флаг перемещения
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving && !isPlaced)
        {
            // Преобразуем позицию мыши в координаты мира
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Перемещаем объект в позицию мыши
            transform.position = new Vector2(mousePosition.x, mousePosition.y);
        }
    }

    private void OnMouseUp()
    {
        // Когда отпускаем кнопку мыши, перестаём перемещать объект
        isMoving = false;

        // Если нужна проверка на допустимую позицию — можно вернуть объект в начальную точку или выполнять логику подгонки.
        if(Mathf.Abs(transform.position.x - puzzlePoint.position.x) <= 1f &&
            Mathf.Abs(transform.position.y - puzzlePoint.position.y) <= 1f)
        {
            transform.position = puzzlePoint.position;
            isPlaced = true;
            gameManager.CheckWin();
        }
        else
        {
            gameManager.TakeDamage();
            transform.position = startPos;
            isPlaced = false;
        }
    }

    public void SetUp(Transform point, Sprite partSprite)
    {
        puzzlePoint = point;
        puzzleImage.sprite = partSprite;
    }
}
