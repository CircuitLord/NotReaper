using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NotReaper.Timing;
using System.Collections.Generic;
using NotReaper.Models;

public class AudioWaveformVisualizer : MonoBehaviour {
    public GameObject waveformSegmentInstance;

    static uint NumQuarterNotesSampled = 4;

    static UInt64 texturePerTickDuration = NumQuarterNotesSampled;
    static UInt64 PixelsPerQuarterNote = 64;
    static UInt64 SecondsPerTexture = 16;

    struct GenerationSections {
        public float start;
        public float end;
    };

    public void GenerateWaveform(ClipData aud, NotReaper.Timeline timeline) {
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
            GameObject obj = GameObject.Instantiate(waveformSegmentInstance, new Vector3(0,0,0), Quaternion.identity, gameObject.transform);

            int sampleStart = (int)(gen.start * aud.frequency * aud.channels);
            int sampleEnd = (int)(gen.end * aud.frequency * aud.channels);

            QNT_Timestamp startTick = timeline.ShiftTick(new QNT_Timestamp(0), gen.start);
            UInt64 microsecondsPerQuarterNote = timeline.tempoChanges[timeline.GetCurrentBPMIndex(startTick)].microsecondsPerQuarterNote;

            Texture2D tex = PaintWaveformSpectrum(aud.samples, sampleStart, sampleEnd - sampleStart, (int)(Conversion.ToQNT(gen.end - gen.start, microsecondsPerQuarterNote).ToBeatTime() * PixelsPerQuarterNote), 64, Color.yellow);

            QNT_Timestamp start = timeline.ShiftTick(new QNT_Timestamp(0), gen.start);
            QNT_Timestamp end = timeline.ShiftTick(new QNT_Timestamp(0), gen.end);

            obj.GetComponent<MeshFilter>().mesh = CreateMesh(start.ToBeatTime(), new QNT_Duration((UInt64)(end.tick - start.tick)).ToBeatTime(), 1);
            obj.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", tex);
            obj.GetComponent<Transform>().localPosition = new Vector3(0, -0.5f ,0);
            obj.GetComponent<Transform>().localScale = new Vector3(1.0f, 1, 1);
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

    public Texture2D PaintWaveformSpectrum(float[] samples, int sampleStart, int sampleSection, int width, int height, Color col) {
     Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

     //Set all to clear
     Color32 resetColor = new Color32(255, 255, 255, 0);
     Color32[] resetColorArray = tex.GetPixels32();
     for (int i = 0; i < resetColorArray.Length; i++) {
         resetColorArray[i] = resetColor;
     }
     tex.SetPixels32(resetColorArray);
     int sampleIncr = sampleSection / width;

     //float maxHeight = 0;
     //for (int x = 0; x < width; x++) {
     //   float currentHeight = Math.Abs(samples[sampleStart + x * sampleIncr]);
     //   maxHeight = Math.Max(currentHeight, maxHeight);
     //}

     float currentHeight = 0.0f;
 
     for (int x = 0; x < width; x++) {
        float sampleHeight = Math.Abs(samples[sampleStart + x * sampleIncr]);
        currentHeight = sampleHeight * 0.8f + currentHeight * 0.2f;
        float paintHeight  = currentHeight * ((float)height * .75f);
        for (int y = 0; y <= paintHeight; y++) {
             tex.SetPixel(x, ( height / 2 ) + y, col);
             tex.SetPixel(x, ( height / 2 ) - y, col);
        }
     }
     tex.Apply();
 
     return tex;
    }

}