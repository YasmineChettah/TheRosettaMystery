using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class IntroSceneSkip : MonoBehaviour
{
    public Button goToGameButton;


    void Start()
    {  
        goToGameButton.onClick.AddListener(loadGameScene);
    }

  
    void loadGameScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
