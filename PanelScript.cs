using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScript : MonoBehaviour
{
    public GameObject panel; // Unity Editor'den panel nesnesini atad���n�z� varsayal�m
    public CameraController cameraMovementScript; // Kameran�n hareketini kontrol eden script

    private void Update()
    {
        // Fare imleci panelin �zerindeyse kameran�n hareketini durdur
        if (IsMouseOverPanel())
        {
            cameraMovementScript.SetCanMove(false);
        }
        else
        {
            cameraMovementScript.SetCanMove(true);
        }
    }

    private bool IsMouseOverPanel()
    {
        // Fare imleci panelin i�inde mi kontrol et
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        Vector2 mousePosition = Input.mousePosition;
        bool isOverPanel = RectTransformUtility.RectangleContainsScreenPoint(panelRect, mousePosition);
        return isOverPanel;
    }
}
