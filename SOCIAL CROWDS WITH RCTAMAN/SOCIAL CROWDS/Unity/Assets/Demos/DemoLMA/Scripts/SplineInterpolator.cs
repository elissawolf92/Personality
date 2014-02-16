/*	Copyright (c) The Code Project Open License (CPOL) 1.02
 	Adapted from Overhauser (Catmull-Rom) Splines for Camera Animation
 	by Radu Gruian
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplineInterpolator {
	
	public List<Vector3> vp = new List<Vector3>();
	float deltaT = 0;
    
	int BOUNDS(int pp)  { 
		if (pp < 0) 
			return 0; 
		else if (pp >= vp.Count) 
			return vp.Count - 1; 
		else
			return pp;
	}
	// Solve the Catmull-Rom parametric equation for a given time(t) and vector quadruple (p1,p2,p3,p4)
	Vector3 Eq(float t, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float tension) {
	    float t2 = t * t;
	    float t3 = t2 * t;
		float s;
		
		s = (1f - tension)/2f;

	    float b1 = -s * t3 + 2f * s * t2 - s * t;
	    float b2 = (2f - s) * t3 + (s - 3f) * t2 + 1f;
	    float b3 =  (s - 2f) * t3 + (3f - 2 * s) * t2 + s * t;
	    float b4 = s * t3 - s * t2;

        return (p1*b1 + p2*b2 + p3*b3 + p4*b4); 
	}
	
	public void AddSplinePoint(Vector3 v) {
		
		vp.Add(v);
	    //vp.push_back(v);

	    deltaT = 1f / (float)(vp.Count - 1);
	}
	
	public Vector3 GetInterpolatedSplinePoint(float lt, int p,  float tension) {
	 
	    // Find out in which interval we are on the spline
        //Assumes intervals are equally spaced
	    //int  p = (int)(t / deltaT);
	    // Compute local control point indices
       
	    int p0 = p - 1;     p0= BOUNDS(p0);
	    int p1 = p;         p1 = BOUNDS(p1);
	    int p2 = p + 1;     p2 = BOUNDS(p2);
	    int p3 = p + 2;     p3 = BOUNDS(p3);
	    // Relative (local) time 
      
	    //float lt = (t - deltaT*(float)p) / deltaT;
		// Interpolate
	    return Eq(lt, vp[p0], vp[p1], vp[p2], vp[p3], tension);
	}
		

}

