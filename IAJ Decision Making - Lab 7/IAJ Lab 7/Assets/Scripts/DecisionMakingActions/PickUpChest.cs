using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class PickUpChest : WalkToTargetAndExecuteAction
    {

        public PickUpChest(AutonomousCharacter character, GameObject target) : base("PickUpChest",character,target)
        {
        }


        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.GET_RICH_GOAL) change -= 5.0f;
            return change;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return true;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            return true;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.PickUpChest(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AutonomousCharacter.GET_RICH_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GET_RICH_GOAL, goalValue - 5.0f);

            var money = (int)worldModel.GetProperty(Properties.MONEY);
            worldModel.SetProperty(Properties.MONEY, money + 5);

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public override double GetHValue(WorldModel m)
        {
            var hp = (int)m.GetProperty(Properties.HP);
            var dragonAlive = (bool)m.GetProperty("Dragon");
            var orc1Alive = (bool)m.GetProperty("Orc");
            var orc2Alive = (bool)m.GetProperty("Orc (1)");

            if (this.Target.name.Equals("Chest (4)") && dragonAlive && hp <= 20)
            {
                //Debug.Log("ENTREI NO 1 -> " + this.Target.name.Equals("Chest (4)"));
                return 1000.0;
            }

            if (this.Target.name.Equals("Chest (1)") && orc2Alive && hp <= 10)
            {
                //Debug.Log("ENTREI NO 2");
                return 1000.0;
            }

            if (this.Target.name.Equals("Chest (2)") && orc1Alive && hp <= 10)
            {
                //Debug.Log("ENTREI NO 3");
                return 1000.0;
            }

            return 0.4;

        }

    }
}
