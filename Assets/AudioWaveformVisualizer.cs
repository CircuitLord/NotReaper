using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioWaveformVisualizer : MonoBehaviour {
    public float saturation = 1;

    public Color colour = Color.white;
    public AudioSource aud;

    public GameObject WF;

    Texture2D texture;
    float[] samples;

    private bool formenabled;


    public Renderer Spectrogram2;

    public void Init() {
        GetComponent<Renderer>().material.mainTexture = PaintWaveformSpectrum(Mathf.CeilToInt(aud.clip.length * 25), 50);
        //Spectrogram2.material.mainTexture = PaintWaveformSpectrum(Mathf.CeilToInt(aud.clip.length * 50), 50, 0);
    }

    public Texture2D PaintWaveformSpectrum(int width, int height, int offset = 0) {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        float[] samples = new float[aud.clip.samples];
        float[] waveform = new float[width];
        aud.clip.GetData(samples, 0);
        int packSize = (aud.clip.samples / width) + 1;
        int s = 0;
        float max = 0f;
        for (int i = 0; i < aud.clip.samples; i += packSize) {
            waveform[s] = Mathf.Abs(samples[i]);
            if (waveform[s] > max) max = waveform[s];
            s++;
        }

        for (int i = 0; i < width; i++) {
            waveform[i] /= (max * saturation);
            if (waveform[i] > 1f)
                waveform[i] = 1f;
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                tex.SetPixel(x, y, Color.black);
            }
        }

        for (int x = offset; x < waveform.Length; x++) {
            for (int y = 0; y <= waveform[x] * ((float) height * .75f); y++) {
                tex.SetPixel(x, (height / 2) + y, colour);
                tex.SetPixel(x, (height / 2) - y, colour);
            }
        }
        tex.Apply();

        return tex;
    }

    public void settingsToggle(Toggle tog) {
        if (tog.isOn) {
            Init();
            WF.SetActive(true);
        } else {
            GetComponent<Renderer>().material.mainTexture = null;
            WF.SetActive(false);
        }
    }

}