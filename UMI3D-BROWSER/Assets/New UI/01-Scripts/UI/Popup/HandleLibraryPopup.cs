using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandleLibraryPopup : MonoBehaviour
{
    public GameObject model;
    public List<GameObject> worldList;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject go in worldList)
        {
            Instantiate(model);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
