/*
Copyright 2019 - 2024 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using TMPro;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UI;

public class SocialElement : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text placeText;
    [SerializeField] private Slider volumeSlider;

    public UMI3DUser User { 
        get => _user;
        set {
            _user = value;
            Update();
        }
    }
    public string UserName => _user.login;
    public float UserVolume { 
        get => _volume; 
        set {
            _volume = value;
            volumeSlider.SetValueWithoutNotify(value);

            var vg = UserVolumeToVG(value);
            AudioManager.Instance.SetGainForUser(_user, vg.gain);
            AudioManager.Instance.SetVolumeForUser(_user, vg.volume);
        }
    }

    private UMI3DUser _user;
    private float _volume = 100;

    private const float logBase = 1.5f;
    private const float factor = 5f / 2f;
    private const float factor2 = 5f / 2f;

    private void Awake()
    {
        volumeSlider.minValue = 0;
        volumeSlider.maxValue = 100;
        volumeSlider.onValueChanged.AddListener(newValue => {
            _volume = newValue;
        });
    }

    private void Update()
    {
        nameText.text = UserName;
        placeText.text = $"({UMI3DCollaborationClientServer.Environement.name})";
    }

    private (float volume, float gain) UserVolumeToVG(float volume)
    {
        if (volume <= 100f)
            return (volume / 100f, 1);
        else
            return (1, GainFactor(volume / 100));
    }
    private float GainFactor(float gain) { return (Mathf.Pow(logBase, (gain - 1) * factor) - 1) * factor2 + 1; }
}
