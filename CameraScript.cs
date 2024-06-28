using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _target; // Yak�nla�t�r�lacak modelin transformu
    public float minZoom = 10; // Minimum g�r�� a��s�
    public float maxZoom = 350; // Maksimum g�r�� a��s�
    public float zoomSpeed = 10; // Yak�nla�t�rma h�z�
    private float currentZoom = 60; // Mevcut g�r�� a��s�

    
    private float _mouseSensitivity = 3.0f;
    private float _rotationY;
    private float _rotationX;

    private float _distanceFromTarget = 3.0f;

    private Vector3 _currentRotation;
    private Vector3 _smoothVelocity = Vector3.zero;
    private float _smoothTime = 0.2f;

    private Vector2 _rotationXMinMax = new Vector2(-40, 40);

    private bool canMove = true; // Kameran�n hareketine izin veren kontrol de�i�keni

    void Update()
    {
        if (canMove)
        {
            MotorScript motorScript = GetComponent<MotorScript>();
            if (motorScript.model != null)
            {
                _target = motorScript.model;
            }

            // Mouse scrolunun de�erini okuyup mevcut g�r�� a��s�ndan ��kart
            currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            // Sonucu minimum ve maksimum aras�nda s�n�rla
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
        canMove = move; // Kameran�n hareketini etkinle�tirme veya devre d��� b�rakma
    }

    void LateUpdate()
    {
        // Kameran�n g�r�� a��s�n� mevcut g�r�� a��s� ile g�ncelle
        Camera.main.fieldOfView = currentZoom;
    }
}

