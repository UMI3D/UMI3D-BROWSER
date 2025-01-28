using ClientLBE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubmitLBEGroupId : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;


    private void Start()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnTextChanged);
        }
        //A enlever aprés la démo
        inputField.text = "1";

    }

    private void OnDestroy()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.RemoveListener(OnTextChanged);
        }
    }

    private void OnTextChanged(string ID)
    {
        if (GuardianManager.Instance != null)
        {
            GuardianManager.Instance.ProcessIDSubmission(ID);
        }
    }
}
