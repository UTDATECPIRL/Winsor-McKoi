﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Leap.Unity;
using Leap;
using System;

public class LEAP_coordinates : MonoBehaviour
{
    public string myName;
    // Start is called before the first frame update
    public HandModel hand_model;
    public Hand leap_hand;
    private FingerModel finger;
    public OSC osc;

    void Start()
    {
        HandModel hand_model = GetComponent<HandModel>();
        //Hand leap_hand = hand_model.GetLeapHand();
        //if (leap_hand == null) Debug.LogError("No leap_hand found");

    }

    // Update is called once per frame
    void Update()
    {
        Hand leap_hand = hand_model.GetLeapHand();
        //if (leap_hand == null) Debug.LogError("No leap_hand found");
        //var hand = Hands.Right;
        if (leap_hand != null)
        {
            Leap.Vector indexPosition = leap_hand.Fingers[1].TipPosition * 100;
            Leap.Vector middlePosition = leap_hand.Fingers[2].TipPosition * 100;
            Leap.Vector ringPosition = leap_hand.Fingers[3].TipPosition * 100;
            Leap.Vector pinkyPosition = leap_hand.Fingers[4].TipPosition * 100;
            Leap.Vector thumbPosition = leap_hand.Fingers[0].TipPosition * 100;
            Leap.Vector handCenter = leap_hand.PalmPosition * 100;

            float[] index = { 0, 0, 0 };
            float[] middle = { 0, 0, 0 };
            float[] ring = { 0, 0, 0 };
            float[] pinky = { 0, 0, 0 };
            float[] thumb = { 0, 0, 0 };
            float[] newCenter = { 0, 0, 0 };

            for (uint i = 0; i < 3; i++)
            {
                newCenter[i] = handCenter[i] - handCenter[i];
            }
            for (uint i = 0; i < 3 ; i++)
            {
                index[i] = indexPosition[i] - handCenter[i];
            }
            for (uint i = 0; i < 3; i++)
            {
                middle[i] = middlePosition[i] - handCenter[i];
            }
            for (uint i = 0; i < 3; i++)
            {
                ring[i] = ringPosition[i] - handCenter[i];
            }
            for (uint i = 0; i < 3; i++)
            {
                pinky[i] = pinkyPosition[i] - handCenter[i];
            }
            for (uint i = 0; i < 3; i++)
            {
                thumb[i] = thumbPosition[i] - handCenter[i];
            }

            float[] fingertips = { index[0], index[1], index[2], middle[0], middle[1], middle[2], ring[0], ring[1], ring[2], pinky[0], pinky[1], pinky[2], thumb[0], thumb[1], thumb[2], handCenter[0], handCenter[1], handCenter[2]};

            for (int i = 0; i < 18; i++)
            {
                fingertips[i] = Mathf.Round(fingertips[i]*10)/10;
            }

            /*
            Debug.Log("Center" + handCenter[0] + "," + handCenter[1] + "," + handCenter[2]);
            Debug.Log("Old Index" + indexPosition[0] + "," + indexPosition[1] + "," + indexPosition[2]);
            Debug.Log("New Index" + index[0] + "," + index[1] + "," + index[2];
            Debug.Log("Middle" + middlePosition[0] + "," + middlePosition[1] + "," + middlePosition[2]);
            Debug.Log("Ring" + ringPosition[0] + "," + ringPosition[1] + "," + ringPosition[2]);
            Debug.Log("Pinky" + pinkyPosition[0] + "," + pinkyPosition[1] + "," + pinkyPosition[2]);
            Debug.Log("Thumb" + thumbPosition[0] + "," + thumbPosition[1] + "," + thumbPosition[2]);
            */

            OscMessage message = new OscMessage();
            message.address = "/wek/inputs";
            for (int i = 0; i < 18; i++)
            {
                message.values.Add(fingertips[i]);
            }
            osc.Send(message);
        }
    }
}
