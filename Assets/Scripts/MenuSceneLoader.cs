using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneLoader : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.LoadScene("Login", LoadSceneMode.Single);
    }
}

