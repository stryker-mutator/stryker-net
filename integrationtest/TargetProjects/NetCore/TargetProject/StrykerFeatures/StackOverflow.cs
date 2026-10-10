namespace TargetProject.StrykerFeatures
{
    public class StackOverflow
    {
        // Mutating the recursive step so it no longer approaches the base case makes execution
        // revisit this call site until the hit limit stops the test run.
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
