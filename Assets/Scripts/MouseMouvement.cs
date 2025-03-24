using UnityEngine;

public class MouseMouvement : MonoBehaviour
{

    public Joystick lookJoystick;  // Joystick pour la rotation de la cam�ra
    public float lookSensitivity = 2f;  // Sensibilit� du mouvement
    public Transform playerBody; 

    private float xRotation = 0f;

    void Update()
    {
        Look();
    }

    void Look()
    {
        if (lookJoystick == null) return;

        // R�cup�ration des entr�es du joystick
        float x = lookJoystick.Horizontal * lookSensitivity;
        float y = lookJoystick.Vertical * lookSensitivity;

        // Rotation verticale (haut/bas) 
        xRotation -= y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotation horizontale (gauche/droite)
        playerBody.Rotate(Vector3.up * x);
    }
}
