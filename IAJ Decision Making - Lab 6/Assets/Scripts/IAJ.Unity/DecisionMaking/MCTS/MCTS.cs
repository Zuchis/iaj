using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTS
    {
        public const float C = 1.4f;
        public bool InProgress { get; private set; }
        public int MaxIterations { get; set; }
        public int MaxIterationsProcessedPerFrame { get; set; } // different for maximum number of iterations, this just separates de algorithms per frames
        public int MaxPlayoutDepthReached { get; private set; }
        public int MaxSelectionDepthReached { get; private set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<GOB.Action> BestActionSequence { get; private set; }


        private int CurrentIterations { get; set; }
        private int CurrentIterationsInFrame { get; set; }
        private int CurrentDepth { get; set; }

        private CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        private MCTSNode InitialNode { get; set; }
        private System.Random RandomGenerator { get; set; }
        
        

        public MCTS(CurrentStateWorldModel currentStateWorldModel)
        {
            this.InProgress = false;
            this.CurrentStateWorldModel = currentStateWorldModel;
            this.MaxIterations = 100;
            this.MaxIterationsProcessedPerFrame = 10;
            this.RandomGenerator = new System.Random();
        }


        public void InitializeMCTSearch()
        {
            this.MaxPlayoutDepthReached = 0;
            this.MaxSelectionDepthReached = 0;
            this.CurrentIterations = 0;
            this.CurrentIterationsInFrame = 0;
            this.TotalProcessingTime = 0.0f;
            this.CurrentStateWorldModel.Initialize();
            this.InitialNode = new MCTSNode(this.CurrentStateWorldModel)
            {
                Action = null,
                Parent = null,
                PlayerID = 0
            };
            this.InProgress = true;
            this.BestFirstChild = null;
            this.BestActionSequence = new List<GOB.Action>();
        }

        private MCTSNode Selection(MCTSNode initialNode)
        {
            GOB.Action nextAction;
            MCTSNode currentNode = initialNode;
            //MCTSNode bestChild;
                      
            while (!currentNode.State.IsTerminal())
            {
                nextAction = currentNode.State.GetNextAction();
                if(nextAction != null)
                {
                    return this.Expand(currentNode, nextAction);
                }
                else
                {
                    currentNode = BestUCTChild(currentNode);
                }
                // var action = n.state.getNextAction();
                // if (action != null) // there is still room for expantion
                //     return this.expand(n,action) 
                // else 
                // n = selectBestUCTChild(n);
            }
            return currentNode;
        }

        private MCTSNode Expand(MCTSNode parent, GOB.Action action)
        {
            MCTSNode child = new MCTSNode(parent.State.GenerateChildWorldModel());
            child.Parent = parent;
            parent.ChildNodes.Add(child);
            child.Action = action;
            action.ApplyActionEffects(child.State);
            return child;           
        }

        public GOB.Action Run()
        {
            MCTSNode selectedNode;
            Reward reward;

            var startTime = Time.realtimeSinceStartup;
            this.CurrentIterationsInFrame = 0;
            //create root node v0 for state s0
            MCTSNode v0 = this.InitialNode;
            MCTSNode v1;

            // while currentIter < maxIterations && currentIterationInFrame < maxIterationsPerFrame
            while (CurrentIterations < MaxIterations && CurrentIterationsInFrame < MaxIterationsProcessedPerFrame)
            {
                v1 = this.Selection(v0);
                reward = this.Playout(v1.State);
                this.Backpropagate(v1, reward);
                this.CurrentIterations += 1;
            }
            return this.BestChild(this.InitialNode).Action;
        }

        private Reward Playout(WorldModel initialPlayoutState)
        {
            int randomIndex;

            while (!initialPlayoutState.IsTerminal())
            {
                // s = initialPlayoutState
                // actions = s.getNextExecutableAction();
                // randomIndex = this.Random.next(actions.Length)
                GOB.Action[] actions = initialPlayoutState.GetExecutableActions();
                randomIndex = this.RandomGenerator.Next(actions.Length);
                //GOB.Action a = initialPlayoutState.GetNextAction();
                GOB.Action randomAction = actions[randomIndex];
                randomAction.ApplyActionEffects(initialPlayoutState);
            }
            Reward r = new Reward();
            r.PlayerID = this.InitialNode.PlayerID;
            r.Value = initialPlayoutState.GetScore();
            //r.Value = initialPlayoutState.GetScore();   //??????????
            return r;
            //Random rand = new Random((int)DateTime.Now.Ticks);      
        }

        /*
         biasedPlayout(){
            actionGetHValue(WorldModel m) ---> ADD THIS METHOD!

            gibbs: e ^h(a) / sum(e ^ -h(a))

            arrayOfOptions = array[actions.legth]

            for(int i = 0; i < actions.length; i++){
                e = math.pow(e, -action[i].getHValue(s));
                total += e;
            }

            returns a value that represents wheter it is good or bad to execute the action in the current world Model m
            h = 0 -> good value
            h > 1 -> bad value
            Example:
            Fireball.getHValue(worldModel m){
                if(this.target.tag.Equals("Dragon")
                    return 100.0f;
                else if(this.target.tag.Equals("Orc")
                    return 0.0f;
                else if(this.target.tag.Equals("Skeleton")
                    return 0.5f;
            }


         }
         */

        private void Backpropagate(MCTSNode node, Reward reward)
        {
            while (node.Parent != null)
            {
                node.N += 1;
                node.Q += reward.Value; // instead of 1 : reward(node, Player(Parent(node)))
                node = node.Parent;
           } 
        }


        //gets the best child of a node, using the UCT formula
        private MCTSNode BestUCTChild(MCTSNode node)
        {
            float uctValue;
            float childEstimatedValue;
            float bestValue = 0.0f;
            float explorationFactor;
            MCTSNode chosenChild = null;
            foreach (var child in node.ChildNodes)
            {
                // value = child.Q / child.N + c * sqrt(log(n.N / child.N))
                childEstimatedValue = child.Q / child.N;
                explorationFactor = MCTS.C * Mathf.Sqrt((Mathf.Log10(node.N) / Mathf.Log10(Mathf.Epsilon)) / child.N);
                uctValue = childEstimatedValue + explorationFactor;
                if (uctValue > bestValue)
                {
                    bestValue = uctValue;
                    chosenChild = child;
                }
            }
            return chosenChild;
            
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        private MCTSNode BestChild(MCTSNode node)
        {
            float childEstimatedValue;
            float bestValue = 0.0f;
            MCTSNode chosenChild = null;
            foreach (var child in node.ChildNodes)
            {
                // value = child.Q / child.N 
                childEstimatedValue = child.Q / child.N;
                if (childEstimatedValue > bestValue)
                {
                    bestValue = childEstimatedValue;
                    chosenChild = child;
                }
            }
            return chosenChild;

        }
    }
}
