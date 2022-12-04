using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerrisWheelPlatformController : MonoBehaviour
{
    [Header("Ferris Wheel Control")]
    [SerializeField] private float timeBetweenParts;
    [SerializeField] private float rotationAdd = 90;//private float[] angles = { 90, 180, 270, 360 };

    [Header("Component References")]
    public HingeJoint2D hinge;

    private FerrisWheelState currentState;
    
    private int angleIndex = 0;

    void Start()
    {
        currentState = new WaitingForSpin(timeBetweenParts);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState.DoActions(Time.deltaTime))
        {
            SwitchState();
        }
    }

    private void SwitchState() //decided to experiment with this type of state system as I learned it in another class and wished to try it (always used enums before and thought this was a cool method)
    {
        if (currentState.GetStateType() == 0)
        {
            currentState = new SpinningSelf(hinge, rotationAdd);
            angleIndex += 1;
        }
        else
        {
            currentState = new WaitingForSpin(timeBetweenParts);
        }
    }

}

public abstract class FerrisWheelState
{
    protected int stateType;
    public abstract bool DoActions(float delta);
    public int GetStateType()
    {
        return stateType;
    }
}

public class WaitingForSpin : FerrisWheelState
{
    private float waitTime;

    public WaitingForSpin(float waitTime)
    {
        this.waitTime = waitTime;
        stateType = 0;
    }
    public override bool DoActions(float deltaTime){
        waitTime -= deltaTime;
        return waitTime <= 0;
    }
}

public class SpinningSelf : FerrisWheelState
{
    private HingeJoint2D jointRef;
    public SpinningSelf(HingeJoint2D jointRef, float angleAdd)
    {
        this.jointRef = jointRef;
        SetjointStuff(angleAdd);
        stateType = 1;
    }
    private void SetjointStuff(float angle)
    {
        JointAngleLimits2D limit = jointRef.limits;
        limit.max += angle;
        limit.min += angle;
        jointRef.limits = limit;
        jointRef.useMotor = true;

    }
    public override bool DoActions(float delta)
    {
        return jointRef.jointAngle >= jointRef.limits.max;
    }
}