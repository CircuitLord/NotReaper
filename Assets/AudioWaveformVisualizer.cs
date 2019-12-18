using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NotReaper.Timing;
using System.Collections.Generic;
using NotReaper.Models;
using NotReaper;

public class AudioWaveformVisualizer : MonoBehaviour {
    public GameObject waveformSegmentInstance;

    static uint NumQuarterNotesSampled = 4;

    static UInt64 texturePerTickDuration = NumQuarterNotesSampled;
    static UInt64 PixelsPerQuarterNote = 128;
    static UInt64 SecondsPerTexture = 16;

    public bool visible = false;

    struct GenerationSections {
        public float start;
        public float end;
    };

    void Update() {
        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>(true)) {
            r.enabled = visible;
        }
    }


    public void GenerateWaveform(ClipData aud, NotReaper.Timeline timeline) {
        StopAllCoroutines();

        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }

        if(timeline.tempoChanges.Count == 0) {
            return;
        }

        //Ensure all timestamps are different, otherwise texture generation is very crashy
        QNT_Timestamp lastTime = timeline.tempoChanges[0].time;
        for(int i = 1; i < timeline.tempoChanges.Count; ++i) {
            if(lastTime == timeline.tempoChanges[i].time) {
                return;
            }

            lastTime = timeline.tempoChanges[i].time;
        }

        var tempoChanges = timeline.tempoChanges;
        int nextTempoChange = 1;

        List<GenerationSections> sections = new List<GenerationSections>();
        for(float t = 0; t < aud.length;) {
            float endOfSection = t + SecondsPerTexture;
            if(endOfSection > aud.length) {
                endOfSection = aud.length;
            }

            if(nextTempoChange < tempoChanges.Count) {
                float nextTempoChangeSec = timeline.TimestampToSeconds(tempoChanges[nextTempoChange].time);
                
                if(endOfSection >= nextTempoChangeSec) {
                    endOfSection = nextTempoChangeSec;
                    ++nextTempoChange;
                }
            }

            GenerationSections sec;
            sec.start = t;
            sec.end = endOfSection;
            sections.Add(sec);

            t = endOfSection;
        }

        foreach(GenerationSections gen in sections) {
            int sampleStart = (int)(gen.start * aud.frequency * aud.channels);
            int sampleEnd = (int)(gen.end * aud.frequency * aud.channels);

            QNT_Timestamp startTick = timeline.ShiftTick(new QNT_Timestamp(0), gen.start);
            UInt64 microsecondsPerQuarterNote = timeline.tempoChanges[timeline.GetCurrentBPMIndex(startTick)].microsecondsPerQuarterNote;

            float beatTime = Conversion.ToQNT(gen.end - gen.start, microsecondsPerQuarterNote).ToBeatTime();

            StartCoroutine(PaintWaveformSpectrum(aud.samples, sampleStart, sampleEnd - sampleStart, (int)(beatTime * PixelsPerQuarterNote), 64, NRSettings.config.waveformColor,
                delegate (Texture2D tex) {
                    GameObject obj = GameObject.Instantiate(waveformSegmentInstance, new Vector3(0,0,0), Quaternion.identity, gameObject.transform);
                    QNT_Timestamp start = timeline.ShiftTick(new QNT_Timestamp(0), gen.start);
                    QNT_Timestamp end = timeline.ShiftTick(new QNT_Timestamp(0), gen.end);

                    obj.GetComponent<MeshFilter>().mesh = CreateMesh(start.ToBeatTime(), new QNT_Duration((UInt64)(end.tick - start.tick)).ToBeatTime(), 1);
                    obj.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", tex);
                    obj.GetComponent<MeshRenderer>().enabled = false;
                    obj.GetComponent<Transform>().localPosition = new Vector3(0, -0.5f ,0);
                    obj.GetComponent<Transform>().localScale = new Vector3(1.0f, 1, 1);
                }
            ));
        }
    }

    public Mesh CreateMesh(float startX, float width, float height) {
        var mesh = new Mesh();

        var vertices = new Vector3[4]
        {
            new Vector3(startX, 0, 0),
            new Vector3(startX + width, 0, 0),
            new Vector3(startX, height, 0),
            new Vector3(startX + width, height, 0)
        };
        mesh.vertices = vertices;

        var tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        var normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        var uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        return mesh;
    }

    public delegate void TextureCallback(Texture2D data);

    IEnumerator PaintWaveformSpectrum(float[] samples, int sampleStart, int sampleSection, int width, int height, Color col, TextureCallback cb) {
     Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

     //Set all to clear
     Color32 resetColor = new Color32(255, 255, 255, 0);
     Color32[] resetColorArray = tex.GetPixels32();
     for (int i = 0; i < resetColorArray.Length; i++) {
         resetColorArray[i] = resetColor;
     }
     tex.SetPixels32(resetColorArray);
     float sampleIncr = sampleSection / (float)width;

     const int PIXELS_PER_YIELD = 32; //Waits every 400,000 loop
     int loopCounter = 0;
 
     for (int x = 0; x < width; x++) {
        float maxValue = 0;
        float avgValue = 0;
        for (int s = 0; s < sampleIncr; s++) {
            float sampleIdx = sampleStart + x * sampleIncr + s;
            int idx = Math.Min((int)sampleIdx, samples.Length - 1);
            float sampleVal = Math.Abs(samples[idx]);
            avgValue += sampleVal * sampleVal;
            maxValue = Math.Max(maxValue, sampleVal * sampleVal);
        }
        avgValue /= sampleIncr;
        
        //For sections with huge peaks, just use avg
        if(maxValue > 0.5f) {
            maxValue = avgValue;
        }

        float paintHeight  = Mathf.Sqrt(maxValue) * height;
        for (int y = 0; y <= paintHeight; y++) {
             tex.SetPixel(x, ( height / 2 ) + y, col);
             tex.SetPixel(x, ( height / 2 ) - y, col);
        }

        ++loopCounter;
        if(loopCounter % PIXELS_PER_YIELD == 0) {
            yield return null;
        }
     }
     tex.Apply();
 
     cb(tex);
    }

}