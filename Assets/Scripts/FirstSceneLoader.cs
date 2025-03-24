using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstSceneLoader : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
