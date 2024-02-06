using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.utils;
using UnityEngine;

namespace umi3dBrowsers.keyboard
{
    public class ElasticKeyBoardInitializer : MonoBehaviour
    {
        [SerializeField] private ElasticFollow elasticFollow;

        private void Awake()
        {
            if (elasticFollow == null) elasticFollow = GetComponentInParent<ElasticFollow>();
        }

        private void Start()
        {
            StartCoroutine(SetParent());
        }

        IEnumerator SetParent()
        {
            Camera camera = Camera.main;

            while (camera == null) 
            { 
                yield return new WaitForSeconds(0.5f);
                camera = Camera.main;
            }

            if (camera != null)
            {
                elasticFollow.SetTarget(camera.transform);
            }
            else
                Debug.Log("Something went wrong there should be a  main camera");
        }
    }
}
