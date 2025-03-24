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
        connectionString = "URI=file:" + Application.persistentDataPath + "/userDatabase.db"; // Chemin vers base de donn�es SQLite
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
            if (CompareTag("MissionItem"))
            {
                AddItemToDatabase();
                if (CheckItemCount() >= 6)
                {
                    // Si le joueur a plus de 6 items, on change de sc�ne
                    SceneManager.LoadScene("SecondCutScene");
                    DeleteItemsFromDatabase();
                    return;
                }
        
                Destroy(gameObject); // D�truire l'objet apr�s avoir ajout� � la base de donn�es
            }
        }
    }

    private void AddItemToDatabase()
    {
        string username = PlayerPrefs.GetString("LoggedInUser");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Aucun utilisateur connect�.");
            return;
        }

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            // Requ�te pour ins�rer l'objet dans la table 'items'
            string insertQuery = "INSERT INTO items (username, item_name) VALUES (@username, @itemName)";

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

                
                command.ExecuteNonQuery();
            }

            dbConnection.Close();
        }
    }

    private int CheckItemCount()
    {
        int itemCount = 0;
        string username = PlayerPrefs.GetString("LoggedInUser");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Aucun utilisateur connect�.");
            return 0;
        }

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

           
            string countQuery = "SELECT COUNT(*) FROM items WHERE username = @username";

            using (IDbCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = countQuery;

                var usernameParam = command.CreateParameter();
                usernameParam.ParameterName = "@username";
                usernameParam.Value = username;
                command.Parameters.Add(usernameParam);

                
                itemCount = int.Parse(command.ExecuteScalar().ToString());
            }

            dbConnection.Close();
        }

        return itemCount;
    }


    private void DeleteItemsFromDatabase()
    {
        string username = PlayerPrefs.GetString("LoggedInUser");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Aucun utilisateur connect�.");
            return;
        }

        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            // Requ�te pour supprimer les items du joueur
            string deleteQuery = "DELETE FROM items WHERE username = @username";

            using (IDbCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;

                var usernameParam = command.CreateParameter();
                usernameParam.ParameterName = "@username";
                usernameParam.Value = username;
                command.Parameters.Add(usernameParam);

                // Ex�cution de la commande pour supprimer les items
                command.ExecuteNonQuery();
            }

            dbConnection.Close();
        }
    }

}
