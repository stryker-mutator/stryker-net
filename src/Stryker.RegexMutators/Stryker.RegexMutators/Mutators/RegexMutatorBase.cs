using RegexParser.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.RegexMutators.Mutators
{
    /// <summary>
    /// Mutators can implement this class to check the type of the node and cast the node to the expected type.
    /// Implementing this class is not obligatory for mutators.
    /// </summary>
    /// <typeparam name="T">The type of RegexNode to cast to</typeparam>
    public abstract class RegexMutatorBase<T>
        where T : RegexNode
    {
        public IEnumerable<RegexMutation> Mutate(RegexNode node, RegexNode root)
        {
            if (node is T)
            {
                return ApplyMutations(node as T, root);
            }

            return Enumerable.Empty<RegexMutation>();
        }

        /// <summary>
        /// Apply the given mutations to a single RegexNode
        /// </summary>
        /// <param name="node">The node to mutate</param>
        /// <returns>One or more mutations</returns>
        public abstract IEnumerable<RegexMutation> ApplyMutations(T node, RegexNode root);
    }
}
