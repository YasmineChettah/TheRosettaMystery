using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{

    public GameObject interaction_Info_UI;
    private TMP_Text interaction_text;

    private void Start()
    {
        interaction_text = interaction_Info_UI.GetComponent<TMP_Text>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;
            NPC npc = selectionTransform.GetComponent<NPC>();

            if (npc && npc.playerInRange)
            {
                interaction_text.text = "Parler";
                interaction_Info_UI.SetActive(true);

                if (Input.GetMouseButtonDown(0) && npc.isTalkingWithPlayer == false)
                {
                    npc.StartConversation();
                }

                if(DialogSystem.Instance.dialogUIActive)
                {
                    interaction_Info_UI.SetActive(false);
                }
                return;
            }
            else
            {
                interaction_text.text = "";
                interaction_Info_UI.SetActive(false);
            }
            if (selectionTransform.GetComponent<InteractableObject>() && selectionTransform.GetComponent<InteractableObject>().playerInRange)
            {
                interaction_text.text = selectionTransform.GetComponent<InteractableObject>().GetItemName();
                interaction_Info_UI.SetActive(true);
            }
            else
            {
                interaction_Info_UI.SetActive(false);
            }

        }

        else
        {
            interaction_Info_UI.SetActive(false);
        }
    }
}