using Leap;
using UnityEngine;

public class HandSelectionManager : MonoBehaviour
{
    public static HandSelectionManager Instance;

    public Chirality SelectedHand { get; private set; }
    public bool HasSelectedHand { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SelectHand(Chirality hand)
    {
        if (HasSelectedHand) return;

        SelectedHand = hand;
        HasSelectedHand = true;

        Debug.Log("Selected hand: " + hand);

        GameManager.Instance.OnHandSelected(hand);
    }

    public void ResetSelection()
    {
        HasSelectedHand = false;
    }
}
