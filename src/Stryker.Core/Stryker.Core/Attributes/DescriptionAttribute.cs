using System;
using System.Diagnostics.CodeAnalysis;

namespace Stryker.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DescriptionAttribute : Attribute
    {
        public static readonly DescriptionAttribute Default = new DescriptionAttribute();

        public DescriptionAttribute() : this(string.Empty)
        {
        }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }

        public override bool Equals([NotNullWhen(true)] object obj) =>
            obj is DescriptionAttribute other && other.Description == Description;

        public override int GetHashCode() => Description?.GetHashCode() ?? 0;

        public override bool IsDefaultAttribute() => Equals(Default);
    }
}
