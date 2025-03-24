using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class MenuLoader : MonoBehaviour
{
    public Button goToMenuButton;


    void Start()
    {
        goToMenuButton.onClick.AddListener(loadMenuScene);
    }


    void loadMenuScene()
    {
        SceneManager.LoadScene("Login");
    }
}