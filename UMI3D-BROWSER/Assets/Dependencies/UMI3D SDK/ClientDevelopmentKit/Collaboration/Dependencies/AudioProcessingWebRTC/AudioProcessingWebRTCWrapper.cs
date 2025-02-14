/*
Copyright 2019 - 2025 Inetum

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

using System;
using System.Runtime.InteropServices;

public static class AudioProcessingWebRTCWrapper
{
    public enum NoiseReductionLevel { Low, Moderate, High, VeryHigh };

    private static IntPtr audioProcessor = IntPtr.Zero;

    [DllImport("AudioProcessingWebRTC.dll")]
    private static extern IntPtr InitAudioProcessing(int sampleRate, int nbOfChannel, bool noiseReduction, int noiseReductionLevel, bool echoCancellation);

    [DllImport("AudioProcessingWebRTC.dll")]
    private static extern void ProcessAudio(IntPtr audioProcessor, int nbSamples, short[] inputSamples, short[] echoSamples, short[] outputSamples);

    public static void Init(int sampleRate, int nbOfChannel, bool noiseReduction, NoiseReductionLevel noiseReductionLevel, bool echoCancellation)
    {
        if (audioProcessor != IntPtr.Zero)
            UnityEngine.Debug.LogError("AudioProcessingWebRTCWrapper already init");

        audioProcessor = InitAudioProcessing(sampleRate, nbOfChannel, noiseReduction, (int)noiseReductionLevel, echoCancellation);
    }

    public static void ProcessAudio(int nbSamples, short[] inputSamples, short[] echoSamples, short[] outputSamples)
    {
        if (audioProcessor == IntPtr.Zero)
            throw new Exception("AudioProcessingWebRTCWrapper not init");

        ProcessAudio(audioProcessor, nbSamples, inputSamples, echoSamples, outputSamples);
    }
}

