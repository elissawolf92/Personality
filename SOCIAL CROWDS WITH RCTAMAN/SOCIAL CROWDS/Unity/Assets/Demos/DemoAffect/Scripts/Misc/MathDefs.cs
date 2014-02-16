using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class MathDefs  {
	
	private static System.Random rand = new System.Random();




	/// <summary>
	/// Gaussian Distribution with Box-Muller transform
	/// </summary>
   /// 
	public static float GaussianDist(float mean, float std) 
	{
		
		double u1 = rand.NextDouble(); //uniform(0,1) random doubles
		double u2 = rand.NextDouble();
		float randStdNormal = (float)Math.Sqrt(-2.0 * Math.Log(u1)) * (float)Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
		float randNormal = mean + std * randStdNormal; //random normal(mean,stdDev^2)						
		return randNormal;
			
	}
		
    public static float StdDev(List<float>data, float mean) {
        float std = 0f;

        if (data.Count == 0)
            return 0f;

        for(int i = 0; i < data.Count; i++){
            std += ((data[i] - mean) * (data[i] - mean));
        }
        
        std /= (data.Count - 1);

        return (float) Math.Sqrt(std);
    }
    public static int GetRandomNumber(int max)
    {		
		return rand.Next (max);		
	}

    public static float GetRandomNumber(float max) {
        return (float)rand.NextDouble() * max;
    }
    public static int GetRandomNumber(int min, int max) {
        return rand.Next(min, max);
    }
    public static float GetRandomNumber(float min, float max) {
        return min + (float)rand.NextDouble() * (max - min);
    }
    

	public static float GetLength(float[] v)
	{
	    float[] vec = {v[0], v[1], v[2], 0};
	
		vec[0] *= vec[0];
		vec[1] *= vec[1];
		vec[2] *= vec[2];
	
		return (float) Math.Sqrt(vec[0] + vec[1] + vec[2]);
	}
	
	/// <summary>
	/// Normalize all elements of the array arr to [min, max]
	/// </summary>
	public static void NormalizeElements(float[] arr, float min, float max)
	{
		int i;
		float arrMin, arrMax;
		float diff;
		
		arrMin = arrMax = arr[0];		
		for(i=0; i<arr.Length; i++) {		
			if(arr[i] < arrMin)
				arrMin = arr[i];
			if(arr[i] > arrMax)
				arrMax = arr[i];
		}
	
		
		diff = (arrMax - arrMin);
		
		if(arrMax <= max && arrMin >= min) //if they are already in the range [min max] do not change the array
			return;
		
		if(arrMax == arrMin) {
			if(arrMax == min)
				return; //all = min = max
			else if(arrMax > min && arrMax < max ) //[0 1] region
				return;
			else if(arrMax > max) { //clamp to 1
				for(i=0; i<arr.Length; i++)	
					arr[i] = max;
			}
			else if(arrMax < min) {  //clamp to 0
				for(i=0; i<arr.Length; i++)	
					arr[i] = min;
			}
			
		}
				
				
		
		for(i=0; i<arr.Length; i++)		
			arr[i] = (arr[i] - arrMin) / diff;
	}
	
	/// Returns the array length
	public static float  Length(float [] arr) {
		float len = 0;
		
		for(int i = 0 ; i < arr.Length; i++)
			len += arr[i] * arr[i];
		
		
		return Mathf.Sqrt (len);
	}

    //Projects point p onto the line segment a-b
    //Adapted from http://www.alecjacobson.com/weblog/?p=1486
    public static Vector3 ProjectPointLine (Vector3 p ,Vector3 a, Vector3 b) {
        Vector3 q;
        Vector3 ab = b - a;
        float abSquared = Vector3.Dot(a,b);
        
        if(abSquared == 0)
            q = a;
        else {       
            Vector3 ap = p - a;
            // from http://stackoverflow.com/questions/849211/
            // Consider the line extending the segment, parameterized as A + t (B - A)
            // We find projection of point p onto the line. 
            // It falls where t = [(p-A) . (B-A)] / |B-A|^2
            float t = Vector3.Dot(ap,ab)/abSquared;
            if (t < 0.0f) // "Before" A on the line, just return A            
                q = a;
            else if (t > 1.0f) {// "After" B on the line, just return B
                q = b;
            }
            else //projection lines "inbetween" A and B on the line
                q = a + t * ab;
        }

        return q;
    }
}
