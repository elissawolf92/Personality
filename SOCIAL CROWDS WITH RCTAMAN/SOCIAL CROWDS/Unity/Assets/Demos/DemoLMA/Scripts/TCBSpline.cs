/* -----------------------------------------------------------------------------
Adapted from libspline - Spline library for realtime applications
Copyright (C) 2006  Joachim Klahr
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlPoint {
    public Vector3 Point;
    public Quaternion Rotation;
    public Vector3 TangentI;
    public Vector3 TangentO;
    public float Time;
}

public class TCBSpline {

    //Segment[] segments;
    int _amount;

    float mLength = 0f;
    public float Tension;
    public float Continuity;
    public float Bias;


    public ControlPoint[] _controlPoints;

    public TCBSpline(ControlPoint[] controlPoints, float tension, float continuity, float bias) {
        _controlPoints = new ControlPoint[controlPoints.Length];
        Tension = tension;
        Continuity = continuity;
        Bias = bias;
        
        _amount = controlPoints.Length;
        
        for (int i = 0; i < controlPoints.Length; ++i) 
            _controlPoints[i] = controlPoints[i];
                
        for (int i = 0; i < controlPoints.Length; i++) 
            CalculateTangents(i, tension, continuity, bias);

    }


    public Vector3 GetInterpolatedSplinePoint(float lt, int p) {

        if (p < _amount - 1) {
            //CalculateTangents(p, Tension, Continuity, Bias);
            return GetPointOnSegment(lt, p);
        }
        else
            return _controlPoints[_amount - 1].Point;
        
    }

    void CalculateTangents(int p, float tension, float continuity, float bias) {
        float a,b, c, d;
        float nPrev, nP, nO, nI;

        a = 0.5f * (1.0f - tension) * (1.0f + continuity) * (1.0f - bias);        
        b = 0.5f * (1.0f - tension) * (1.0f - continuity) * (1.0f + bias);
        c = 0.5f * (1.0f - tension) * (1.0f - continuity) * (1.0f - bias);
        d = 0.5f * (1.0f - tension) * (1.0f + continuity) * (1.0f + bias);

        //delta ensures continuity of speed
        
        float delta, deltaPrev;
        nO = nI = 1;
        delta = deltaPrev = 1;
        if (p == 0) {
            delta = _controlPoints[p + 1].Time - _controlPoints[p].Time;
         
            nPrev = 0;            
            nP = _controlPoints[p + 1].Time - _controlPoints[p].Time;

      //      nI =  2 * nP / (nPrev + nP) ;

            _controlPoints[p].TangentI = nI * a * (_controlPoints[p + 1].Point - _controlPoints[p].Point) / delta ;
            _controlPoints[p].TangentO = nO * c * (_controlPoints[p + 1].Point - _controlPoints[p].Point) / delta;        
        }       
        else if (p == _amount - 1) {
            deltaPrev =   _controlPoints[p].Time - _controlPoints[p - 1].Time;

            nPrev = _controlPoints[p].Time - _controlPoints[p - 1].Time;
            nP = 0;

      //      nO =   2 * nPrev / (nPrev + nP);
            

            _controlPoints[p].TangentI = nI * b * (_controlPoints[p].Point - _controlPoints[p - 1].Point) / deltaPrev;
            _controlPoints[p].TangentO = nO * d * (_controlPoints[p].Point - _controlPoints[p - 1].Point) / deltaPrev;
        }       
        else {

            delta = _controlPoints[p + 1].Time - _controlPoints[p].Time;
            deltaPrev = _controlPoints[p].Time - _controlPoints[p - 1].Time;

            nP = _controlPoints[p + 1 ].Time - _controlPoints[p].Time;
            nPrev = _controlPoints[p].Time - _controlPoints[p - 1].Time;


//            nO = 2 * nPrev / (nPrev + nP);
  //         nI =  2 * nP / (nPrev + nP);

        

            _controlPoints[p].TangentI = nI * (a * (_controlPoints[p + 1].Point - _controlPoints[p].Point) / delta + b * (_controlPoints[p].Point - _controlPoints[p - 1].Point) / deltaPrev);
            _controlPoints[p].TangentO = nO * (c * (_controlPoints[p + 1].Point - _controlPoints[p].Point) / delta + d * (_controlPoints[p].Point - _controlPoints[p - 1].Point) / deltaPrev);
        }


           // Debug.Log(p + " " + nI + " " + nO);
        //Debug.Log(p + " " + (_controlPoints[p].TangentI - _controlPoints[p].TangentO));
    }

    Vector3 GetPointOnSegment(float distance, int p) {
        Vector3 Point;
        float d2 = distance * distance;
        float d3 = d2 * distance;
        float fH1 = 2* d3  - 3 * d2 + 1;
        float fH2 = -2 * d3 + 3 * d2;
        float fH3 = d3 - 2 * d2 + distance;
        float fH4 = d3 - d2;

        Point = _controlPoints[p].Point * fH1 + _controlPoints[p + 1].Point * fH2 +  _controlPoints[p].TangentO * fH3 +  _controlPoints[p + 1].TangentI * fH4;

        return Point;
    }
    /*
    //Local distance on the segment
    public float FindDistanceOnSegment(Vector3 point, int p) {
        Vector3 d3Coef, d2Coef, dCoef, cCoef;        
        List<float> coefList = new List<float>();        
        List<float> rootList = new List<float>();

        d3Coef = 2 * _controlPoints[p].Point - 2 * _controlPoints[p + 1].Point + _controlPoints[p].TangentO + _controlPoints[p + 1].TangentI;
        d2Coef = -3 * _controlPoints[p].Point + 3 * _controlPoints[p + 1].Point - 2 * _controlPoints[p].TangentO - _controlPoints[p + 1].TangentI;
        dCoef = _controlPoints[p].TangentO;
        cCoef = _controlPoints[p].Point - point;

        
        coefList.Add(d3Coef.x);
        coefList.Add(d2Coef.x);
        coefList.Add(dCoef.x);
        coefList.Add(cCoef.x);
        rootList = SolveEquations.SolvePolynomialEquation(coefList);


        return rootList[0];
    }
    */
   
}
