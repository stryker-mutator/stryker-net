namespace TargetProject.StrykerFeatures
{
    public class StrykerComments
    {
        public int Age { get; set; }

        // Stryker disable all: Because I want to
        public string IsExpired()
        {
            return Age >= 30 ? "Yes" : "No";
        }

        // Stryker restore all
        public bool IsExpiredBool()
        {
            // this should be mutated
            return Age >= 30;
        }

        public string IsNotExpired()
        {
            // here the arrithmic operator should be mutated but string should not be mutated
            // Stryker disable once string: Because I want to
            return Age < 30 ? "Yes" : "No";
        }

        public bool IsNotExpiredBool()
        {
            // this should be mutated
            return Age < 30;
        }
    }
}
