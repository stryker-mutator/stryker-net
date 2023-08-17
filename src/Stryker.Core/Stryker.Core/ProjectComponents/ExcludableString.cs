namespace Stryker.Core.ProjectComponents
{
    public class ExcludableString
    {
        public ExcludableString(string s)
        {
            IsExclude = s.StartsWith('!');
            Pattern = IsExclude ? s[1..] : s;
        }

        public bool IsExclude { get; }

        public string Pattern { get; }

        public static ExcludableString Parse(string s) => new(s);
    }
}
