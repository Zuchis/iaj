using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicFollowPath : DynamicArrive
    {
        public Path Path { get; set; }
        public float PathOffset { get; set; }

        public float CurrentParam { get; set; }

        private MovementOutput EmptyMovementOutput { get; set; }

        public DynamicFollowPath(KinematicData character, Path path) 
        {
            this.Target = new KinematicData();
            this.Character = character;
            this.Path = path;
            this.EmptyMovementOutput = new MovementOutput();
            this.PathOffset = 1.50f;
            //don't forget to set all properties
            //arrive properties
            //they are set in the DynamicArrive class
                      
        }

        public override MovementOutput GetMovement()
        {
            if (object.ReferenceEquals(null, this.Path)) return EmptyMovementOutput;

            if (this.Path.PathEnd(this.CurrentParam)) { 
                /* Hack : next line is a hack so our character can stop doing that annoying turn before stopping at the end of the path */
                this.Character.velocity = this.Path.GetPosition(this.CurrentParam) - this.Path.GetPosition(this.CurrentParam);
                return base.GetMovement();
            }
            this.CurrentParam = this.Path.GetParam(this.Character.position, this.CurrentParam);
            this.Target.position = this.Path.GetPosition(this.CurrentParam + this.PathOffset);      
            return base.GetMovement(); 
        }
    }
}
