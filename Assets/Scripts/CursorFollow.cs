using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollow : MonoBehaviour
{

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        //第一版
        //Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //transform.transform.position = cursorPos;

        //第二版
        transform.position = Input.mousePosition;
    }
}
