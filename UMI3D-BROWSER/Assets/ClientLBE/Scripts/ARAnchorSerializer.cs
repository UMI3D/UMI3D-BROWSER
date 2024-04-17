using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.XR.ARFoundation;
using TMPro;


namespace UnityEngine.XR.ARFoundation
{
    public class ARAnchorSerializer : MonoBehaviour
    {
        public ARAnchorManager anchorManager;
        private List<SerializedARAnchor> serializedAnchors = new List<SerializedARAnchor>();

        // S�rialise les ancres et les enregistre au format JSON
        public void SerializeAnchorsToJson(List<ARAnchor> anchors, string filePath)
        {
            serializedAnchors.Clear();

            try
            {
                // Parcours toutes les ancres fournies en argument
                foreach (var anchor in anchors)
                {
                    SerializedARAnchor serializedAnchor = new SerializedARAnchor();

                    // Copie les donn�es souhait�s de ARAnchor � SerializedARAnchor
                    serializedAnchor.trackableId = anchor.trackableId.ToString();
                    serializedAnchor.pose = new SerializedPose(anchor.transform.position, anchor.transform.rotation);

                    /*
                    // D�finit la propri�t� isGuardianAnchor en fonction de si l'ancre fait partie du guardian ou non
                    serializedAnchor.isGuardianAnchor = anchor.gameObject.CompareTag("GuardianAnchor");

                    if(anchor.gameObject.tag == "GuardianAnchor")
                    {
                        serializedAnchor.isGuardianAnchor = true;
                    }*/

                    // Ajoute l'ancre s�rialis�e � la liste
                    serializedAnchors.Add(serializedAnchor);
                }

                // S�rialise la liste en JSON et �crit les donn�es dans un fichier
                string json = JsonUtility.ToJson(new SerializableList<SerializedARAnchor>(serializedAnchors));
                File.WriteAllText(filePath, json);

                // Debug en paquetb de 1000 charract�res
                DisplayJsonDebug(json);

                Debug.Log("Anchor data serialized and saved to: " + filePath);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to serialize anchors to JSON file: " + filePath + ". Error: " + e.Message);
            }
        }

        //Debug du json pour la console meta limit� a environs 1025 chract�re pour pouvoir visualiser le json en entier
        public void DisplayJsonDebug(string jsonString)
        {
            int chunkSize = 1000;
            int length = jsonString.Length;

            for (int i = 0; i < length; i += chunkSize)
            {
                int endIndex = Mathf.Min(i + chunkSize, length);
                Debug.Log(jsonString.Substring(i, endIndex - i));
            }
        }

        public List<SerializedARAnchor> DeserializeAnchorsFromJson(string filePath)
        {
            List<SerializedARAnchor> deserializedAnchors = new List<SerializedARAnchor>();

            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);

                    // D�s�rialise le JSON en une liste d'ancres s�rialis�es
                    SerializableList<SerializedARAnchor> serializedList = JsonUtility.FromJson<SerializableList<SerializedARAnchor>>(json);

                    if (serializedList != null && serializedList.items != null)
                    {
                        // Ajoute les ancres d�s�rialis�es � la liste
                        deserializedAnchors.AddRange(serializedList.items);
                        Debug.LogWarning("Deserialize anchors from JSON file success : " + filePath);

                    }
                    else
                    {
                        Debug.LogError("Failed to deserialize anchors from JSON file: " + filePath);
                    }
                }
                else
                {
                    Debug.LogError("JSON file not found: " + filePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to deserialize anchors from JSON file: " + filePath + ". Error: " + e.Message);
            }

            return deserializedAnchors;
        }

        // M�thode pour r�cup�rer une ancre s�rialis�e � partir de son ID
        public SerializedARAnchor GetSerializedAnchor(string trackableId)
        {
            // Parcourir toutes les ancres s�rialis�es
            foreach (var serializedAnchor in serializedAnchors)
            {
                // V�rifier si l'ID de l'ancre s�rialis�e correspond � l'ID recherch�
                if (serializedAnchor.trackableId == trackableId)
                {
                    return serializedAnchor;
                }
            }

            // Retourner null si aucune ancre correspondante n'est trouv�e
            return null;
        }

        // Classe auxiliaire pour la s�rialisation de la pose
        [Serializable]
        public class SerializedPose
        {
            public SerializedVector3 position;
            public SerializedQuaternion rotation;

            public SerializedPose(Vector3 pos, Quaternion rot)
            {
                position = new SerializedVector3(pos);
                rotation = new SerializedQuaternion(rot);
            }
        }

        // Classe auxiliaire pour la s�rialisation de Vector3
        [Serializable]
        public class SerializedVector3
        {
            public float x;
            public float y;
            public float z;

            public SerializedVector3(Vector3 vector)
            {
                x = vector.x;
                y = vector.y;
                z = vector.z;
            }
        }

        // Classe auxiliaire pour la s�rialisation de Quaternion
        [Serializable]
        public class SerializedQuaternion
        {
            public float x;
            public float y;
            public float z;
            public float w;

            public SerializedQuaternion(Quaternion quaternion)
            {
                x = quaternion.x;
                y = quaternion.y;
                z = quaternion.z;
                w = quaternion.w;
            }
        }

        // Classe auxiliaire pour la s�rialisation de listes
        [Serializable]
        public class SerializableList<T>
        {
            public List<T> items;

            public SerializableList(List<T> list)
            {
                items = list;
            }
        }

        // Classe pour la s�rialisation de ARAnchor
        [Serializable]
        public class SerializedARAnchor
        {
            public string trackableId;
            public SerializedPose pose;
            public bool isGuardianAnchor;
        }
    }
}
