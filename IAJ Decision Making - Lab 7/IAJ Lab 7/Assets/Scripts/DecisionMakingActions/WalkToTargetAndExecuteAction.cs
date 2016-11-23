﻿using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts.DecisionMakingActions
{
    public abstract class WalkToTargetAndExecuteAction : Action
    {
        protected AutonomousCharacter Character { get; set; }

        protected GameObject Target { get; set; }

        protected WalkToTargetAndExecuteAction(string actionName, AutonomousCharacter character, GameObject target) : base(actionName+"("+target.name+")")
        {
            this.Character = character;
            this.Target = target;
        }

        public override float GetDuration()
        {
            //assume a velocity of 20.0f/s to get to the target
            //return (this.Target.transform.position - this.Character.Character.KinematicData.position).magnitude / this.Character.Character.MaxSpeed; //20.0f;
            GatewayHeuristic g = (GatewayHeuristic)this.Character.GameManager.autonomousCharacter.AStarPathFinding.Heuristic;
            return g.H(this.Character.Character.KinematicData.position, this.Target.transform.position) / this.Character.Character.MaxSpeed;
        }

        public override float GetDuration(WorldModel worldModel)
        {
            //assume a velocity of 20.0f/s to get to the target
            var position = (Vector3)worldModel.GetProperty(Properties.POSITION);
            //return (this.Target.transform.position - position).magnitude / this.Character.Character.MaxSpeed; //20.0f;  
            GatewayHeuristic g = (GatewayHeuristic)this.Character.GameManager.autonomousCharacter.AStarPathFinding.Heuristic;
            return g.H(this.Character.Character.KinematicData.position, this.Target.transform.position) / this.Character.Character.MaxSpeed;         
        }

        public override float GetGoalChange(Goal goal)
        {
            if (goal.Name == AutonomousCharacter.BE_QUICK_GOAL)
            {
                return this.GetDuration();
            }
            else return 0;
        }

        public override bool CanExecute()
        {
            return this.Target != null;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (this.Target == null) return false;
            var targetEnabled = (bool)worldModel.GetProperty(this.Target.name);
            return targetEnabled;
        }

        public override void Execute()
        {
            this.Character.StartPathfinding(this.Target.transform.position);
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            var duration = this.GetDuration(worldModel);

            var quicknessValue = worldModel.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, quicknessValue + duration*0.1f);

            var time = (float)worldModel.GetProperty(Properties.TIME);
            worldModel.SetProperty(Properties.TIME, time + duration);

            worldModel.SetProperty(Properties.POSITION, Target.transform.position);
        }

    }
}