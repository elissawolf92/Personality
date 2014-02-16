using UnityEngine;
using System.Collections;

public class Blender
{
    private static readonly float EPSILON = 0.05f;
    private enum State { Max, Min, BlendUp, BlendDown };
    private State state = State.Min;

    private float maxVal;
    private float minVal;
    private float multiplier;

    public float Value { get; private set; }
    public float Inverse { get { return this.maxVal - this.Value; } }
    public bool IsMin { get { return this.Value < (this.minVal + EPSILON); } }
    public bool IsMax { get { return this.Value > (this.maxVal - EPSILON); } }

    public Blender()
    {
        this.state = State.Min;
        this.maxVal = 1.0f;
        this.minVal = 0.0f;
        this.multiplier = 1.0f;
        this.Value = this.minVal;
    }

    public Blender(float mult)
    {
        this.state = State.Min;
        this.maxVal = 1.0f;
        this.minVal = 0.0f;
        this.multiplier = mult;
        this.Value = this.minVal;
    }

    public void Tick(float deltaTime)
    {
        switch (this.state)
        {
            case State.BlendUp:
                this.Blend(deltaTime, this.multiplier);
                break;
            case State.BlendDown:
                this.Blend(deltaTime, -this.multiplier);
                break;
        }
    }

    public void ToMax()
    {
        switch (this.state)
        {
            case State.Min:
                this.state = State.BlendUp;
                break;
            case State.BlendDown:
                this.state = State.BlendUp;
                break;
        }
    }

    public void ForceMax()
    {
        this.state = State.Max;
        this.Value = this.maxVal;
    }

    public void ForceMin()
    {
        this.state = State.Min;
        this.Value = this.minVal;
    }

    public void ToMin()
    {
        switch (this.state)
        {
            case State.Max:
                this.state = State.BlendDown;
                break;
            case State.BlendUp:
                this.state = State.BlendDown;
                break;
        }
    }

    private void Blend(float deltaTime, float scale)
    {
        this.Value += deltaTime * scale;
        if (this.Value > this.maxVal)
        {
            this.Value = this.maxVal;
            this.state = State.Max;
        }
        else if (this.Value < this.minVal)
        {
            this.Value = this.minVal;
            this.state = State.Min;
        }
    }
}
