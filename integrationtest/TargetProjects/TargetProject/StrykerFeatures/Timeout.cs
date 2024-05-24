namespace TargetProject.StrykerFeatures
{
    public class Timeout
    {
        // this method will generate a timout mutation
        public void SomeLoop()
        {
            while (1 < 0)
            {
                ;
            }
        }
    }
}
