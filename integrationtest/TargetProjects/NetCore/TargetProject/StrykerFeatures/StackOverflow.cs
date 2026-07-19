namespace TargetProject.StrykerFeatures
{
    public class StackOverflow
    {
        // This method recurses towards its base case (count <= 0). Mutating the recursion so it no
        // longer approaches that base case - for example "count - 1" -> "count + 1", or emptying the
        // guard block - makes it recurse forever, overflowing the stack and crashing the test host.
        // A StackOverflowException cannot be caught and terminates the process, so under the Microsoft
        // Testing Platform runner the mutant is reported with the RuntimeError state.
        public int SumTo(int count)
        {
            if (count <= 0)
            {
                return 0;
            }

            return count + SumTo(count - 1);
        }
    }
}
