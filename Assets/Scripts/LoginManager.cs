using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button goToRegisterButton;
    public Button goToForumButton;

    private string dbPath;

    void Start()
    {
        // Chemin de la base de données
        dbPath = "URI=file:" + Application.persistentDataPath + "/userDatabase.db";

        // Ajouter des listeners aux boutons
        loginButton.onClick.AddListener(LoginUser);
        goToRegisterButton.onClick.AddListener(MoveToRegisterScene);
        goToForumButton.onClick.AddListener(MoveToForumScene);

        // Vérifier que la base de données existe
        if (!File.Exists(Application.persistentDataPath + "/userDatabase.db"))
        {
            Debug.LogError("La base de données n'existe pas. Veuillez vous enregistrer d'abord.");
        }
    }

    void MoveToRegisterScene()
    {
        SceneManager.LoadScene("Register");
    }

    void LoginUser()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.Log("Le nom d'utilisateur ou le mot de passe ne peut pas être vide.");
            return;
        }

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            // Vérifier si les identifiants sont corrects
            string loginQuery = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
            using (var command = new SqliteCommand(loginQuery, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password); // En production, comparer les mots de passe (hashage)

                long userExists = (long)command.ExecuteScalar();

                if (userExists > 0)
                {
                    Debug.Log($"Connexion réussie pour l'utilisateur '{username}' !");
                    PlayerPrefs.SetString("LoggedInUser", usernameInput.text);
                    PlayerPrefs.Save();
                    loadIntroScene();

                }
                else
                {
                    Debug.Log("Nom d'utilisateur ou mot de passe incorrect.");
                }
            }

            connection.Close();
        }
    }

    void loadIntroScene()
    {
        SceneManager.LoadScene("introScene");
    }

    void MoveToForumScene()
    {
        SceneManager.LoadScene("forumScene");
    }
}
