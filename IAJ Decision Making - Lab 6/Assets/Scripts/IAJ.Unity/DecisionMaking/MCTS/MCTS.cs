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
            MCTSNode bestChild;
                      
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
           
        }

        public GOB.Action Run()
        {
            MCTSNode selectedNode;
            Reward reward;

            var startTime = Time.realtimeSinceStartup;
            this.CurrentIterationsInFrame = 0;

            //create root node v0 for state s0

            // while currentIter < maxIterations && currentIterationInFrame < maxIterationsPerFrame
            while (CurrentIterations < MaxIterations && CurrentIterationsInFrame < MaxIterationsProcessedPerFrame)
            {
                //v1 = this.Selection(v0);
                //reward = this.Playout(Selection(v1));
                //this.Backpropagate(v1, reward);
            }
            //return BestChild(v0);
        }

        private Reward Playout(WorldModel initialPlayoutState)
        {
            while (!initialPlayoutState.IsTerminal())
            {
                // s = initialPlayoutState
                // actions = s.getNextExecutableAction();
                // randomIndex = this.Random.next(actions.Length)
                GOB.Action a = initialPlayoutState.GetNextAction();
                a.ApplyActionEffects(initialPlayoutState);
            }
            Reward r = new Reward();
            r.Value = initialPlayoutState.GetScore();   //??????????
            // return new Reward(PlayerID, s.getScore());
            return new Reward();

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
            foreach (var child in node.ChildNodes)
            {
                //child.
            }
            // value = child.Q / child.N + c * sqrt(log(n.N / child.N))
            
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        private MCTSNode BestChild(MCTSNode node)
        {
           
        }
    }
}
