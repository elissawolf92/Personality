using UnityEngine;
using System.Collections;
using System;

public enum StatusType 
{
	Susceptible,
	Wounded,
	Infected
}

[System.Serializable]
public class Contagion {
	
	private const float DoseVariance = 0.1f;
	private const float DoseMean = 1.0f;	
	
	//private MathDefs md = new MathDefs();
	private float susceptibility = 0.05f;
	
	private StatusType status = StatusType.Susceptible;
	
	private float dose = 0.0f;	
	public float Dose{	
		get{
			return dose;
		}
        set{
            dose = value;
            UpdateStatus();
        }		
	}
	
	public StatusType Status{
		get{		
			UpdateStatus();
			return status;
		}
	}
	
	private float doseThreshold;
	
	public float DoseThreshold{	
		get{
		
			return doseThreshold;
		}
		set{
			doseThreshold = MathDefs.GaussianDist(value,DoseVariance);
			if(doseThreshold < 0f)
				doseThreshold  = 0f;
						
		}
	}
	
		
	private float woundThreshold;
	
	public float WoundThreshold{	
		get{
		
			return woundThreshold;
		}
		set
		{
			woundThreshold = MathDefs.GaussianDist(value,DoseVariance);
		}
	}
	

	public void UpdateStatus(){
		if(Math.Abs(dose) > DoseThreshold)			
			status = StatusType.Infected;
		else if(Math.Abs(dose) > WoundThreshold)			
			status = StatusType.Wounded;
		else			
			status = StatusType.Susceptible;
	}
	
	public void AddDose( float sus){
	
		if(Status == StatusType.Susceptible || Status == StatusType.Wounded) {
	
			float d;
					
			d = MathDefs.GaussianDist(DoseMean,DoseVariance);

            dose += d * sus * Time.deltaTime;
								
			UpdateStatus();
		}
		
		
	}

    public void AddDose() {
        AddDose(susceptibility);        
    }
    
	public void DecayDose()  {
			
		float d;
		float decayCoef = 2f; //decays in twice the time as it increases
		
		d = MathDefs.GaussianDist(DoseMean/decayCoef,DoseVariance/decayCoef);
		
		//take some percentage of the normal dose depending on susceptibility
		dose -= d * susceptibility * Time.deltaTime;
	
		if(dose < 0f)
			dose = 0f;
		
		UpdateStatus();
		

	}
	
	
	
}
