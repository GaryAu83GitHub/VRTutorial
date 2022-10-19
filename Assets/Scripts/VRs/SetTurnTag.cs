using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SetTurnTag : MonoBehaviour
{
    public ActionBasedSnapTurnProvider SnapTurn;
    public ActionBasedContinuousTurnProvider ContinuousTurn;

    public void SetTypeInIndex(int anIndex)
    {
        if (anIndex == 0)
            SnapTurn.enabled = false;
        else if (anIndex == 1)
            SnapTurn.enabled = true;

        ContinuousTurn.enabled = !SnapTurn.enabled;
    }
}
