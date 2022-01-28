using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorial : MonoBehaviour
{
    private void Start()
    {
        Vector3 pos = transform.localPosition;
        pos.x = -130;
        transform.localPosition = pos;
        transform.LeanMoveLocalX(130, 1.2f).setEaseInOutQuart().setLoopPingPong();
    }
}
