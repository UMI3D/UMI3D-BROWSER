using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.picoBrowser
{
    public class PicoHapticSelectionFeedback : umi3dBrowsers.interaction.selection.feedback.AbstractVRHapticSelectionFeedback
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void Trigger()
        {
            Umi3dPicoInputManager.Instance.VibrateController(controller.type, settings.duration, settings.frequency, settings.amplitude);
        }
    }
}
