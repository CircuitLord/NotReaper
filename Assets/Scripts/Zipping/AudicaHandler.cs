using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Compression;
using System.IO;
using System;
using System.Text;

public class AudicaHandler : MonoBehaviour {

    public static void MoggToOgg(string path) {

        byte[] oggStartLocation = new byte[4];

        using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
        {
            reader.BaseStream.Seek(4, SeekOrigin.Begin);
            reader.Read(oggStartLocation, 0, 4);
        }

        print(BitConverter.ToString(oggStartLocation));
        //int oggStartIndex = Array.IndexOf(File.ReadAllBytes(path), )
        long final = 0;
        final |= (oggStartLocation[0]);
        final |= (oggStartLocation[1] << 8);
        final |= (oggStartLocation[2] << 16);
        final |= (oggStartLocation[3] << 24);

        print(final);
 
        

        byte[] src = File.ReadAllBytes(path);
        
        byte[] dst = new byte[src.Length - final];
        Array.Copy(src, final, dst, 0, dst.Length);
        File.WriteAllBytes(@"C:\Files\GameStuff\AUDICACustom\Testing\test.ogg", dst);
    }



    public void Update() {

        if(Input.GetKeyDown(KeyCode.K)) {
            MoggToOgg(@"C:\Files\GameStuff\AUDICACustom\Testing\song.mogg");
        }
    }

    
}
