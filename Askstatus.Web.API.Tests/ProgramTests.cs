using FluentAssertions;

namespace Askstatus.Web.API.Tests;

public class ProgramTests
{
    [Fact]
    public async Task Main_WithVersionArgument_PrintsVersionAndReturnsZero()
    {
        // Arrange
        var originalIntegrationTestRun = Program.IsIntegrationTestRun;
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Program.IsIntegrationTestRun = true;
        Console.SetOut(writer);

        int result;
        try
        {
            // Act
            result = await Program.Main(["--version"]);
        }
        finally
        {
            Console.SetOut(originalOut);
            Program.IsIntegrationTestRun = originalIntegrationTestRun;
        }

        // Assert
        result.Should().Be(0);
        writer.ToString().Trim().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Main_WithSeedArgumentAndMissingConfiguration_ReturnsOne()
    {
        // Arrange
        var originalIntegrationTestRun = Program.IsIntegrationTestRun;
        var originalDirectory = Directory.GetCurrentDirectory();
        var tempDirectory = Directory.CreateTempSubdirectory("askstatus-seed-test");
        Program.IsIntegrationTestRun = true;

        int result;
        try
        {
            Directory.SetCurrentDirectory(tempDirectory.FullName);

            // Act
            result = await Program.Main(["--seed"]);
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDirectory);
            Program.IsIntegrationTestRun = originalIntegrationTestRun;
            tempDirectory.Delete(true);
        }

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task Main_WithVersionArgumentAmongOtherArguments_ReturnsZeroBeforeSeeding()
    {
        // Arrange
        var originalIntegrationTestRun = Program.IsIntegrationTestRun;
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Program.IsIntegrationTestRun = true;
        Console.SetOut(writer);

        int result;
        try
        {
            // Act
            result = await Program.Main(["--seed", "--version"]);
        }
        finally
        {
            Console.SetOut(originalOut);
            Program.IsIntegrationTestRun = originalIntegrationTestRun;
        }

        // Assert
        result.Should().Be(0);
        writer.ToString().Should().NotContain("Migrating and Seeding data");
    }
}
