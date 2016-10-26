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
            //this.CurrentParam = 0f; //????????
            //don't forget to set all properties
            //arrive properties
                      
        }

        public override MovementOutput GetMovement()
        {
            if (this.Path == null) return EmptyMovementOutput;

            if (this.Path.PathEnd(this.CurrentParam)) { /*this.Target.position = this.Path.GetPosition(this.CurrentParam);*/ return base.GetMovement(); }//return EmptyMovementOutput; 

            this.CurrentParam = this.Path.GetParam(this.Character.position, this.CurrentParam);
            Debug.Log("CURRENT PARAM = " + CurrentParam);
            this.Target.position = this.Path.GetPosition(this.CurrentParam + this.PathOffset);
            //this.Target.position.y = 0;
            //this.CurrentParam = this.Path.GetParam(Target.position, CurrentParam);
            
            return base.GetMovement(); 
        }
    }
}
