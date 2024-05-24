using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetProject.Constructs;

public class Csharp9
{
    // patern matching
    public bool IsLetter(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';

    public bool IsLetterOrSeparator(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '.' or ',';

    // null check
    public bool GetLength(string? text) => text is not null;

    // target type new expressions
    public void TargetTypeNewExpressions()
    {
        List<string> list = new() { "one", "two", "three" };
        Dictionary<int, string> dictionary = new() { [1] = "one", [2] = "two", [3] = "three" };
    }
}
