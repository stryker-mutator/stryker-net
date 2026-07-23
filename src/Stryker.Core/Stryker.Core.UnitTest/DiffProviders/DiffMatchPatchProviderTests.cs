using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.DiffProviders;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.UnitTest.DiffProviders;

[TestClass]
public class DiffMatchPatchProviderTests : TestBase
{
    [TestMethod]
    public void GetContentDiff_ReturnsSameLocation_WhenSourceIsUnchanged()
    {
        // Arrange
        var source = "class C\n{\n    void M()\n    {\n        var foo = \"bar\";\n    }\n}";
        var target = new DiffMatchPatchProvider();
        var location = new Location
        {
            Start = new Position { Line = 5, Column = 19 },
            End = new Position { Line = 5, Column = 24 }
        };

        // Act
        var diff = target.GetContentDiff(source, source);
        var mapped = diff.TryMapLocation(location, out var newLocation);

        // Assert
        mapped.ShouldBeTrue();
        newLocation.Start.Line.ShouldBe(location.Start.Line);
        newLocation.Start.Column.ShouldBe(location.Start.Column);
        newLocation.End.Line.ShouldBe(location.End.Line);
        newLocation.End.Column.ShouldBe(location.End.Column);
    }

    [TestMethod]
    public void GetContentDiff_ShiftsLocation_WhenLinesAreInsertedBeforeIt()
    {
        // Arrange
        var oldSource = "class C\n{\n    void M()\n    {\n        var foo = \"bar\";\n    }\n}";
        var newSource = "using System;\n\n" + oldSource;
        var target = new DiffMatchPatchProvider();
        var location = new Location
        {
            Start = new Position { Line = 5, Column = 19 },
            End = new Position { Line = 5, Column = 24 }
        };

        // Act
        var diff = target.GetContentDiff(oldSource, newSource);
        var mapped = diff.TryMapLocation(location, out var newLocation);

        // Assert
        mapped.ShouldBeTrue();
        newLocation.Start.Line.ShouldBe(location.Start.Line + 2);
        newLocation.Start.Column.ShouldBe(location.Start.Column);
    }

    [TestMethod]
    public void GetContentDiff_ReturnsFalse_WhenLocationOverlapsAChange()
    {
        // Arrange
        var oldSource = "class C\n{\n    void M()\n    {\n        var foo = \"bar\";\n    }\n}";
        var newSource = "class C\n{\n    void M()\n    {\n        var foo = \"baz\";\n    }\n}";
        var target = new DiffMatchPatchProvider();
        var location = new Location
        {
            Start = new Position { Line = 5, Column = 19 },
            End = new Position { Line = 5, Column = 24 }
        };

        // Act
        var diff = target.GetContentDiff(oldSource, newSource);
        var mapped = diff.TryMapLocation(location, out var newLocation);

        // Assert
        mapped.ShouldBeFalse();
        newLocation.ShouldBeNull();
    }

    [TestMethod]
    public void ScanDiff_ThrowsNotSupported()
    {
        // Arrange
        var target = new DiffMatchPatchProvider();

        // Act
        Action act = () => target.ScanDiff();

        // Assert
        act.ShouldThrow<NotSupportedException>();
    }

    [TestMethod]
    public void Tests_ThrowsNotSupported()
    {
        // Arrange
        var target = new DiffMatchPatchProvider();

        // Act
        Action act = () => _ = target.Tests;

        // Assert
        act.ShouldThrow<NotSupportedException>();
    }
}
