namespace TargetProject.StrykerFeatures;

// all but one mutation should be killed here. one surviving mutant
public class KilledMutants
{
    public int Age { get; set; }

    public string IsExpired()
    {
        return Age >= 30 ? "Yes" : "No";
    }

    public bool IsExpiredBool()
    {
        return Age >= 30;
    }
}
