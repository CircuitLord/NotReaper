using System.Collections;
using UnityEngine;

public class AudioWaveformVisualizer : MonoBehaviour
{
    public float saturation = 1;

    public Color colour = Color.white;
    public AudioSource aud;
    
    Texture2D texture;
    float[] samples;

    public void Init()
    {
        GetComponent<Renderer>().material.mainTexture = PaintWaveformSpectrum(Mathf.CeilToInt(aud.clip.length * 50), 50);
    }

    public Texture2D PaintWaveformSpectrum(int width, int height)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        float[] samples = new float[aud.clip.samples];
        float[] waveform = new float[width];
        aud.clip.GetData(samples, 0);
        int packSize = (aud.clip.samples / width) + 1;
        int s = 0;
        float max = 0f;
        for (int i = 0; i < aud.clip.samples; i += packSize)
        {
            waveform[s] = Mathf.Abs(samples[i]);
            if (waveform[s] > max) max = waveform[s];
            s++;
        }

        for (int i = 0; i < width; i++)
        {
            waveform[i] /= (max * saturation);
            if (waveform[i] > 1f)
                waveform[i] = 1f;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, Color.black);
            }
        }

        for (int x = 0; x < waveform.Length; x++)
        {
            for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++)
            {
                tex.SetPixel(x, (height / 2) + y, colour);
                tex.SetPixel(x, (height / 2) - y, colour);
            }
        }
        tex.Apply();

        return tex;
    }

}