using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiasedPlayout : MCTS
    {
        public MCTSBiasedPlayout(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
        {
        }

        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            WorldModel child = initialPlayoutState.GenerateChildWorldModel();
            while (!child.IsTerminal())
            {   
                GOB.Action[] actions = child.GetExecutableActions();
                double[] actionIndexes = new double[actions.Length];
                double heuristicValue = 0.0;
                double accumulatedHeuristicValue = 0.0;
                double randomIndex;
                int chosenActionIndex = 0;
                for (int i = 0; i < actions.Length; i++)
                {

                    heuristicValue = actions[i].GetHValue(child);
                    accumulatedHeuristicValue += Math.Pow(Math.E, -heuristicValue);
                    actionIndexes[i] = accumulatedHeuristicValue;
                }
                
                randomIndex = this.RandomGenerator.NextDouble() * accumulatedHeuristicValue;
                //Debug.Log("Acumulated: " + accumulatedHeuristicValue);
                for (int i = 0; i < actions.Length; i++)
                {
                    if (randomIndex <= actionIndexes[i])
                    {
                        chosenActionIndex = i;
                        break;
                    }
                        
                }

                actions[chosenActionIndex].ApplyActionEffects(child);
                child.CalculateNextPlayer();
            }
            
            Reward r = new Reward();
            r.PlayerID = this.InitialNode.PlayerID;
            r.Value = child.GetScore();
            
            //Debug.Log("REWARD VALUE: " + r.Value);
            //Debug.Log("CHOSEN INDEX: " + chosenActionIndex);
            return r;//.GetRewardForNode(); 
        }

        /*
        protected MCTSNode Expand(WorldModel parentState, GOB.Action action)
        {
            //TODO: implement
            throw new NotImplementedException();
        }*/

       /* actionGetHValue(WorldModel m) ---> ADD THIS METHOD!

            gibbs: e ^h(a) / sum(e ^ -h(a))

            arrayOfOptions = array[actions.legth]

            for(int i = 0; i<actions.length; i++){
                e = math.pow(e, -action[i].getHValue(s));
                total += e;
            }

    returns a value that represents wheter it is good or bad to execute the action in the current world Model m
    h = 0->good value
            h > 1 -> bad value
            Example:
            Fireball.getHValue(worldModel m){
                if(this.target.tag.Equals("Dragon")
                    return 100.0f;
                else if(this.target.tag.Equals("Orc")
                    return 0.0f;
                else if(this.target.tag.Equals("Skeleton")
                    return 0.5f;
    }*/
    }
}
