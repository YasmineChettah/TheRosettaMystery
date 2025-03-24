using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class ForumManager : MonoBehaviour
{
    public TMP_InputField commentInput; // Champ pour entrer le commentaire
    public Button sendButton;          // Bouton pour envoyer
    public Button logoutButton;
    public TMP_Text responseText;      // Texte pour afficher la r�ponse
    public string loggedInUser;        // Nom de l'utilisateur connect�
    public GameObject commentPrefab;   
    public Transform commentContainer; 

    private WebSocket ws; // Instance WebSocket

    void Start()
    {
        // V�rification de l'utilisateur connect�
        loggedInUser = PlayerPrefs.GetString("LoggedInUser", "");
        if (string.IsNullOrEmpty(loggedInUser))
        {
            Debug.LogError("Aucun utilisateur connect� !");
            responseText.text = "Erreur : Aucun utilisateur connect�.";
            return;
        }

        // Initialiser la connexion WebSocket
        ws = new WebSocket("ws://localhost:8080"); 
        ws.OnOpen += OnConnected;
        ws.OnClose += OnDisconnected;
        ws.OnError += OnError;
        ws.Connect();

        // Ajouter des listener aux boutons
        sendButton.onClick.AddListener(SendComment);
        logoutButton.onClick.AddListener(Logout);

        // Charger les commentaires existants
        List<string> userComments = LoadComments(loggedInUser);
        foreach (string comment in userComments)
        {
            AddCommentToUI(comment);
        }
    }

    void Logout()
    {
        // Supprimer les informations de l'utilisateur connect�
        PlayerPrefs.DeleteKey("LoggedInUser");
        Debug.Log("D�connexion r�ussie. Retour � l'�cran Register.");
        SceneManager.LoadScene("Register"); // Retourner � la sc�ne Register
    }
    void SendComment()
    {
        if (ws == null || !ws.IsAlive)
        {
            responseText.text = "Connexion perdue. R�essayez.";
            return;
        }

        string comment = commentInput.text;
        if (string.IsNullOrEmpty(comment))
        {
            responseText.text = "Veuillez entrer un commentaire.";
            return;
        }

        // Envoyer le commentaire via WebSocket
        ws.Send(comment);
        SaveComment(loggedInUser, comment);

        // Ajouter le commentaire � l'interface utilisateur
        AddCommentToUI(comment);

        responseText.text = "Commentaire envoy� !";
        commentInput.text = ""; // Effacer le champ de saisie
    }

    void SaveComment(string username, string comment)
    {
        using (var connection = new SqliteConnection("URI=file:" + Application.persistentDataPath + "/userDatabase.db"))
        {
            connection.Open();

            string insertCommentQuery = "INSERT INTO comments (username, comment) VALUES (@username, @comment)";
            using (var command = new SqliteCommand(insertCommentQuery, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@comment", comment);
                command.ExecuteNonQuery();
            }
        }

        Debug.Log("Commentaire sauvegard� : " + comment);
    }

    List<string> LoadComments(string username)
    {
        List<string> comments = new List<string>();

        using (var connection = new SqliteConnection("URI=file:" + Application.persistentDataPath + "/userDatabase.db"))
        {
            connection.Open();

            string selectCommentsQuery = "SELECT comment FROM comments WHERE username = @username ORDER BY rowid DESC";
            using (var command = new SqliteCommand(selectCommentsQuery, connection))
            {
                command.Parameters.AddWithValue("@username", username);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comments.Add(reader["comment"].ToString());
                    }
                }
            }
        }

        return comments;
    }

    void AddCommentToUI(string comment)
    {
        // Instancier le pr�fab pour afficher le commentaire
        GameObject commentGO = Instantiate(commentPrefab, commentContainer);
        TMP_Text commentText = commentGO.GetComponentInChildren<TMP_Text>();

        if (commentText != null)
        {
            commentText.text = comment;
        }
    }

    void OnConnected(object sender, EventArgs e)
    {
        Debug.Log("Connect� au serveur WebSocket.");
        responseText.text = "Connect�.";
    }

    void OnDisconnected(object sender, CloseEventArgs e)
    {
        Debug.Log("D�connect� du serveur WebSocket.");
        responseText.text = "D�connect�.";
    }

    void OnError(object sender, WebSocketSharp.ErrorEventArgs e)
    {
        Debug.LogError("Erreur WebSocket : " + e.Message);
        responseText.text = "Erreur : " + e.Message;
    }

    void OnDestroy()
    {
        // Fermer la connexion WebSocket proprement
        if (ws != null)
        {
            ws.Close();
        }
    }
}
