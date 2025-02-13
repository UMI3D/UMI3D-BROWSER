using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace umi3d.VRBase.lbe
{
    public class LBEDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_InputField urlField; // Champ texte
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
            //GuardianManager.Instance.ProcessIDSubmission(idValue);

            //Debug.Log("ID soumis : " + idValue);
        }
    }
}
