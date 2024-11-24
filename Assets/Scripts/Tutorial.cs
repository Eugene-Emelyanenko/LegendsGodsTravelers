using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private RectTransform handTransform; // RectTransform ��� ��������
    [SerializeField] private Vector2 startPoint;          // ��������� ����� (anchor position)
    [SerializeField] private Vector2 endPoint;            // �������� ����� (anchor position)
    public float speed;                                   // �������� ��������
    private bool isAnimationStarted = false;

    public void StartLoopAnimation()
    {
        if (isAnimationStarted)
            return;

        isAnimationStarted = true;
        // ������������� ��������� �������
        handTransform.anchoredPosition = startPoint;

        // �������� ����������� ����� �������
        handTransform.DOAnchorPos(endPoint, speed)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo); // ����������� �������� ���� � �������
    }
}
