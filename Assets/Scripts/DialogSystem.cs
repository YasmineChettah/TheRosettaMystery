using TMPro;
using UnityEngine;


public class DialogSystem : MonoBehaviour
{
    public static DialogSystem Instance { get; set; }


    public TextMeshProUGUI dialogText;

    public Canvas dialogUI;

    public bool dialogUIActive;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void OpenDialogUI()
    { 
        dialogUI.gameObject.SetActive(true);
        dialogUIActive = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CloseDialogUI() 
    {
        dialogUI.gameObject.SetActive(false);
        dialogUIActive = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
