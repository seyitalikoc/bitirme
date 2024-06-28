using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _target; // Yakýnlaþtýrýlacak modelin transformu
    public float minZoom = 10; // Minimum görüþ açýsý
    public float maxZoom = 350; // Maksimum görüþ açýsý
    public float zoomSpeed = 10; // Yakýnlaþtýrma hýzý
    private float currentZoom = 60; // Mevcut görüþ açýsý

    
    private float _mouseSensitivity = 3.0f;
    private float _rotationY;
    private float _rotationX;

    private float _distanceFromTarget = 3.0f;

    private Vector3 _currentRotation;
    private Vector3 _smoothVelocity = Vector3.zero;
    private float _smoothTime = 0.2f;

    private Vector2 _rotationXMinMax = new Vector2(-40, 40);

    private bool canMove = true; // Kameranýn hareketine izin veren kontrol deðiþkeni

    void Update()
    {
        if (canMove)
        {
            MotorScript motorScript = GetComponent<MotorScript>();
            if (motorScript.model != null)
            {
                _target = motorScript.model;
            }

            // Mouse scrolunun deðerini okuyup mevcut görüþ açýsýndan çýkart
            currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            // Sonucu minimum ve maksimum arasýnda sýnýrla
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

                _rotationY += mouseX;
                _rotationX += mouseY;

                _rotationX = Mathf.Clamp(_rotationX, _rotationXMinMax.x, _rotationXMinMax.y);
                Vector3 nextRotation = new Vector3(_rotationX, _rotationY);

                _currentRotation = Vector3.SmoothDamp(_currentRotation, nextRotation, ref _smoothVelocity, _smoothTime);
                transform.localEulerAngles = _currentRotation;

                if (_target == null)
                {
                    transform.position = new Vector3(0, 0, -3);
                }
                else
                {
                    transform.position = _target.position - transform.forward * _distanceFromTarget;
                }
            }
        }

        
    }

    public void SetCanMove(bool move)
    {
        canMove = move; // Kameranýn hareketini etkinleþtirme veya devre dýþý býrakma
    }

    void LateUpdate()
    {
        // Kameranýn görüþ açýsýný mevcut görüþ açýsý ile güncelle
        Camera.main.fieldOfView = currentZoom;
    }
}

