using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts
{
    [System.Serializable]
    public class Haptic
    {
        [Range(0f, 1f)]
        public float Intensity;
        public float Duration;


        public void TriggerHaptic(BaseInteractionEventArgs args)
        {
            if (args.interactorObject is XRBaseControllerInteractor controllerInteractor)
                TriggerHaptic(controllerInteractor.xrController);
        }

        public void TriggerHaptic(XRBaseController controller)
        {
            if (Intensity > 0f)
                controller.SendHapticImpulse(Intensity, Duration);
        }
    }
}
