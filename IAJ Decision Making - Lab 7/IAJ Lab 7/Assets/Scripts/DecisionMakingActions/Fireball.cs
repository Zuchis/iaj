using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class Fireball : WalkToTargetAndExecuteAction
    {
        private int xpChange;
        private int manaChange;

        public Fireball(AutonomousCharacter character, GameObject target) : base("Fireball",character,target)
        {
            if (target.tag.Equals("Skeleton"))
            {
                this.xpChange = 5;
            }
            else if (target.tag.Equals("Orc")) //&& this.Character.GameManager.characterData.Mana >= 5)
            {
                this.xpChange = 10;
            }
            else if (target.tag.Equals("Dragon"))
            {
                this.xpChange = 0;
            }
            this.manaChange = -5;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.GAIN_XP_GOAL)
            {
                change += -this.xpChange;
            }
            return change;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Mana >= 5;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            return mana >= 5;
        }

        public override void Execute()
        {
            if (this.Target != null) { 
                base.Execute();
            this.Character.GameManager.Fireball(this.Target);
        }
        }
        

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var xpValue = worldModel.GetGoalValue(AutonomousCharacter.GAIN_XP_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GAIN_XP_GOAL, xpValue - this.xpChange);

            /*var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue - this.hpChange);*/
            //Are the two lines below needed??
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            worldModel.SetProperty(Properties.MANA, mana + this.manaChange);
            var xp = (int)worldModel.GetProperty(Properties.XP);
            worldModel.SetProperty(Properties.XP, xp + this.xpChange);
            
            //disables the target object so that it can't be reused again
            if (!this.Target.tag.Equals("Dragon"))
            {
                worldModel.SetProperty(this.Target.name, false);
            }            
        }

        public override double GetHValue(WorldModel m)
        {
            if (this.Target.tag.Equals("Dragon"))
                return 100.0;

            if (this.Target.tag.Equals("Orc"))
                return 0.0;

            if (this.Target.tag.Equals("Skeleton"))
                return 0.5;

            return 0.5;

        }

    }
}
