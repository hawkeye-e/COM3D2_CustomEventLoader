using System.Collections.Generic;
using UnityEngine;

namespace COM3D2.CustomEventLoader.Plugin
{
    internal class ConfigurableValue
    {
        internal const float CameraFadeTime = 0.5f;

        internal const float AnimationBlendTime = 0.5f;

        internal static class YotogiCameraWork
        {
            internal static readonly Vector3 CameraVectorOffset = new Vector3(3f, 2f, 2f);
            internal static readonly Vector3 TargetVectorOffset = Vector3.zero;
            internal const float CameraDistance = 4f;
            //internal static readonly Vector2 CameraAngle = new Vector2(230f, 32f);

            internal const float DefaultHorizontalRotation = 225f;
            internal const float DefaultVerticalRotation = 25f;
            //internal const float DefaultSidewardRotation = 0f;
        }


    }
}