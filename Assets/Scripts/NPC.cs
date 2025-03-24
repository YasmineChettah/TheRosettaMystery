using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public bool playerInRange;

    public bool isTalkingWithPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
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

    public void StartConversation()
    {
        isTalkingWithPlayer = true;
        print("Conversation Started");

        DialogSystem.Instance.OpenDialogUI();
    }
}
