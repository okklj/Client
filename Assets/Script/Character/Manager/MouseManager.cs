using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySingleTon;
public class MouseManager :SingleTon<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;
    RaycastHit hitInfo;

    private void Update()
    {
        SetCursorTexture();
    }

    void SetCursorTexture()//…Ë÷√ Û±ÍÃ˘Õº
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            //«–ªªÃ˘Õº
            switch (hitInfo.collider.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }
}
