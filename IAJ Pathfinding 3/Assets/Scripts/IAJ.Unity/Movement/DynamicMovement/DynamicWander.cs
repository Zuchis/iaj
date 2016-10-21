using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicWander : DynamicSeek
    {
        public DynamicWander()
        {
            this.TurnAngle = 0.25f * MathConstants.MATH_PI;
            this.WanderOffset = 10f;
            this.WanderRadius = 9f;
            this.WanderOrientation = 0.0f;
            base.MaxAcceleration = 40.0f;
            this.Target = new KinematicData();
        }
        public override string Name
        {
            get { return "Wander"; }
        }
        public float TurnAngle { get; private set; }

        public float WanderOffset { get; private set; }

        public float WanderRadius { get; private set; }
        
        protected float WanderOrientation { get; set; }

        public override MovementOutput GetMovement()
        {

            WanderOrientation += RandomHelper.RandomBinomial() * TurnAngle;
            this.Target.orientation = WanderOrientation + this.Character.orientation;
            Vector3 circleCenter = this.Character.position + WanderOffset * MathHelper.ConvertOrientationToVector(this.Character.orientation);
            this.Target.position = circleCenter + WanderRadius * MathHelper.ConvertOrientationToVector(this.Target.orientation);
    
            return base.GetMovement();
        }
    }
}
