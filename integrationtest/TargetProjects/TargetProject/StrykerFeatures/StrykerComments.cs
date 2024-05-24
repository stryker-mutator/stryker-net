namespace TargetProject.StrykerFeatures
{
    public class StrykerComments
    {
        public DateTime ExpireDate { get; set; }

        // Stryker disable all: Because I want to
        public string IsExpired()
        {
            return ExpireDate > DateTime.Now ? "Yes" : "No";
        }

        // Stryker restore all
        public bool IsExpiredBool()
        {
            // this should be mutated
            return ExpireDate > DateTime.Now;
        }

        public string IsNotExpired()
        {
            // here the arrithmic operator should be mutated but string should not be mutated
            // Stryker disable once string: Because I want to
            return ExpireDate < DateTime.Now ? "Yes" : "No";
        }

        public bool IsNotExpiredBool()
        {
            // this should be mutated
            return ExpireDate < DateTime.Now;
        }
    }
}
