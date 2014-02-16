using UnityEngine;
using System.Collections;
using System.IO;
using Meta.Numerics;
using Meta.Numerics.Statistics;

enum MetricType{
    Posture,
    Weight,
    Space,
    Time,
    Shape,
    Effort, //for all effort parameters
    All
}

[RequireComponent (typeof (AnimationManager))]
public class MetricRecorder : MonoBehaviour {
    AnimationManager animMgr;
    Metrics metrics {
        get {			 			
			return animMgr.Metrics;				
		}
    }


	int keyCnt;
  
	// Use this for initialization
	void Start () {
        animMgr = GetComponent<AnimationManager>();
    
   //   if(animMgr.metricsComputed){
        
		
        RecordEffort("space");
        RecordEffort("spaceDirect");
        RecordEffort("weight");        
        RecordEffort("weightStrong");
        RecordEffort("time");
        RecordEffort("timeSudden");
        RecordEffort("flow");
        RecordEffort("flowBound");

               
        //Reorder files for evaluation
        ReorderFilesForStatAnalysis((int)MetricType.Effort);
 
    
        RecordPosture();

        RecordHistograms();

     
    //}
        
    //Always start running from the first animation in the clip array
      ComputeMetricsForAnim(animMgr.AnimNameArr[0]);
      animMgr.MetricsComputed = true;
      
        
	}

    string AnimNameToPersonality(string s) {
        string[] persTraitArr = {"O-", "O+", "C-", "C+","E-", "E+", "A-", "A+", "N-", "N+", "Ne"};
        for(int i  = 0 ; i < persTraitArr.Length; i++)
            if(s.Contains(persTraitArr[i]))
                return persTraitArr[i];

        return null;
    }

      string AnimNameTo2DPersonality(string s) {
        string[] persTraitArr = {"OE-", "OE+", "NCA-", "NCA+","Ne"};

        if (s.Contains("O-") || s.Contains("E-"))
            return "OE-";
        else if (s.Contains("O+") || s.Contains("E+"))
            return "OE+";
        else if (s.Contains("N-") || s.Contains("C-") || s.Contains("A-"))
            return "NCA-";
        else if (s.Contains("N+") || s.Contains("C+") || s.Contains("A+"))
            return "NCA+";         
        else if (s.Contains("Ne"))
            return "Ne";         
        else
            return null;
    }

    /// <summary>
    /// Computes metrics for animation s
    /// First changes the  animation to s 
    /// Called in recording methods
    /// </summary>
    void ComputeMetricsForAnim(string s)
    {
	    animMgr.ChangeAnim(s);                    
		keyCnt =  animMgr.Keys.Length -2 ; // -1 because keys 0 and keys.length-1 are the same, -2 because the last frame gives extreme results   
        animMgr.ComputeMetrics();
       
	}

    void RecordHistograms() {

         string fileName = "METRICS\\effortHistograms.txt";
        StreamWriter swE = new StreamWriter(fileName);
         swE.WriteLine("spaceR,spaceL,weightR,weightL,timeR,timeL,flowR,flowL,Class"); 
 
        fileName = "METRICS\\postureHistograms.txt";
        StreamWriter swP = new StreamWriter(fileName);
        swP.WriteLine("abdomenTwist" + ",chestBend" + ",headBend" + ",lShoulderAbduct" + ",rShoulderAbduct" +
                ",lShoulderSwing" + ",rShoulderSwing" + ",lElbowBend" + ",rElbowBend" + ",Class");             
//            sw.WriteLine("chestBend" + ",headBend" + ",Class");             
        
        
        foreach(string s in animMgr.AnimNameArr) {                                    
            ComputeMetricsForAnim(s);                  
           animMgr.ComputeHistograms();
            swE.WriteLine("{0:0},{1:0},{2:0},{3:0},{4:0},{5:0},{6:0},{7:0},{8:0}", metrics.MaxSpaceBin[0], metrics.MaxSpaceBin[1], metrics.MaxWeightBin[0], metrics.MaxWeightBin[1],
                                                                                                                             metrics.MaxTimeBin[0], metrics.MaxTimeBin[1],metrics.MaxFlowBin[0], metrics.MaxFlowBin[1], AnimNameTo2DPersonality(s));
            
            swP.WriteLine("{0:0},{1:0},{2:0},{3:0},{4:0},{5:0},{6:0},{7:0},{8:0},{9:0}", metrics.MaxPostureBin[0], metrics.MaxPostureBin[1],  metrics.MaxPostureBin[2], metrics.MaxPostureBin[3], metrics.MaxPostureBin[4],
                                                                                                                metrics.MaxPostureBin[5], metrics.MaxPostureBin[6], metrics.MaxPostureBin[7], metrics.MaxPostureBin[8],AnimNameTo2DPersonality(s));                    
        }
        swE.Close();
        swP.Close();
    }
    
    void RecordEffort(string effortType){
        string fileName = "METRICS\\" + effortType + ".txt";
        StreamWriter sw = new StreamWriter(fileName);     		
        foreach(string s in animMgr.AnimNameArr) {                                    			
			ComputeMetricsForAnim(s);
                        
		    //Record effort metrics
            if(effortType.Equals("space")) {
				for(int i = 0; i < keyCnt; i++)
                	sw.WriteLine(s+ "\t{0:0.00}\t{1:0}\t{2:0.00}\t{3:0}", metrics.SpaceMetric[0][i], metrics.SpaceIndex[0][i], metrics.SpaceMetric[1][i], metrics.SpaceIndex[1][i]);                    
			}
            else if(effortType.Equals("spaceDirect")) {				
                	sw.WriteLine(s+ "\t{0:0.0000}\t{1:0.0000}", metrics.MinSpace[0], metrics.MinSpace[1]);                    
			}
			else if(effortType.Equals("weight")) {								
				sw.WriteLine(s+ "\t{0:0}\t{1:0}", metrics.WeightStartKey, metrics.WeightEndKey);                    
				for(int i = 0; i < keyCnt; i++)
                	sw.WriteLine(s+ "\t{0:0.0000}\t{1:0}\t{2:0.0000}\t{3:0}", metrics.WeightMetric[0][i], metrics.WeightIndex[0][i], metrics.WeightMetric[1][i], metrics.WeightIndex[1][i]);                    
			}
            else if(effortType.Equals("weightStrong")) {								
				sw.WriteLine(s+ "\t{0:0.0000}\t{1:0.0000}", metrics.MaxWeight[0], metrics.MaxWeight[1]);                    
				
			}            				
            else if(effortType.Equals("time")){
                for(int i = 0; i < keyCnt; i++)
                    sw.WriteLine(s+ "\t{0:0.00}\t{1:0}\t{2:0.00}\t{3:0}", metrics.TimeMetric[0][i], metrics.TimeIndex[0][i], metrics.TimeMetric[1][i], metrics.TimeIndex[1][i]);                              
            }

            else if(effortType.Equals("timeSudden")){                
                    sw.WriteLine(s+ "\t{0:0.0000}\t{1:0.0000}", metrics.MinTime[0], metrics.MinTime[1]);                    
            }

            else if(effortType.Equals("flow")){
                for(int i = 0; i < keyCnt - 1; i++) //eliminate the last one
                    sw.WriteLine(s+ "\t{0:0.00}\t{1:0}\t{2:0.00}\t{3:0}", metrics.FlowMetric[0][i], metrics.FlowIndex[0][i], metrics.FlowMetric[1][i], metrics.FlowIndex[1][i]);                              
            }

            else if(effortType.Equals("flowBound")){                
                    sw.WriteLine(s+ "\t{0:0.0000}\t{1:0.0000}", metrics.MinFlow[0], metrics.MinFlow[1]);                    
            }


            //Determine histogram boundaries
            for (int i = 0; i < 2; i++){
                if (metrics.MinSpace[i] < animMgr.MinSpaceAll[i])
                    animMgr.MinSpaceAll[i] = metrics.MinSpace[i];
                
                if (metrics.MaxSpace[i] > animMgr.MaxSpaceAll[i])
                    animMgr.MaxSpaceAll[i] = metrics.MaxSpace[i];

                if (metrics.MinTime[i] < animMgr.MinTimeAll[i])
                    animMgr.MinTimeAll[i] = metrics.MinTime[i];
                
                if (metrics.MaxTime[i] > animMgr.MaxTimeAll[i])
                    animMgr.MaxTimeAll[i] = metrics.MaxTime[i];

                if (metrics.MinWeight[i] < animMgr.MinWeightAll[i])
                    animMgr.MinWeightAll[i] = metrics.MinWeight[i];

                if (metrics.MaxWeight[i] > animMgr.MaxWeightAll[i])
                    animMgr.MaxWeightAll[i] = metrics.MaxWeight[i];

                if (metrics.MinFlow[i] < animMgr.MinFlowAll[i])
                    animMgr.MinFlowAll[i] = metrics.MinFlow[i];

                if (metrics.MaxFlow[i] > animMgr.MaxFlowAll[i])
                    animMgr.MaxFlowAll[i] = metrics.MaxFlow[i];
            }
              
            for (int i = 0; i < 10; i++){
                if (metrics.MinPosture[i] < animMgr.MinPostureAll[i])
                    animMgr.MinPostureAll[i] = metrics.MinPosture[i];

                if (metrics.MaxPosture[i] > animMgr.MaxPostureAll[i])
                    animMgr.MaxPostureAll[i] = metrics.MaxPosture[i];
            }
        }
        sw.Close();

       
 /*       
         //Record weight confidence
        fileName = "METRICS\\" + "weightConfidence.txt";
        sw = new StreamWriter(fileName);
        sw.WriteLine("Anim" + "\tleftArm" + "\tRightArm"); 
        string [] traitArr = {"O-", "O+", "C-", "C+", "E-", "E+", "A-", "A+", "N-", "N+"};
        for(int j = 0; j < 10; j++){
            string trait = traitArr[j];                    
            sw.WriteLine(trait + "\t{0:0.0000}\t{1:0.0000}", 
            EvaluateMetric(trait, (int)MetricType.Weight,0),EvaluateMetric(trait, (int)MetricType.Weight,1));
            
        }
        sw.Close();
   */    
    }
    
 

    void RecordPosture() {
        string fileName = "METRICS\\posture.txt";
        StreamWriter sw = new StreamWriter(fileName);
        sw.WriteLine("Anim" + "\tabdomenTwist" + "\tchestBend" + "\theadBend" + "\tlShoulderAbduct" + "\trShoulderAbduct" +
                "\tlShoulderSwing" + "\trShoulderSwing" + "\tlElbowBend" + "\trElbowBend");
 
        foreach(string s in animMgr.AnimNameArr) {                                    
            ComputeMetricsForAnim(s);                   

           /* 
            * //min 
                sw.WriteLine(s + "\t{0:0.0000}\t{1:0.0000}\t{2:0.0000}\t{3:0.0000}\t{4:0.0000}\t{5:0.0000}\t{6:0.0000}\t{7:0.0000}\t{8:0.0000}", 
                 metrics.postureMetric[0,0], metrics.postureMetric[1,0],metrics.postureMetric[2,0], metrics.postureMetric[3,0],metrics.postureMetric[4,0], metrics.postureMetric[5,0],metrics.postureMetric[6,0], metrics.postureMetric[7,0],metrics.postureMetric[8,0]);
            //max
                sw.WriteLine(s+ "\t{0:0.0000}\t{1:0.0000}\t{2:0.0000}\t{3:0.0000}\t{4:0.0000}\t{5:0.0000}\t{6:0.0000}\t{7:0.0000}\t{8:0.0000}", 
                 metrics.postureMetric[0,1], metrics.postureMetric[1,1],metrics.postureMetric[2,1], metrics.postureMetric[3,1],metrics.postureMetric[4,1], metrics.postureMetric[5,1],metrics.postureMetric[6,1], metrics.postureMetric[7,1],metrics.postureMetric[8,1]);
            */
            //avg
                sw.WriteLine(s+  "\t{0:0.0000}\t{1:0.0000}\t{2:0.0000}\t{3:0.0000}\t{4:0.0000}\t{5:0.0000}\t{6:0.0000}\t{7:0.0000}\t{8:0.0000}", 
                 metrics.PostureMetric[0][2], metrics.PostureMetric[1][2],metrics.PostureMetric[2][2], metrics.PostureMetric[3][2],metrics.PostureMetric[4][2], metrics.PostureMetric[5][2],metrics.PostureMetric[6][2], metrics.PostureMetric[7][2],metrics.PostureMetric[8][2]);
        
            
        }
        sw.Close();


               //Reorder files for evaluation
        ReorderFilesForStatAnalysis((int)MetricType.Posture);
       
/*
        //Record posture confidence
        fileName = "METRICS\\" + "postureConfidence.txt";
        sw = new StreamWriter(fileName);
        sw.WriteLine("Anim" + "\tabdomenTwist" + "\tchestBend" + "\theadBend" + "\tlShoulderAbduct" + "\trShoulderAbduct" +
                "\tlShoulderSwing" + "\trShoulderSwing" + "\tlElbowBend" + "\trElbowBend");
 
        string [] traitArr = {"O-", "O+", "C-", "C+", "E-", "E+", "A-", "A+", "N-", "N+"};
        for(int j = 0; j < 10; j++){
            string trait = traitArr[j];                    
            sw.WriteLine(trait + "\t{0:0.0000}\t{1:0.0000}\t{2:0.0000}\t{3:0.0000}\t{4:0.0000}\t{5:0.0000}\t{6:0.0000}\t{7:0.0000}\t{8:0.0000}", 
            EvaluateMetric(trait, (int)MetricType.Posture,0),EvaluateMetric(trait, (int)MetricType.Posture,1),EvaluateMetric(trait, (int)MetricType.Posture,2),
            EvaluateMetric(trait, (int)MetricType.Posture,3),EvaluateMetric(trait, (int)MetricType.Posture,4),EvaluateMetric(trait, (int)MetricType.Posture,5),
            EvaluateMetric(trait, (int)MetricType.Posture,6),EvaluateMetric(trait, (int)MetricType.Posture,7),EvaluateMetric(trait, (int)MetricType.Posture,8));
            
        }
        sw.Close();
        */
    }
    /*
    void InterpretPosture(){
        if(metrics.maxPostureBin[0])

    }
    */
    void ReorderFilesForStatAnalysis(int metricType){
        string s;
        string[] persTraitArr = {"O-", "O+", "C-", "C+","E-", "E+", "A-", "A+", "N-", "N+"};

        if(metricType == (int)MetricType.Weight) {
            StreamWriter sw = new StreamWriter("METRICS\\weightStrongSVM.txt");       
            sw.WriteLine("left, right, Class");
            StreamReader sr = new StreamReader("METRICS\\weightStrong.txt");                                                         
            while((s = sr.ReadLine()) != null) {                    
                string [] line = s.Split('\t');            
                for (int j = 1; j < line.Length; j++) {
                    sw.Write(line[j] + ",");
              
                }
                sw.WriteLine(AnimNameTo2DPersonality(s));
            }
            sr.Close();
            sw.Close();
        }
        if(metricType == (int)MetricType.Posture) {
            StreamWriter sw = new StreamWriter("METRICS\\postureSVM.txt");  
            sw.WriteLine("abdomenTwist" + ",chestBend" + ",headBend" + ",lShoulderAbduct" + ",rShoulderAbduct" +
                ",lShoulderSwing" + ",rShoulderSwing" + ",lElbowBend" + ",rElbowBend" + ",Class");             
//            sw.WriteLine("chestBend" + ",headBend" + ",Class");             
            StreamReader sr = new StreamReader("METRICS\\posture.txt");
            s = sr.ReadLine(); //skip heading line                          
            while((s = sr.ReadLine()) != null) {                                        
                  
                string [] line = s.Split('\t');
            //    sw.Write(line[2] +"," + line[3] + ",");
                for (int j = 1; j < line.Length; j++) {
                    sw.Write(line[j] + ",");
                }
                sw.WriteLine(AnimNameTo2DPersonality(s));
                        
   
            }                
           
            sr.Close();             
            sw.Close();  
        }

        if(metricType == (int)MetricType.Effort) { //all effort
            string sW, sS, sT, sF;
            StreamWriter sw = new StreamWriter("METRICS\\effortSVM.txt");       
           sw.WriteLine("spaceR, spaceL, weightR, weightL, timeR, timeL, flowR, flowL, Class"); 
            
            StreamReader srS = new StreamReader("METRICS\\spaceDirect.txt");                                                         
            StreamReader srW = new StreamReader("METRICS\\weightStrong.txt");                                                         
            StreamReader srT = new StreamReader("METRICS\\timeSudden.txt");                                                         
            StreamReader srF = new StreamReader("METRICS\\flowBound.txt");                                                         
            while((sS = srS.ReadLine()) != null) {
                sW = srW.ReadLine();
                sT = srT.ReadLine();
                sF = srF.ReadLine();
                string [] lineS = sS.Split('\t');            
                string [] lineW = sW.Split('\t');            
                string [] lineT = sT.Split('\t');            
                string [] lineF = sF.Split('\t');
                
                for (int j = 1; j < lineS.Length; j++)  sw.Write(lineS[j] + ",");              
                for (int j = 1; j < lineW.Length; j++)  sw.Write(lineW[j] + ",");              
                for (int j = 1; j < lineT.Length; j++)  sw.Write(lineT[j] + ",");              
                for (int j = 1; j < lineF.Length; j++)  sw.Write(lineF[j] + ",");              
                
                sw.WriteLine(AnimNameTo2DPersonality(sS)); //all are in the same order
            }
            srS.Close();
            srW.Close();
            srT.Close();
            srF.Close();
            sw.Close();
        }



    }
     //Return t-test values between individual personality traits and the rest of the data
    //persTrait = [neutral o- o+ c- c+ e- e+ a- a+ n- n+]
    //metricType = [0, 1, 2, 3] for posture, weight, space, time, torso
    //submetricType : individual elements in metric.txt
    double  EvaluateMetric(string persTrait, int metricType, int subMetricType) {
        
        Sample traitValues = new Sample();
        Sample otherTraitValues = new Sample();
        string s;
        if(metricType == (int)MetricType.Posture) {
            StreamReader sr = new StreamReader("METRICS\\posture.txt");
            sr.ReadLine(); //the title line
            while((s = sr.ReadLine()) != null) {
                string [] line = s.Split('\t');
                                    
                if(s.Contains(persTrait))
                    traitValues.Add(float.Parse(line[1 + subMetricType]));                
                else
                   otherTraitValues.Add(float.Parse(line[1 + subMetricType])); // 1 for animation name                 
            }
            sr.Close();
        }  

        else if(metricType == (int)MetricType.Weight) {
            /*StreamReader sr = new StreamReader("METRICS\\weightStrongCnt.txt");                  
            while((s = sr.ReadLine()) != null) {
                string [] line = s.Split('\t');                
                if(s.Contains(persTrait))
                    traitValues.Add(float.Parse(line[1 + subMetricType]));
                else
                    otherTraitValues.Add(float.Parse(line[1 + subMetricType])); // 1 for animation name                
            }
            sr.Close();

            */
            StreamReader sr = new StreamReader("METRICS\\weightStrong.txt");            
            while((s = sr.ReadLine()) != null) {
                string [] line = s.Split('\t');                
                if(s.Contains(persTrait))
                    traitValues.Add(float.Parse(line[1 + subMetricType]));
                else
                    otherTraitValues.Add(float.Parse(line[1 + subMetricType])); // 1 for animation name                
            }
             sr.Close();
            
        } 
 

        TestResult result = Sample.StudentTTest(traitValues, otherTraitValues);

        return result.RightProbability;

      //  OneWayAnovaResult result = Sample.OneWayAnovaTest(traitValues, otherTraitValues);

        //return(result.Result.RightProbability);
     
    }
   
    
}

