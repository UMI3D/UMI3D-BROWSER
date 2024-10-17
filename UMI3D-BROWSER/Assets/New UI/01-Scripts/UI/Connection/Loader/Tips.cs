using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Tips : ScriptableObject
{
    [Serializable]
    public class Data
    {
        [SerializeField] private string tip;
        [SerializeField] private Sprite sprite;

        public string Tip => tip;
        public Sprite Sprite => sprite;
    }

    [SerializeField] private List<Data> loadingTips;

    public List<Data> LoadingTips => loadingTips;
}