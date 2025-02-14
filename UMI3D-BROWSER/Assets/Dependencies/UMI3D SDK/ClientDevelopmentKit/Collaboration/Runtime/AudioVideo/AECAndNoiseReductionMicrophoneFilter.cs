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

//using CSCore;
//using CSCore.Codecs.WAV;
using CSCore.SoundIn;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.cdk.collaboration
{
    public struct MicrophoneFilterSettings
    {
        public int sampleRate;

        public int channels;
    }

    /// <summary>
    /// A filter which perfoms an acoustic echo cancellation and noise reduction based on WebRTC Voice Engine.
    /// </summary>
    public class AECAndNoiseReductionMicrophoneFilter : IMicrophoneFilter, IDisposable
    {
        private const int MAX_ECHO_QUEUE_SIZE = 4096;

        #region Fields

        private bool enable = true;

        /// <inheritdoc/>
        public bool Enable
        {
            get => this.enable;

            set
            {
                this.enable = value;
                this.echoSamples.Clear();

                if (value)
                    this.capture.Start();
                else
                    this.capture.Stop();
            }
        }

        /// <summary>
        /// Audio played by the OS.
        /// </summary>
        private Queue<float> echoSamples = new();

        /// <summary>
        /// Recorder for audio system.
        /// </summary>
        private WasapiLoopbackCapture capture;

        /// <summary>
        /// Bytes per sample for <see cref="capture"/>
        /// </summary>
        private int bytesPerSample;

        /// <summary>
        /// Buffer used by <see cref="IMicrophoneFilter.ProcessAudio"/>.
        /// </summary>
        private short[] echoShortSamples;

        /// <summary>
        /// Buffer used by <see cref="IMicrophoneFilter.ProcessAudio"/>.
        /// </summary>
        private short[] micShortSamples;

        /// <summary>
        /// Buffer used by <see cref="IMicrophoneFilter.ProcessAudio"/>.
        /// </summary>
        private short[] outShortSamples;

        #endregion

        //public List<float> micWithoutProcess = new(), micWithProcess = new(), speaker = new();

        public AECAndNoiseReductionMicrophoneFilter(MicrophoneFilterSettings settings)
        {
            AudioProcessingWebRTCWrapper.Init(settings.sampleRate,
                settings.channels,
                true,
                AudioProcessingWebRTCWrapper.NoiseReductionLevel.VeryHigh,
                true);

            // we can't change WasapiLoopbackCapture format to mono sound, otherwise it fails
            this.capture = new WasapiLoopbackCapture(100, new(48000, 16, 2));
            this.capture.Initialize();
            this.bytesPerSample = capture.WaveFormat.BitsPerSample / 8;
            Debug.Assert(bytesPerSample == 2, "Only works if its a 16bits sound");
            capture.DataAvailable += RecordSystemAudio;
        }

        private void RecordSystemAudio(object sender, DataAvailableEventArgs e)
        {
            int sampleCount = e.ByteCount / bytesPerSample;

            lock(this.echoSamples)
            {
                for (int i = 0; i < sampleCount; i += capture.WaveFormat.Channels)
                {
                    short shortSample = BitConverter.ToInt16(e.Data, i * bytesPerSample);
                    float sample = shortSample / (float)(short.MaxValue);

                    if (this.echoSamples.Count > MAX_ECHO_QUEUE_SIZE)
                        this.echoSamples.Dequeue();

                    this.echoSamples.Enqueue(sample);
                    //this.speaker.Add(sample);
                }
            }
        }

        void IMicrophoneFilter.ProcessAudio(float[] samples)
        {
            if (!this.Enable)
                return;

            int bufferSize = samples.Length;

            if (echoShortSamples == null || echoShortSamples.Length != bufferSize)
                echoShortSamples = new short[bufferSize];

            if (micShortSamples == null || micShortSamples.Length != bufferSize)
                micShortSamples = new short[bufferSize];

            if (outShortSamples == null || outShortSamples.Length != bufferSize)
                outShortSamples = new short[bufferSize];

            // 1. Convert samples to short samples and get echo samples.
            lock (this.echoSamples)
            {
                for (int i = 0; i < bufferSize; i++)
                {
                    micShortSamples[i] = (short)(samples[i] * short.MaxValue);

                    if (this.echoSamples.Count > 0)
                        echoShortSamples[i] = (short)(this.echoSamples.Dequeue() * short.MaxValue);
                    else
                        echoShortSamples[i] = 0;
                }
            }

            //micWithoutProcess.AddRange(samples);

            // 2. Process audio
            AudioProcessingWebRTCWrapper.ProcessAudio(bufferSize, micShortSamples, echoShortSamples, outShortSamples);

            // 3. Convert output to float.
            for (int i = 0; i < bufferSize; i++)
            {
                samples[i] = (outShortSamples[i] / (float)short.MaxValue);
            }

            //micWithProcess.AddRange(samples);
        }

        void IDisposable.Dispose()
        {
            capture.Dispose();
        }

        public void Clear()
        {
            //micWithoutProcess.Clear();
            //micWithProcess.Clear();
            //speaker.Clear();
        }

        public void Save()
        {
            //string outputFilePath = @"C:\Users\frup77677\Downloads\mic-process.wav";
            //using (WaveWriter writer = new (outputFilePath, new WaveFormat(48000, 16, 1)))
            //{
            //    // Écrire les données audio dans le fichier .wav
            //    writer.WriteSamples(micWithProcess.ToArray(), 0, micWithProcess.Count);
            //}

            //Debug.Log("Write file " + outputFilePath);

            //outputFilePath = @"C:\Users\frup77677\Downloads\mic-no-process.wav";
            //using (WaveWriter writer = new (outputFilePath, new WaveFormat(48000, 16, 1)))
            //{
            //    // Écrire les données audio dans le fichier .wav
            //    writer.WriteSamples(micWithoutProcess.ToArray(), 0, micWithoutProcess.Count);
            //}

            //Debug.Log("Write file " + outputFilePath);

            //Debug.Log(micWithoutProcess.Count + " vs " + speaker.Count);

            //outputFilePath = @"C:\Users\frup77677\Downloads\mic-speakers.wav";
            //using (WaveWriter writer = new (outputFilePath, new WaveFormat(48000, 16, 1)))
            //{
            //    // Écrire les données audio dans le fichier .wav
            //    writer.WriteSamples(speaker.ToArray(), 0, speaker.Count);
            //}

            //int n = micWithoutProcess.Count;
            //short[] postprocess = new short[n];
            //float[] postProcessFloat = new float[n];
            //short[] mic = new short[n];
            //short[] echo = new short[n];

            //for (int i = 0; i < n; i++)
            //{
            //    postprocess[i] = (short)(speaker[i] * short.MaxValue);
            //    mic[i] = (short)(micWithoutProcess[i] * short.MaxValue);
            //    echo[i] = (short)(speaker[i] * short.MaxValue);
            //}

            //AudioProcessingWebRTCWrapper.ProcessAudio(n, mic, echo, postprocess);

            //for (int i = 0; i < n; i++)
            //{
            //    postProcessFloat[i] = postprocess[i]/(short)(short.MaxValue);

            //    if (i == 6790)
            //        postProcessFloat[i] = 1f;
            //}

            //outputFilePath = @"C:\Users\frup77677\Downloads\mic-post-process.wav";
            //using (WaveWriter writer = new(outputFilePath, new WaveFormat(48000, 16, 1)))
            //{
            //    // Écrire les données audio dans le fichier .wav
            //    writer.WriteSamples(postProcessFloat, 0, postProcessFloat.Length);
            //}

            //Debug.Log("Write file " + outputFilePath);
        }
    }
}