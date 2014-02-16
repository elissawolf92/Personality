using UnityEngine;
using System.Collections;
using System.IO;

public class EditDriveData : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string fileName = "drives.txt";
        string outFileName = "drivesOut.txt";
        StreamReader sr = new StreamReader(fileName);
        StreamWriter sw = new StreamWriter(outFileName);

        string[] content = File.ReadAllLines(fileName);

       
        for (int i = 0; i < content.Length; i++) {
            string[] tokens = content[i].Split('\t');

            sw.WriteLine("INSERT INTO driveDataRCTAMAN(userId, driveInd, speed, v0, v1, ti, texp, tval, t0, t1, hr, hf, squash, wb, wx, wt,wf, et, ef, d,  tr, tf, encSpr0, sinRis0, retAdv0, encSpr1, sinRis1, retAdv1, continuity, bias, armLX, armLY,	armLZ,	armRX,	armRY,	armRZ)");
            sw.Write("VALUES (");

            for (int j = 0; j < tokens.Length; j++) {
                sw.Write("'" + tokens[j] + "'");
                if (j < tokens.Length - 1)
                    sw.Write(",");
                
            }
            sw.WriteLine(");");



        }
        
        sw.Close();
        sr.Close();


        EditShapes();
	
	}

    void EditShapes() {
        string fileName = "shapes.txt";
        string outFileName = "shapesOut.txt";
        StreamReader sr = new StreamReader(fileName);
        StreamWriter sw = new StreamWriter(outFileName);

        string[] content = File.ReadAllLines(fileName);


        for (int i = 0; i < content.Length; i++) {
            string[] tokens = content[i].Split('\t');

            sw.WriteLine("INSERT INTO shapeDataRCTAMAN(userId, shapeInd, head, neck, spine, spine1, shouldersX, shouldersY, shouldersZ, claviclesX, claviclesY, claviclesZ, pelvisLX, pelvisRX, pelvisY, pelvisZ, kneeX, hipsX, toesX, spineLength)");
            sw.Write("VALUES (");

            for (int j = 0; j < tokens.Length; j++) {
                sw.Write("'" + tokens[j] + "'");
                if (j < tokens.Length - 1)
                    sw.Write(",");

            }
            sw.WriteLine(");");



        }

        sw.Close();
        sr.Close();
    }
	
	
}
