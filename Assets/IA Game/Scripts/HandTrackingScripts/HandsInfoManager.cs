using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class HandsInfoManager : MonoBehaviour
{
    public static HandsInfoManager Instance { get; private set; }
    public LeapProvider leapProvider;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    /*private void OnEnable() {
        leapProvider.OnUpdateFrame += OnUpdateFrame;
    }

    private void OnDisable() {
        leapProvider.OnUpdateFrame -= OnUpdateFrame;
    }

    void OnUpdateFrame(Frame frame) {


    }*/


    public List<Hand> GetCurrentHands() {

    Frame frame = leapProvider.CurrentFrame;

    List<Hand> hands = new List<Hand>();

    foreach (Hand hand in frame.Hands) {
        hands.Add(hand);
    }
    return hands;
    }

}
