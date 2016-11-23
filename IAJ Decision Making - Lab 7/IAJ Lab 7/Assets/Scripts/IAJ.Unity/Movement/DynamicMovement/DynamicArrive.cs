using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicVelocityMatch
    {
        public override string Name
        {
            get { return "Arrive"; }
        }
        public float TimeToTarget { get; set; }
        public float stopRadius { get; set; }
        public float slowRadius { get; set; }
        public float maxSpeed { get; set; }
        
        public DynamicArrive()
        {
           /* this.MaxAcceleration = 80.0f;
            this.TimeToTargetSpeed = 1.0f;
            this.stopRadius = 1.0f;
            this.slowRadius = 2.5f;
            this.maxSpeed = 40.0f;*/
            this.MovingTarget = new KinematicData();
        }

        public override MovementOutput GetMovement()
        {

            Vector3 direction = this.Target.position - this.Character.position;
            float distance = direction.magnitude;
            float targetSpeed = 0f;
            if (distance < stopRadius)
            {
                targetSpeed = 0f;
            }
            else if (distance > slowRadius)
            {
                targetSpeed = maxSpeed;
            }
            else
            {
                targetSpeed = maxSpeed * (distance / slowRadius);
            }
            this.MovingTarget.velocity = direction.normalized * targetSpeed;
            return base.GetMovement();
        }
    }
}

