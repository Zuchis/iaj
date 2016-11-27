using Assets.Scripts.GameManager;
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
        public int MaxIterationsProcessedPerFrame { get; set; }
        public int MaxPlayoutDepthReached { get; private set; }
        public int MaxSelectionDepthReached { get; private set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<GOB.Action> BestActionSequence { get; private set; }


        protected int CurrentIterations { get; set; }
        protected int CurrentIterationsInFrame { get; set; }
        protected int CurrentDepth { get; set; }

        protected CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        protected MCTSNode InitialNode { get; set; }
        protected System.Random RandomGenerator { get; set; }
        
        

        public MCTS(CurrentStateWorldModel currentStateWorldModel)
        {
            this.InProgress = false;
            this.CurrentStateWorldModel = currentStateWorldModel;
            this.MaxIterations = 100;
            this.MaxIterationsProcessedPerFrame = 10;
            this.RandomGenerator = new System.Random(); //new System.Random(DateTime.Now.TimeOfDay.Milliseconds);
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
            MCTSNode previousNode;
            while (!currentNode.State.IsTerminal())
            {
                nextAction = currentNode.State.GetNextAction();
                
                if (nextAction != null)
                {
                    return this.Expand(currentNode, nextAction);
                }
                else
                {
                    previousNode = currentNode;
                    currentNode = BestUCTChild(currentNode);
                    //if (currentNode == null) return previousNode;   //FILTHY HACK
                }
            }
            return currentNode;
        }



        private MCTSNode Expand(MCTSNode parent, GOB.Action action)
        {
            WorldModel state = parent.State.GenerateChildWorldModel();
            MCTSNode child = new MCTSNode(state);

            child.Parent = parent;
            action.ApplyActionEffects(child.State);
            child.State.CalculateNextPlayer();

            child.Action = action;
            parent.ChildNodes.Add(child);

            return child;
        }



        public GOB.Action Run()
        {
            //MCTSNode selectedNode;
            Reward reward;
            var startTime = Time.realtimeSinceStartup;
            this.CurrentIterationsInFrame = 0;
            MCTSNode v0 = this.InitialNode;     //create root node v0 for state s0
            MCTSNode v1;

            while (CurrentIterations < MaxIterations && CurrentIterationsInFrame < MaxIterationsProcessedPerFrame)
            {
                v1 = this.Selection(v0);
                reward = this.Playout(v1.State);
                this.Backpropagate(v1, reward);
                this.CurrentIterations++;
                this.CurrentIterationsInFrame++;
            }
            this.TotalProcessingTime = Time.realtimeSinceStartup - startTime;
            if(this.CurrentIterations < this.MaxIterations)
            {
                this.InProgress = false;
            }

            this.BestFirstChild = this.BestChild(this.InitialNode);

            this.BestActionSequence.Clear();

            var auxNode = this.BestFirstChild;

            while (true)
            {
                if (auxNode == null || auxNode.State.IsTerminal())
                {
                    break;
                }
                this.BestActionSequence.Add(auxNode.Action);
                auxNode = BestChild(auxNode);
            }

            if (this.BestFirstChild == null)
            {
                return null;
            }

            return this.BestFirstChild.Action;

            //if (!v0.State.IsTerminal()) //hack to display victory screen
            //{
            //    GOB.Action a = this.BestChild(v0).Action;
            //    if (!this.BestActionSequence.Contains(a))
            //    {

            //        BestActionSequence.Add(a);
            //        //Debug.Log(BestActionSequence.Count);
            //    }
            //    return a;
            //}
            //return null; //hack above ends
        }



        protected virtual Reward Playout(WorldModel initialPlayoutState)
        {
            int randomIndex;
            GOB.Action action;
            GOB.Action[] actions;
            WorldModel wm = initialPlayoutState.GenerateChildWorldModel();  //nao alterar estado inicial, fazer copia. De resto ta bom
            while (!wm.IsTerminal())
            {
                actions = wm.GetExecutableActions();
                if (actions.Length == 0)
                    continue;
                randomIndex = this.RandomGenerator.Next(0,actions.Length);
                action = actions[randomIndex];
                action.ApplyActionEffects(wm);
                wm.CalculateNextPlayer();
            }
            Reward r = new Reward();
            r.PlayerID = wm.GetNextPlayer();
            r.Value = wm.GetScore();
            return r;
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
                node.Q += reward.GetRewardForNode(node);
                node = node.Parent;
            }
        }




        //gets the best child of a node, using the UCT formula
        private MCTSNode BestUCTChild(MCTSNode node)
        {
            List<MCTSNode> children = node.ChildNodes;
            MCTSNode best, currentChild;
            float ui;
            double uct;
            double bestUCT = -1;
            best = null;
            MCTSNode chosenChild = null;
            for (int i = 0; i < children.Count; i++)
            {
                currentChild = children[i];
                ui = currentChild.Q / currentChild.N;
                uct = ui + C * Math.Sqrt(Math.Log(currentChild.Parent.N) / currentChild.N);
                if (uct > bestUCT)
                {
                    bestUCT = uct;
                    best = currentChild;
                }
            }
            return best;
        }



        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        private MCTSNode BestChild(MCTSNode node)
        {
            List<MCTSNode> children = node.ChildNodes;
            MCTSNode best, currentChild;
            float ui;
            double uct;
            double bestUCT = -1;
            best = null;
            MCTSNode chosenChild = null;
            for (int i = 0; i < children.Count; i++)
            {
                currentChild = children[i];
                ui = currentChild.Q / currentChild.N;
                uct = ui + Math.Sqrt(Math.Log(currentChild.Parent.N) / currentChild.N);
                if (uct > bestUCT)
                {
                    bestUCT = uct;
                    best = currentChild;
                }
            }
            return best;
        }


        protected GOB.Action BestFinalAction(MCTSNode node)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

    }
}
