using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeSharpPlus
{
    public abstract class CompositeGroupWeighted : CompositeGroup
    {
        /// <summary>
        /// Returns a shuffle of the children composites similar to that of Fisher-Yates,
        /// but incorporating the node weights as opposed to the original, which uses uniform
        /// probabilities
        /// </summary>
        public List<Composite> ShuffleChildren()
        {
            Random randObj = new Random();

            // Iterate through the list and build a range list (0..n-1) and count
            // the weight total
            double total = 0.0;
            List<int> unusedchildren = new List<int>();
            for (int i = 0; i < this.Children.Count; i++)
            {
                total += this.Weights[i];
                unusedchildren.Add(i);
            }

            // Now, perform the shuffle
            List<Composite> order = new List<Composite>();
            while (unusedchildren.Count > 0)
            {
                double subtotal = 0.0;
                double next = randObj.NextDouble() * total;

                // The node we selected for the next child
                int selected = -1;

                // Look through all of the unused children remaining
                foreach (int unusedchild in unusedchildren)
                {
                    // If we can overtake the random value with the weight mass
                    // of this particular child, select it
                    double weight = this.Weights[unusedchild];
                    if ((subtotal + weight) >= next)
                    {
                        selected = unusedchild;
                        break;
                    }

                    // Otherwise, add to the subtotal and keep going
                    subtotal += weight;
                }

                // Add the child we selected
                order.Add(this.Children[selected]);

                // Remove the weight for de-facto renormalization
                total -= this.Weights[selected];

                // Remove the child from consideration
                unusedchildren.Remove(selected);
            }

            return order;
        }

        public List<double> Weights { get; set; }

        /// <summary>
        /// Adds default weights to a list of composite nodes
        /// </summary>
        public static CompositeWeight[] AddWeights(params Composite[] children)
        {
            List<CompositeWeight> weightedchildren = new List<CompositeWeight>();
            foreach(Composite child in children)
            {
                weightedchildren.Add(new CompositeWeight(child));
            }
            return weightedchildren.ToArray();
        }

        public CompositeGroupWeighted(params CompositeWeight[] weightedchildren)
        {
            // Initialize the base Children list and our new Weights list
            this.Children = new List<Composite>();
            this.Weights = new List<double>();

            // Unpack the pairs and store their individual values
            foreach (CompositeWeight weightedchild in weightedchildren)
            {
                this.Children.Add(weightedchild.Composite);
                this.Weights.Add(weightedchild.Weight);
            }
        }

        /// <summary>
        /// If we just get a list of children, we initialize them all with the default weight
        /// </summary>
        public CompositeGroupWeighted(params Composite[] children)
            : this(AddWeights(children))
        {
        }

        public void AddChild(CompositeWeight weightedchild)
        {
            if (weightedchild != null)
            {
                weightedchild.Composite.Parent = this;
                this.Children.Add(weightedchild.Composite);
                this.Weights.Add(weightedchild.Weight);
            }
        }

        public void InsertChild(int index, CompositeWeight weightedchild)
        {
            if (weightedchild != null)
            {
                weightedchild.Composite.Parent = this;
                this.Children.Insert(index, weightedchild.Composite);
                this.Weights.Insert(index, weightedchild.Weight);
            }
        }

        /// <summary>
        /// Overrides the original AddChild method to include the default weight
        /// </summary>
        public override void AddChild(Composite child)
        {
            this.AddChild(new CompositeWeight(child));
        }

        /// <summary>
        /// Overrides the original InsertChild method to include the default weight
        /// </summary>
        public override void InsertChild(int index, Composite child)
        {
            this.InsertChild(index, new CompositeWeight(child));
        }
    }

    /// <summary>
    /// A simple pair class for composites and weights, used for stochastic control nodes
    /// </summary>
    public class CompositeWeight
    {
        public Composite Composite { get; set; }
        public double Weight { get; set; }

        public CompositeWeight(Composite composite, double weight = 1.0f)
        {
            this.Composite = composite;
            this.Weight = weight;
        }
    }
}
