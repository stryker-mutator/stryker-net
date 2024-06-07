namespace TargetProject.StrykerFeatures
{
    public class StrykerComments
    {
        public int Age { get; set; }

        // Stryker disable all: Because I want to
        public string IsExpired()
        {
            // all mutations should be ignored here
            return Age >= 30 ? "Yes" : "No";
        }

        // Stryker restore all
        public bool IsExpiredBool()
        {
            // no mutations should be ignored here
            return Age >= 30;
        }

        public string IsNotExpired()
        {
            // here the arrithmic operator should be mutated but string should be ignored
            // Stryker disable once string: Because I want to
            return Age < 30 ? "Yes" : "No";
        }

        public string String()
        {
            // this should not be ignored
            return "";
        }
    }
}
