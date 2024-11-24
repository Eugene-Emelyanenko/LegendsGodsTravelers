using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour
{
    private bool isMoving = false;      // ����, ����������� �� ��, ��� ������ ������������
    private Vector2 mousePosition;      // ������� ����
    private Vector2 startPos;           // ��������� ������� �������
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
        // ����� ������ ������ ����, �������� ���� �����������
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving && !isPlaced)
        {
            // ����������� ������� ���� � ���������� ����
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // ���������� ������ � ������� ����
            transform.position = new Vector2(mousePosition.x, mousePosition.y);
        }
    }

    private void OnMouseUp()
    {
        // ����� ��������� ������ ����, �������� ���������� ������
        isMoving = false;

        // ���� ����� �������� �� ���������� ������� � ����� ������� ������ � ��������� ����� ��� ��������� ������ ��������.
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
