using UnityEngine;

public class WorldTrigger : MonoBehaviour
{
    [Header("Type")]
    public bool isLedge = false;
    public bool isRamp = false;
    public bool simpleHeightchange = false;
    public bool dashLanding = false;

    [Header("Behaviour")]
    public bool changesHeight = false;

    [Header("Ramp settings")]
    public float rampSpeedModifier = 1f;
    public float rampInclination = 0.2f;
    public bool rampToRight = false;
    public bool rampToLeft = false;

    [Header("Ledge settings")]
    public bool fallToEast;
    public bool fallToWest;
    public bool fallToNorth;
    public bool fallToSouth;

    [Header("Height settings")]
    public int targetHeight = 0;

    [Header("debug?")]
    public string triggerName = "Trigger";


}