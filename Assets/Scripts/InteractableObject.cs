using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.SceneManagement;

public class InteractableObject : MonoBehaviour
{
    public bool playerInRange;
    public string ItemName;

    private string connectionString;

    void Start()
    {
        connectionString = "URI=file:" + Application.persistentDataPath + "/userDatabase.db"; // Chemin vers base de données SQLite
    }

    public string GetItemName()
    {
        return ItemName;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange)
        {
            string currentTag = gameObject.tag; // Récupérer le tag de l'objet
            AddItemToDatabase(); // Ajouter dans la base de données avec le tag

            int countMission1 = CheckItemCount("MissionItem"); // Nombre d'objets de mission 1
            int countMission2 = CheckItemCount("SecondMission"); // Nombre d'objets de mission 2

            if (countMission1 >= 6)
            {
                SceneManager.LoadScene("SecondMissionScene"); // Passer à la mission 2
                DeleteItemsFromDatabase("MissionItem");
                return;
            }
            else if (countMission2 >= 4)
            {
                SceneManager.LoadScene("SecondCutScene"); // Passer à la cut scene 2
                DeleteItemsFromDatabase("SecondMission");
                return;
            }

            Destroy(gameObject); // Détruire l’objet après collecte
        }
    }



    private void AddItemToDatabase()
    {
        string username = PlayerPrefs.GetString("LoggedInUser");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Aucun utilisateur connecté.");
            return;
        }

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            // Insérer l'objet avec son tag
            string insertQuery = "INSERT INTO items (username, item_name, tag) VALUES (@username, @itemName, @tag)";

            using (IDbCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = insertQuery;

                var usernameParam = command.CreateParameter();
                usernameParam.ParameterName = "@username";
                usernameParam.Value = username;
                command.Parameters.Add(usernameParam);

                var itemNameParam = command.CreateParameter();
                itemNameParam.ParameterName = "@itemName";
                itemNameParam.Value = ItemName;
                command.Parameters.Add(itemNameParam);

                var tagParam = command.CreateParameter();
                tagParam.ParameterName = "@tag";
                tagParam.Value = tag; // Ajout du tag (MissionItem, SecondMission)
                command.Parameters.Add(tagParam);

                command.ExecuteNonQuery();
            }

            dbConnection.Close();
        }
    }


    private int CheckItemCount(string tagType)
    {
        int itemCount = 0;
        string username = PlayerPrefs.GetString("LoggedInUser");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Aucun utilisateur connecté.");
            return 0;
        }

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            // Compter les objets avec le tag correspondant
            string countQuery = "SELECT COUNT(*) FROM items WHERE username = @username AND tag = @tagType";

            using (IDbCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = countQuery;

                var usernameParam = command.CreateParameter();
                usernameParam.ParameterName = "@username";
                usernameParam.Value = username;
                command.Parameters.Add(usernameParam);

                var tagParam = command.CreateParameter();
                tagParam.ParameterName = "@tagType";
                tagParam.Value = tagType;
                command.Parameters.Add(tagParam);

                itemCount = int.Parse(command.ExecuteScalar().ToString());
            }

            dbConnection.Close();
        }

        return itemCount;
    }



    private void DeleteItemsFromDatabase(string tagType)
    {
        string username = PlayerPrefs.GetString("LoggedInUser");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Aucun utilisateur connecté.");
            return;
        }

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            // Supprimer seulement les objets du type donné
            string deleteQuery = "DELETE FROM items WHERE username = @username AND tag = @tagType";

            using (IDbCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;

                var usernameParam = command.CreateParameter();
                usernameParam.ParameterName = "@username";
                usernameParam.Value = username;
                command.Parameters.Add(usernameParam);

                var tagParam = command.CreateParameter();
                tagParam.ParameterName = "@tagType";
                tagParam.Value = tagType;
                command.Parameters.Add(tagParam);

                command.ExecuteNonQuery();
            }

            dbConnection.Close();
        }
    }
}
