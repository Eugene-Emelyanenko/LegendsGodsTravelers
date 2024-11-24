using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate : MonoBehaviour
{
    public int frameRate = 60;

    private void Start()
    {
        Application.targetFrameRate = frameRate;
    }
}
