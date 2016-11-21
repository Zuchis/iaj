using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 2;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModel InitialWorldModel { get; set; }
        private List<Goal> Goals { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionPerLevel { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth {  get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> actions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 200;
            this.Goals = goals;
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;                              //slide 98 line 5
            this.Models = new WorldModel[MAX_DEPTH + 1];        //slide 98 line 1
            this.Models[0] = this.InitialWorldModel;            //slide 98 line 4
            this.ActionPerLevel = new Action[MAX_DEPTH];        //slide 98 line 2
            this.BestActionSequence = new Action[MAX_DEPTH];    //slide 98 line 2
            this.BestAction = null;                             //slide 98 line 6
            this.BestDiscontentmentValue = float.MaxValue;      //slide 98 line 7
            this.InitialWorldModel.Initialize();
        }



        /**
        * CHOOSE ACTION
        * Missing following functionalities: 
        *  - Limit the Number of Action Combinations to be processed per frame (basically the number of actions that reach the last level) 
        *  - TotalActionsCombinationProcessed 
        *  - Best Action Sequence (and not just the best first action)
        **/
        public Action ChooseAction()        
        {
            Action nextAction;
            float currentValue;
            var processedActions = 0;
            var startTime = Time.realtimeSinceStartup;
            while (processedActions < ActionCombinationsProcessedPerFrame)
            {
                if (CurrentDepth >= 0)
                {       
                    if (CurrentDepth >= MAX_DEPTH)
                    {
                        processedActions++;  
                        currentValue = Models[CurrentDepth].CalculateDiscontentment(Goals);
                        if (currentValue < BestDiscontentmentValue)
                        {
                            BestDiscontentmentValue = currentValue;
                            BestAction = ActionPerLevel[0];
                            for (int i = 0; i < ActionPerLevel.Length; i++)
                            {
                                this.BestActionSequence[i] = ActionPerLevel[i];
                            }
                        }
                        CurrentDepth -= 1;
                        continue;
                    }
                    nextAction = Models[CurrentDepth].GetNextAction();
                    if (nextAction != null)
                    {
                        Models[CurrentDepth + 1] = Models[CurrentDepth].GenerateChildWorldModel();
                        nextAction.ApplyActionEffects(Models[CurrentDepth + 1]);
                        ActionPerLevel[CurrentDepth] = nextAction;
                        CurrentDepth += 1;
                    }
                    else
                    {
                        CurrentDepth -= 1;
                    }
                }
                else
                {
                    this.InProgress = false;
                    break;
                }
            }
            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;      //Total processing time     2.b)
            TotalActionCombinationsProcessed += processedActions;
            return this.BestAction;
        }
    }
}
