using UnityEngine;

using System;
using System.Collections;

public class DeviceAudioListener : MonoBehaviour
{
    bool m_ListenerRegistered = false;
    AudioFocusListener m_FocusListener;

    class AudioFocusListener : AndroidJavaProxy
    {
        public AudioFocusListener() : base("android.media.AudioManager$OnAudioFocusChangeListener") { }

        public bool m_HasAudioFocus = true;

        public bool HasAudioFocus { get { return m_HasAudioFocus; } }

        public void onAudioFocusChange(int focus)
        {
            m_HasAudioFocus = (focus >= 0);

            UnityEngine.Debug.Log("OnAudioFocusChange " + m_HasAudioFocus);

            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject audioManager = activity.Call<AndroidJavaObject>("getSystemService", "audio");

            int res = audioManager.Call<Int32>("requestAudioFocus", this, 3, 1);

            UnityEngine.Debug.Log("RE REQUEST AUDIO FOCUS " + res + " ->  " + HasAudioFocus);
        }

        public string toString()
        {
            return "MyAwesomeAudioListener";
        }
    }

    void Awake()
    {
        if (m_FocusListener == null)
            m_FocusListener = new AudioFocusListener();

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject audioManager = activity.Call<AndroidJavaObject>("getSystemService", "audio");

        int res = audioManager.Call<Int32>("requestAudioFocus", m_FocusListener, 3, 1);

        UnityEngine.Debug.Log("REquest AUDIO FOCUS " + res);
    }

    void FixedUpdate()
    {
        //AudioListener.pause = !m_FocusListener.HasAudioFocus;
    }
}