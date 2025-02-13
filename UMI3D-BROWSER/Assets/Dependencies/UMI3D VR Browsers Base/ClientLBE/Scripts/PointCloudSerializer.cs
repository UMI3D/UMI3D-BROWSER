using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.XR.ARFoundation;

public class PointCloudSerializer : MonoBehaviour
{
    public ARPointCloudManager pointCloudManager;

    // Sérialise le pointCloud pour le sauvegarder en Json
    public void SerializePointCloudsToJson(string filePath)
    {
        if (pointCloudManager == null)
        {
            Debug.LogError("ARPointCloudManager is not assigned!");
            return;
        }

        // créer la liste pour stocker le pointCloud
        List<SerializedPointCloud> serializedPointClouds = new List<SerializedPointCloud>();

        // parcours tous les points disponible dans le pointCloudManager
        foreach (var pointCloud in pointCloudManager.trackables)
        {
            SerializedPointCloud serializedPointCloud = new SerializedPointCloud();

            // Copie les données souhaités du pointCloud à serializedPointCloud
            serializedPointCloud.trackableId = pointCloud.trackableId.ToString();
            serializedPointCloud.positions = new List<SerializedVector3>();

            foreach (var position in pointCloud.positions.Value)
            {
                serializedPointCloud.positions.Add(new SerializedVector3(position));
            }
            serializedPointClouds.Add(serializedPointCloud);
        }

        string json = JsonUtility.ToJson(new SerializableList<SerializedPointCloud>(serializedPointClouds));
        File.WriteAllText(filePath, json);

        Debug.Log("Point cloud data serialized and saved to: " + filePath);
    }

    // Classe auxiliaire pour la sérialisation de Vector3
    [System.Serializable]
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

    // Classe auxiliaire pour la sérialisation de ARPointCloud
    [System.Serializable]
    public class SerializedPointCloud
    {
        public string trackableId;
        public List<SerializedVector3> positions;
    }

    // Classe auxiliaire pour la sérialisation de items
    [System.Serializable]
    public class SerializableList<T>
    {
        public List<T> items;

        public SerializableList(List<T> list)
        {
            items = list;
        }
    }
}
