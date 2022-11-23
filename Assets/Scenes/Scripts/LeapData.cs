using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapData : MonoBehaviour
{
    public Vector3 FingerTipPosition(Leap.Hand hand, Leap.Finger.FingerType fingerType)
    {
        if (hand == null)
        {
            print("Hand is null");
            return Vector3.zero;
        }
        else
        {
            return hand.Fingers[ (int) fingerType ].TipPosition;
        }
    }

    private void Start()
    {
        lastFingerTipsPosition = new Dictionary<string, Vector3>();
    }

    private void Update()
    {
        if (!leapServiceProvider) return;

        Leap.Frame currentFrame = leapServiceProvider.CurrentFrame;
        foreach (var hand in currentFrame.Hands)
        {
            Vector3 currentThumbPosition = FingerTipPosition(hand, Leap.Finger.FingerType.TYPE_THUMB);
            Vector3 currentIndexPosition = FingerTipPosition(hand, Leap.Finger.FingerType.TYPE_INDEX);

            string handOrientation = hand.IsRight ? "Right" : "Left";
            string thumbKey = handOrientation + Leap.Finger.FingerType.TYPE_THUMB;
            string indexKey = handOrientation + Leap.Finger.FingerType.TYPE_INDEX;

            if (!lastFingerTipsPosition.ContainsKey(thumbKey))
            {
                lastFingerTipsPosition[thumbKey] = FingerTipPosition(hand, Leap.Finger.FingerType.TYPE_THUMB);
            }

            if (!lastFingerTipsPosition.ContainsKey(indexKey))
            {
                lastFingerTipsPosition[indexKey] = FingerTipPosition(hand, Leap.Finger.FingerType.TYPE_INDEX);
            }

            if (Vector3.Distance(lastFingerTipsPosition[thumbKey], currentThumbPosition) >= displacementCheckThreshold)
            {
                thumbMagnitude = (currentThumbPosition - lastFingerTipsPosition[thumbKey]).magnitude;
                lastFingerTipsPosition[thumbKey] = currentThumbPosition;
            }
            else
            {
                thumbMagnitude = 0f;
            }

            if (Vector3.Distance(lastFingerTipsPosition[indexKey], currentIndexPosition) >= displacementCheckThreshold)
            {
                indexMagnitude = (currentIndexPosition - lastFingerTipsPosition[indexKey]).magnitude;
                lastFingerTipsPosition[indexKey] = currentIndexPosition;
            }
            else
            {
                indexMagnitude = 0f;
            }

            print(handOrientation + " thumb velocity: " + thumbMagnitude / Time.deltaTime);
            print(handOrientation + " index velocity: " + indexMagnitude / Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        if (lastFingerTipsPosition == null) return;

        foreach (var item in lastFingerTipsPosition)
        {
            print(item.Key + " : " + item.Value);
        }
    }

    [SerializeField] private Leap.Unity.LeapServiceProvider leapServiceProvider;
    [SerializeField] private float displacementCheckThreshold;
    private Dictionary<string, Vector3> lastFingerTipsPosition;
    private float thumbMagnitude;
    private float indexMagnitude;
}
