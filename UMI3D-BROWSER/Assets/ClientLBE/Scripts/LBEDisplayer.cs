using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.displayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace ClientLBE
{
    public class LBEDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_UMI3DUIInputField urlField; // Champ texte
        [SerializeField] private Button submitButton; // Bouton de validation


        void Start()
        {
            submitButton.onClick.AddListener(SubmitIDToGuardianManager);
        }

        private void SubmitIDToGuardianManager()
        {
            // Récupère le contenu du champ texte
            string idValue = urlField.text.Trim();

            Debug.LogWarning("idValue -> " + idValue);


            // Vérifie si l'entrée est valide
            if (string.IsNullOrEmpty(idValue))
            {
                Debug.LogWarning("Le champ ID est vide !");
                return;
            }

            // Appelle la méthode du GuardianManager
            GuardianManager.Instance.ProcessIDSubmission(idValue);

            Debug.Log("ID soumis : " + idValue);
        }
    }
}
