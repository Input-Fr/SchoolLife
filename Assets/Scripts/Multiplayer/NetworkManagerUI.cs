using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]private Button clientBtn;
    [SerializeField]private Button QuickJoinBtn;

    [SerializeField]private GameObject inputField;
        
    private void Awake()
    {
        clientBtn.onClick.AddListener(() => { 
            string catchInput = inputField.GetComponent<TextMeshProUGUI>().text;
            catchInput = catchInput.Substring(0, 6);
            Relay.Instance.JoinRelay(catchInput);
            gameObject.SetActive(false);
            
        });
        QuickJoinBtn.onClick.AddListener(() =>
        {
            Relay.Instance.QuickJoinLobby();
            gameObject.SetActive(false);
        });
    }
}
