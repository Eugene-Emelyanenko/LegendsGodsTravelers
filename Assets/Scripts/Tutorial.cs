using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private RectTransform handTransform; // RectTransform для анимации
    [SerializeField] private Vector2 startPoint;          // Начальная точка (anchor position)
    [SerializeField] private Vector2 endPoint;            // Конечная точка (anchor position)
    public float speed;                                   // Скорость анимации
    private bool isAnimationStarted = false;

    public void StartLoopAnimation()
    {
        if (isAnimationStarted)
            return;

        isAnimationStarted = true;
        // Устанавливаем начальную позицию
        handTransform.anchoredPosition = startPoint;

        // Анимация перемещения между точками
        handTransform.DOAnchorPos(endPoint, speed)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo); // Зацикливаем движение туда и обратно
    }
}
