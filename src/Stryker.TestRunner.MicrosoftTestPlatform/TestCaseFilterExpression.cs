using System.Text;
using Stryker.Abstractions.Exceptions;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Evaluates a <c>test-case-filter</c> against discovered test nodes, for test applications that do not
/// register the MTP <c>--filter</c> option (for example xUnit v3 and TUnit).
/// Supports the VSTest filter syntax: <c>Property=Value</c>, <c>!=</c>, <c>~</c> (contains), <c>!~</c>,
/// combined with <c>&amp;</c>, <c>|</c> and parentheses, with <c>\</c> escaping a special character.
/// A condition without a property and operator is read as <c>FullyQualifiedName~Value</c>.
/// Values are compared ignoring case, as VSTest does.
/// </summary>
internal sealed class TestCaseFilterExpression
{
    private static readonly string[] SupportedProperties = ["FullyQualifiedName", "Name", "DisplayName", "ClassName"];

    private readonly Func<TestNode, bool> _predicate;

    private TestCaseFilterExpression(Func<TestNode, bool> predicate) => _predicate = predicate;

    public bool Matches(TestNode test) => _predicate(test);

    public static TestCaseFilterExpression Parse(string filter)
    {
        var parser = new Parser(filter);
        var predicate = parser.ParseOr();
        if (!parser.AtEnd)
        {
            throw Invalid(filter, $"unexpected '{parser.Current}' at position {parser.Position}");
        }

        return new TestCaseFilterExpression(predicate);
    }

    private static InputException Invalid(string filter, string reason) =>
        new($"The test-case-filter '{filter}' is invalid: {reason}.",
            "Use the VSTest filter syntax, e.g. \"FullyQualifiedName~MyTests&Name!=SlowTest\".");

    private static Func<TestNode, string> GetProperty(string filter, string name) => name.ToLowerInvariant() switch
    {
        "fullyqualifiedname" => test => new MtpTestCase(test).FullyQualifiedName,
        "name" or "displayname" => test => test.DisplayName,
        "classname" => test => test.LocationType ?? string.Empty,
        _ => throw new InputException(
            $"The test-case-filter property '{name}' is not supported for test applications without the MTP '--filter' option.",
            $"Supported properties are {string.Join(", ", SupportedProperties)}. Filter: '{filter}'."),
    };

    private sealed class Parser(string filter)
    {
        private int _position;

        public bool AtEnd
        {
            get
            {
                SkipWhitespace();
                return _position >= filter.Length;
            }
        }

        public char Current => filter[_position];

        public int Position => _position;

        public Func<TestNode, bool> ParseOr()
        {
            var left = ParseAnd();
            while (TryConsume('|'))
            {
                var first = left;
                var second = ParseAnd();
                left = test => first(test) || second(test);
            }

            return left;
        }

        private Func<TestNode, bool> ParseAnd()
        {
            var left = ParsePrimary();
            while (TryConsume('&'))
            {
                var first = left;
                var second = ParsePrimary();
                left = test => first(test) && second(test);
            }

            return left;
        }

        private Func<TestNode, bool> ParsePrimary()
        {
            if (TryConsume('('))
            {
                var inner = ParseOr();
                if (!TryConsume(')'))
                {
                    throw Invalid(filter, "missing ')'");
                }

                return inner;
            }

            return ParseCondition();
        }

        private Func<TestNode, bool> ParseCondition()
        {
            var text = ReadCondition();
            var (name, op, value) = SplitCondition(text);
            if (value.Length == 0)
            {
                throw Invalid(filter, $"condition '{text}' has no value");
            }

            var property = GetProperty(filter, name);
            return op switch
            {
                "=" => test => string.Equals(property(test), value, StringComparison.OrdinalIgnoreCase),
                "!=" => test => !string.Equals(property(test), value, StringComparison.OrdinalIgnoreCase),
                "~" => test => property(test).Contains(value, StringComparison.OrdinalIgnoreCase),
                _ => test => !property(test).Contains(value, StringComparison.OrdinalIgnoreCase),
            };
        }

        // Reads one condition up to the next unescaped '&', '|' or ')', keeping escapes for SplitCondition.
        private string ReadCondition()
        {
            SkipWhitespace();
            var start = _position;
            while (_position < filter.Length && filter[_position] is not ('&' or '|' or ')' or '('))
            {
                _position += filter[_position] == '\\' && _position + 1 < filter.Length ? 2 : 1;
            }

            var text = filter[start.._position].Trim();
            if (text.Length == 0)
            {
                throw Invalid(filter, $"expected a condition at position {start}");
            }

            return text;
        }

        private (string Name, string Op, string Value) SplitCondition(string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '\\')
                {
                    i++;
                    continue;
                }

                if (text[i] is '=' or '~')
                {
                    var negated = i > 0 && text[i - 1] == '!';
                    var name = text[..(negated ? i - 1 : i)].Trim();
                    var op = (negated ? "!" : string.Empty) + text[i];
                    if (name.Length == 0)
                    {
                        throw Invalid(filter, $"condition '{text}' has no property name");
                    }

                    return (name, op, Unescape(text[(i + 1)..].Trim()));
                }
            }

            return ("FullyQualifiedName", "~", Unescape(text));
        }

        private static string Unescape(string value)
        {
            var builder = new StringBuilder(value.Length);
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] == '\\' && i + 1 < value.Length)
                {
                    i++;
                }

                builder.Append(value[i]);
            }

            return builder.ToString();
        }

        private bool TryConsume(char expected)
        {
            SkipWhitespace();
            if (_position < filter.Length && filter[_position] == expected)
            {
                _position++;
                return true;
            }

            return false;
        }

        private void SkipWhitespace()
        {
            while (_position < filter.Length && char.IsWhiteSpace(filter[_position]))
            {
                _position++;
            }
        }
    }
}
