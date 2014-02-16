using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Metrics {

    public int NBins = 2;//20;//200;//500;//100;
	public float[][] SpaceMetric = new float[2][];
    public int[][] SpaceIndex = new int[2][];	
    public int SpaceStartKey, SpaceEndKey;
    public int[] DirectKey = new int[2];
    public float[] MinSpace = new float[2];
    public float[] MaxSpace = new float[2];
    public float[] AvgSpace = new float[2];
    public float[][] SpaceHist = new float[2][];
    public int[] MaxSpaceBin = new int[2]; //the bin with max # of space elements

    public float[][] WeightMetric = new float[2][];
    public int[][] WeightIndex = new int[2][];
	public int WeightStartKey, WeightEndKey;
    public int[] StrongestKey = new int[2];
	public float[] MinWeight = new float[2];
    public float[] MaxWeight = new float[2];
    public float[] AvgWeight = new float[2];
    public float[][] WeightHist = new float[2][];
    public int[] MaxWeightBin = new int[2]; //the bin with max # of weight elements
    
    public float[][] TimeMetric = new float[2][];
    public int[][] TimeIndex = new int[2][];	
	public int TimeStartKey, TimeEndKey;
    public int[] SuddenKey = new int[2];
	public float[] MinTime = new float[2];
    public float[] MaxTime = new float[2];
    public float[] AvgTime = new float[2];
    public float[][] TimeHist = new float[2][];
    public int[] MaxTimeBin = new int[2]; //the bin with max # of time elements
    
    public float[][] FlowMetric = new float[2][];
    public int[][] FlowIndex = new int[2][];	
	public int FlowStartKey, FlowEndKey;
    public int[] BoundKey = new int[2];
	public float[] MinFlow = new float[2];
    public float[] MaxFlow = new float[2];
    public float[] AvgFlow = new float[2];
    public float[][] FlowHist = new float[2][];
    public int[] MaxFlowBin = new int[2]; //the bin with max # of flow elements
    
    
    public float[][] PostureMetric = new float[10][]; //abdomen twist, chest bend, head bend, shoulder ad/abduct, shoulder swing, elbow bend  range [min-max], arm openness
	public float[][] PostureHist = new float[10][];
    public float[] MinPosture = new float[10];
    public float[] MaxPosture = new float[10];
    public int[] MaxPostureBin = new int[10];

    //Emote metrics
    public float[][] Curvature = new float[2][];  //curvature for each arm
    public float[][] CurvatureHist = new float[2][];  //curvature for each arm
    public int[] MaxCurvatureBin = new int[2];

	public KeyInfo[] Keys;

    public List<int> Segments = new List<int>(); //frame indices for the start of motion segments
    public List<int> KeySegments = new List<int>(); //frame indices for the start of motion segments
	
	
	int _keyCnt;
	// Use this for initialization
	public void Initialize(KeyInfo [] animKeys) {
		
		Keys = new KeyInfo[animKeys.Length];
		for(int i = 0; i < animKeys.Length ; i++){
			Keys[i] = animKeys[i];			
		}
		
		
        _keyCnt = Keys.Length - 2 ;	// -1 because keys 0 and keys.length-1 are the same, -2 because the last frame gives extreme results
		//Alloc space according to keyframe count 
        for(int i = 0; i < 2; i++) {
            SpaceMetric[i] = new float[_keyCnt];
            SpaceIndex[i] = new int[_keyCnt];
            WeightMetric[i] = new float[_keyCnt];
            WeightIndex[i] = new int[_keyCnt];
            TimeMetric[i] = new float[_keyCnt];
            TimeIndex[i] = new int[_keyCnt];
            FlowMetric[i] = new float[_keyCnt -1]; //eliminate the last element as it is unnaturally big
            FlowIndex[i] = new int[_keyCnt];
            
            Curvature[i] = new float[_keyCnt];

            //Initialize histograms        
            SpaceHist[i] = new float[NBins + 1];
            TimeHist[i] = new float[NBins + 1];
            FlowHist[i] = new float[NBins + 1];
            WeightHist[i] = new float[NBins + 1];
            CurvatureHist[i] = new float[NBins + 1];

            for(int j = 0; j < NBins +1 ; j++) {
                SpaceHist[i][j] = 0;
                TimeHist[i][j] = 0;
                FlowHist[i][j] = 0;
                WeightHist[i][j] = 0;
                CurvatureHist[i][j] = 0;
            }

        }		

        Segments.Clear();
        KeySegments.Clear();
        
        for(int i = 0; i < 10; i++) {
            PostureMetric[i] = new float[_keyCnt]; // 10 angles for keyframes
            PostureHist[i] = new float[NBins + 1];
            for(int j = 0; j< NBins +1 ; j++) 
                PostureHist[i][j] = 0;
            
        }
        
	}

    //Segment the motion according to the end effector: arm
    public void SegmentMotion(int arm)
    {        
        //for(int i = 0; i < Keys.Length-1; i++) {            
        for(int i = 1; i < Keys.Length-2; i++) {            
            float v1 = Keys[i].EeAcc[arm].magnitude-Keys[i-1].EeAcc[arm].magnitude;
            float v2 = Keys[i+1].EeAcc[arm].magnitude-Keys[i].EeAcc[arm].magnitude;

            if(Mathf.Sign(v1*v2) < 0){

         //   if(Vector3.Dot(Keys[i].EeAcc[arm], Keys[i+1].EeAcc[arm])  < 0f) { //different direction in acceleration            
         //   if (Vector3.Dot(Keys[i].EeVel[arm], Keys[i + 1].EeVel[arm]) < 0f) { //different direction in velocity
     //       if ((Keys[i].EeAcc[arm].magnitude - 0) < 0.01f) { //local extrema in motion
                Segments.Add(Keys[i].FrameNo); //keep the frame number
                KeySegments.Add(i);
                
            }
        }
    }

    public int GetSegmentNo(int currFrame) {    
        for(int i = 0; i < Segments.Count; i++){            
            if(Segments[i] >= currFrame) {
                return i;
            }
        }
        return Segments.Count;
    }
    
    int ComputeHistogram(int nBins, float minMetric, float maxMetric, float[] effortMetric, float[] effortHist) {

        int maxEffortInd = -1;
  
         //Histogram
        float range = (maxMetric - minMetric) / nBins;        
        for(int i = 2; i < effortMetric.Length; i++) 
            effortHist[Mathf.FloorToInt((effortMetric[i] - minMetric) / range)] += 1.0f;            

        float maxVal = -10000f;
        for(int i = 0; i < nBins; i++) {
            if(effortHist[i] > maxVal) {
                maxVal = effortHist[i];
                maxEffortInd = i;
            }
        }

        return maxEffortInd;

        /*
        //Normalize histogram to [0 10] range
        float minHist = Mathf.Min(effortHist); 
        float maxHist = Mathf.Max(effortHist);
        float dist = maxHist - minHist;
        if (dist != 0) {
            for(int i = 0; i < nBins + 1; i++) {
                effortHist[i] = 10* (effortHist[i] - minHist) / dist;
                
            }
        } */       
    }

    public void Space(int arm)
    {
        //Space
		SpaceIndex[arm][0] = 0;
		SpaceMetric[arm][0] = 1;
		
		SpaceIndex[arm][1] = 0;
		SpaceMetric[arm][1] = 1;

        AvgSpace[arm] = 0;
		for(int k = 2; k < _keyCnt; k++) {			
        	int minInd = -1;
        	float minRatio = 100000f;
	        for(int i = 0; i <= k - 2; i++) {	    //skip closest key because it will always be the most direct          				
				float displacement = 0;
	            for(int j = i; j < k ; j++)           
	                displacement +=  (Keys[k].EePos[arm] - Keys[j].EePos[arm]).magnitude;	             	        
	            
	            float netDist = (Keys[k].EePos[arm] - Keys[i].EePos[arm]).magnitude;
	
	            float ratio = displacement / netDist;

	            if (ratio < minRatio) {
	                minRatio = ratio;
	                minInd = i;
	            }
                
                AvgSpace[arm] += ratio;
	        }
			
			SpaceIndex[arm][k] = minInd;
			SpaceMetric[arm][k] = minRatio;
			
		}

        AvgSpace[arm] /= (_keyCnt - 2);
	//for debugging
		float minMetric = 1000000;
        float maxMetric = -1000000;        
		for(int i = 2; i < _keyCnt; i++) {
			if(SpaceMetric[arm][i] < minMetric && Math.Abs(SpaceMetric[arm][i] - 0) > 0.000000001) { //eliminate 0s
                DirectKey[arm] = i;
				minMetric = SpaceMetric[arm][i];
                SpaceStartKey = SpaceIndex[arm][i];
                SpaceEndKey = i;
            }
            if(SpaceMetric[arm][i] > maxMetric)
                maxMetric = SpaceMetric[arm][i];
            
        }
        MinSpace[arm] = minMetric;
        MaxSpace[arm] = maxMetric;

        //ComputeHistogram(nBins, minMetric, maxMetric, spaceMetric[arm], spaceHist[arm]);
    }



    public bool IsDirect(int arm, int keyInd) {
		if(SpaceMetric[arm][keyInd] < AvgSpace[arm])
			return true;
		else
			return false;
	}


    public void Weight(int arm){             
   
		WeightIndex[arm][0] = 0;
		WeightMetric[arm][0] = 0;
		AvgWeight[arm] = 0;			
	    
		for(int k = 1; k < _keyCnt; k++) {			
        	int maxInd = -1;
			float dec;
        	float maxDec = -100000f;
	        for(int i = 0; i < k; i++) {	             									            
	        	//acc =  (keys[k].eeVel[arm] - keys[i].eeVel[arm]).magnitude / (keys[k].frameNo - keys[i].frameNo);	             
               // dec =  (keys[k].eeVel[arm] - keys[i].eeVel[arm]).magnitude / (keys[k].time - keys[i].time);	             
                float deltaT = Keys[k].Time - Keys[i].Time;
                if (Math.Abs(deltaT - 0f) < 0.0000001)
                    continue;                
                if (Keys[k].EeVel[arm].magnitude - Keys[i].EeVel[arm].magnitude > 0) 
                    dec = ((Keys[k].EeVel[arm] - Keys[i].EeVel[arm]).magnitude) / deltaT ;
                else 
                    dec = -((Keys[k].EeVel[arm] - Keys[i].EeVel[arm]).magnitude) / deltaT;
	        	 
	            if (dec > maxDec) {
	                maxDec = dec;
	                maxInd = i; 
	            }
	        }
			
			WeightIndex[arm][k] = maxInd;
			WeightMetric[arm][k] = maxDec;
			AvgWeight[arm] += maxDec;
		
		}
		
		AvgWeight[arm] /= (_keyCnt - 1);
							
		//find strongest weight        
        float maxMetric = -1000000f;
        float minMetric = 1000000;
		for(int i = 0; i < _keyCnt; i++) {
			if(WeightMetric[arm][i] > maxMetric) {
                StrongestKey[arm] = i;
				maxMetric = WeightMetric[arm][i];								
				WeightStartKey = WeightIndex[arm][i];
				WeightEndKey = i;
			}
               if(WeightMetric[arm][i] < minMetric && Math.Abs(WeightMetric[arm][i] - 0) > 0.00000001)  //eliminate 0s
                minMetric = WeightMetric[arm][i];
        }        		
		MinWeight[arm] = minMetric;
        MaxWeight[arm] = maxMetric;
        

        //ComputeHistogram(nBins, minMetric, maxMetric, weightMetric[arm], weightHist[arm]);

        
            

    }

    public bool IsStrong(int arm, int keyInd)
    {
		if(WeightMetric[arm][keyInd] > AvgWeight[arm])
			return true;
		else
			return false;
	}

    
    public void Time(int arm){                
        TimeIndex[arm][0] = 0;
		TimeMetric[arm][0] = 0;		
        AvgTime[arm] = 0;	
		for(int k = 1; k < _keyCnt; k++) {			
        	int minInd = -1;			
        	float minAcc = 100000f;
	        for(int i = 0; i < k; i++) {	             									            
	            float acc = 0;
                for(int j = i+1 ; j <= k; j++) {
                    float deltaT = Keys[j].Time - Keys[j-1].Time;
                    if(Math.Abs(deltaT - 0f) < 0.00000001) continue;
                    acc +=  (Keys[j].EeVel[arm] - Keys[j-1].EeVel[arm]).magnitude / deltaT;	             
                }
	        	            				
	            if (acc < minAcc) {
	                minAcc = acc;
	                minInd = i; 
	            }
	        }        
			
		    TimeIndex[arm][k] = minInd;
	        TimeMetric[arm][k] = minAcc;
            AvgTime[arm] += minAcc;
		}	
    
        AvgTime[arm] /= (_keyCnt - 1);
		
         //find most sudden time        
        float minMetric = 1000000f;
        float maxMetric = -1000000f;
		for(int i = 0; i < _keyCnt; i++) {
            if (TimeMetric[arm][i] < minMetric && Math.Abs(TimeMetric[arm][i] - 0) > 0.00000001) { //eliminate 0s
                SuddenKey[arm] = i;
				minMetric = TimeMetric[arm][i];								
				TimeStartKey = TimeIndex[arm][i];
				TimeEndKey = i;
			}

             if(TimeMetric[arm][i] >maxMetric)
                maxMetric = TimeMetric[arm][i];
            
		}        		
		MinTime[arm] = minMetric;
        MaxTime[arm] = maxMetric;


      
		
        //ComputeHistogram(nBins, minMetric, maxMetric, timeMetric[arm], timeHist[arm]);
   
    }


    public void Flow(int arm){                
        FlowIndex[arm][0] = 0;
		FlowMetric[arm][0] = 0;		
        AvgFlow[arm] = 0;	
		for(int k = 1; k < _keyCnt - 1; k++) {	//eliminate the last element in the flow as it is unnaturally big		
        	int minInd = -1;			
        	float minJerk = 100000f;
	        for(int i = 0; i < k; i++) {	             									            
	            float jerk = 0;
                for(int j = i+1 ; j <= k; j++) {
                    float deltaT = Keys[j].Time - Keys[j-1].Time;
                    if (Math.Abs(deltaT - 0f) < 0.00000001) continue;
                    jerk +=  (Keys[j].EeAcc[arm] - Keys[j-1].EeAcc[arm]).magnitude / deltaT;	             
                }
	        	            				
	            if (jerk < minJerk) {
	                minJerk = jerk;
	                minInd = i; 
	            }
	        }        
			
		    FlowIndex[arm][k] = minInd;
	        FlowMetric[arm][k] = minJerk;
            AvgFlow[arm] += minJerk;
		}	
    
        AvgFlow[arm] /= (_keyCnt - 1);
		
         //find most bounded flow
        float minMetric = 1000000f;
        float maxMetric = -1000000f;

        
		for(int i = 0; i < _keyCnt-1; i++) { //eliminate the last element in the flow as it is unnaturally big
            if (FlowMetric[arm][i] < minMetric && Math.Abs(FlowMetric[arm][i] - 0) > 0.00000001) { //eliminate 0s
                BoundKey[arm] = i;
				minMetric = FlowMetric[arm][i];								
				FlowStartKey = FlowIndex[arm][i];
				FlowEndKey = i;
			}
             if(FlowMetric[arm][i] > maxMetric) 
                maxMetric = FlowMetric[arm][i];
            
		}      		
		MinFlow[arm] = minMetric;
        MaxFlow[arm] = maxMetric;


        //ComputeHistogram(nBins, minMetric, maxMetric, flowMetric[arm], flowHist[arm]);
		
    }

     public void Posture() {
        float angle  = 0f;        
        
        for(int j = 0; j < 10; j++) { //for each different angle
            for(int k = 0; k < _keyCnt; k++) { //For each keyframe            
                //distance of end effector to body
                //computed as the projection of wrist position to spine-neck line
                if(j==9) {
                    Vector3 x0 = Keys[k].EePos [1];
                    Vector3 x1 = Keys[k].SpinePos;
                    Vector3 x2 = Keys[k].NeckPos;
                
                    PostureMetric[j][k] = Vector3.Cross((x0 - x1), (x0 - x2)).magnitude / (x2-x1).magnitude;
                    continue;
                }

                else if(j==0) angle = Keys[k].SpineRot.z;
                else if(j == 1) angle = Keys[k].Spine1Rot.x;
                else if(j == 2) angle = Keys[k].NeckRot.x;
                else if(j == 3) angle = Keys[k].ClavicleLRot.y;
                else if(j == 4) angle = Keys[k].ClavicleRRot.y;
                else if(j == 5) angle = Keys[k].ClavicleLRot.x;
                else if(j == 6) angle = Keys[k].ClavicleRRot.x;
                else if(j == 7) angle = Keys[k].ElbowLRot.z;
                else if(j == 8) angle = Keys[k].ElbowRRot.z;       
        
                PostureMetric[j][ k] = angle * Mathf.Rad2Deg;
                 //if (postureMetric[j][ k] < 0f) postureMetric[j][k] *= -1;               
            }

            MinPosture[j] = Mathf.Min(PostureMetric[j]);
            MaxPosture[j] = Mathf.Max(PostureMetric[j]);
           // float minMetric = Mathf.Min(postureMetric[j]);
           // float maxMetric = Mathf.Max(postureMetric[j]);
           // ComputeHistogram(nBins, minMetric, maxMetric, postureMetric[j], postureHist[j]);
    
        }	   
    
     }

    //Compute emote-specific effort parameters
    public void Emote(int arm){

    //curvature
    //http://math.mit.edu/classes/18.013A/HTML/chapter15/section04.html
        for(int k = 0; k < _keyCnt; k++) { //For each keyframe   

            
            //MIT method
            float vv = Vector3.Dot(Keys[k].EeVel[arm], Keys[k].EeVel[arm]);
            float av = Vector3.Dot(Keys[k].EeAcc[arm], Keys[k].EeVel[arm]);
            if (Math.Abs(vv - 0) < 0.000001)
                Curvature[arm][k] = 0;
            else
                Curvature[arm][k] = ((vv * Keys[k].EeAcc[arm] - av * Keys[k].EeVel[arm]) / vv).magnitude / vv;
            

            //Zhao's method
            //(Vector3.Cross(Keys[k].EeVel[arm], Keys[k].EeAcc[arm]));
        }
    }

    public void ComputeHistograms(float[] minSpaceAll,float[] maxSpaceAll,float[] minWeightAll,float[] maxWeightAll, float[] minTimeAll,float[] maxTimeAll,float[] minFlowAll,float[] maxFlowAll, float[]minPostureAll, float[] maxPostureAll ) {
        //Posture
        /*for(int j = 0; j < 10; j++) { //for each different angle
                ComputeHistogram(nBins,0,360, postureMetric[j], postureHist[j]);
    
        }*/

        for(int j = 0; j < 2; j++) { //for each arm
             MaxSpaceBin[j] = ComputeHistogram(NBins, minSpaceAll[j], maxSpaceAll[j], SpaceMetric[j], SpaceHist[j]);            
            MaxWeightBin[j] = ComputeHistogram(NBins, minWeightAll[j], maxWeightAll[j], WeightMetric[j], WeightHist[j]);

           MaxTimeBin[j] = ComputeHistogram(NBins, minTimeAll[j], maxTimeAll[j], TimeMetric[j], TimeHist[j]);
           MaxFlowBin[j] = ComputeHistogram(NBins, minFlowAll[j], maxFlowAll[j], FlowMetric[j], FlowHist[j]);
        } 


         for(int j = 0; j < 10; j++) {             
            MaxPostureBin[j] = ComputeHistogram(NBins, minPostureAll[j], maxPostureAll[j], PostureMetric[j], PostureHist[j]);
            

         }
        
    }
    
   
}
