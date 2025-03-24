using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Mono.Data.Sqlite;
using System.IO;
using Unity.VisualScripting.Dependencies.Sqlite;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button registerButton;
    public Button goToLoginButton;

    private string dbPath;

    void Start()
    {
        // Chemin de la base de donn�es
        dbPath = "URI=file:" + Application.persistentDataPath + "/userDatabase.db";

        // Cr�er la base de donn�es si elle n'existe pas
        if (!File.Exists(Application.persistentDataPath + "/userDatabase.db"))
        {
            CreateDatabase();
        }

        // Ajouter des listeners aux boutons
        registerButton.onClick.AddListener(RegisterUser);
        goToLoginButton.onClick.AddListener(GoToLoginScene);
    }

    void GoToLoginScene()
    {
        SceneManager.LoadScene("Login");
    }

    void CreateDatabase()
    {
        // Cr�er la base de donn�es SQLite et la table users
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string createTableQuery = @"CREATE TABLE IF NOT EXISTS users (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            username TEXT NOT NULL UNIQUE,
                                            password TEXT NOT NULL
                                        );";

            using (var command = new SqliteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }


            string createStatsTableQuery = @"CREATE TABLE IF NOT EXISTS stats (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            username TEXT NOT NULL,
                                            score INTEGER DEFAULT 0,
                                            level INTEGER DEFAULT 1,
                                            FOREIGN KEY (username) REFERENCES users (username)
                                        );";
            using (var command = new SqliteCommand(createStatsTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            string createItemsTableQuery = @"CREATE TABLE IF NOT EXISTS items (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    username TEXT NOT NULL,
                                    item_name TEXT NOT NULL,
                                    date_collected DATETIME DEFAULT CURRENT_TIMESTAMP,
                                    FOREIGN KEY (username) REFERENCES users (username)
                                );";

            using (var command = new SqliteCommand(createItemsTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }


            // Cr�ation de la table des commentaires
            string createCommentsTableQuery = @"CREATE TABLE IF NOT EXISTS comments (
                                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                username TEXT NOT NULL,
                                                comment TEXT NOT NULL,
                                                timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                                                FOREIGN KEY (username) REFERENCES users (username)
                                            );";

            using (var command = new SqliteCommand(createCommentsTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }


            connection.Close();
        }
        Debug.Log("Base de donn�es cr��e : " + dbPath);
    }

    void RegisterUser()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.Log("Le nom d'utilisateur ou le mot de passe ne peut pas �tre vide.");
            return;
        }

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            // V�rifier si l'utilisateur existe d�j�
            string checkUserQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
            using (var checkCommand = new SqliteCommand(checkUserQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@username", username);
                long userExists = (long)checkCommand.ExecuteScalar();

                if (userExists > 0)
                {
                    Debug.Log($"Le nom d'utilisateur '{username}' existe d�j�.");
                    return;
                }
            }

            // Ins�rer le nouvel utilisateur
            string insertUserQuery = "INSERT INTO users (username, password) VALUES (@username, @password)";
            using (var insertCommand = new SqliteCommand(insertUserQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@username", username);
                insertCommand.Parameters.AddWithValue("@password", password);
                insertCommand.ExecuteNonQuery();
            }


            // Ajouter une entr�e correspondante dans la table stats
            string insertStatsQuery = "INSERT INTO stats (username, score, level) VALUES (@username, @score, @level)";
            using (var insertCommand = new SqliteCommand(insertStatsQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@username", username);
                insertCommand.Parameters.AddWithValue("@score", 0); // Score par d�faut
                insertCommand.Parameters.AddWithValue("@level", 1); // Niveau par d�faut
                insertCommand.ExecuteNonQuery();
            }

            connection.Close();
        }
        Debug.Log("Utilisateur enregistr� avec succ�s !");
        Debug.Log("Chemin de la base de donn�es : " + Application.persistentDataPath);
    }
}
