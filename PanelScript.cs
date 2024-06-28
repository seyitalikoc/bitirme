using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScript : MonoBehaviour
{
    public GameObject panel; // Unity Editor'den panel nesnesini atadýðýnýzý varsayalým
    public CameraController cameraMovementScript; // Kameranýn hareketini kontrol eden script

    private void Update()
    {
        // Fare imleci panelin üzerindeyse kameranýn hareketini durdur
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
        // Fare imleci panelin içinde mi kontrol et
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        Vector2 mousePosition = Input.mousePosition;
        bool isOverPanel = RectTransformUtility.RectangleContainsScreenPoint(panelRect, mousePosition);
        return isOverPanel;
    }
}
