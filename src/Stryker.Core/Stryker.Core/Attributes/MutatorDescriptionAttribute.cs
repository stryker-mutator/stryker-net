using System;
using System.Diagnostics.CodeAnalysis;

namespace Stryker.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MutatorDescriptionAttribute : Attribute
    {
        public static readonly MutatorDescriptionAttribute Default = new();

        public MutatorDescriptionAttribute() : this(string.Empty)
        {
        }

        public MutatorDescriptionAttribute(string description) => Description = description;

        public string Description { get; set; }

        public override bool Equals([NotNullWhen(true)] object obj) =>
            obj is MutatorDescriptionAttribute other && other.Description == Description;

        public override int GetHashCode() => Description?.GetHashCode() ?? 0;

        public override bool IsDefaultAttribute() => Equals(Default);
    }
}
