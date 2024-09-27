using FluentAssertions;
using NetArchTest.Rules;

namespace Askstatus.Architecture.Tests
{
    public class ArchitectureTests
    {
        private const string DomainNamespace = "Askstatus.Domain";
        private const string ApplicationNamespace = "Askstatus.Application";
        private const string InfrastructureNamespace = "Askstatus..Infrastructure";
        private const string WebAPINamespace = "Askstatus.Web.API";
        private const string WebUiNamespace = "Askstatus.Web.App";

        private const string ApplicationTestsNamespace = "Askstatus.Application.Tests";
        private const string ArchitectureTestsNamespace = "Askstatus.Architecture.Tests";
        private const string InfrastructureTestsNamespace = "Askstatus.Infrastructure.Tests";
        //private const string PersistenceTestsNamespace = "Askstatus.Persistence.Tests";
        //private const string WebAPITestsNamespace = "Askstatus.Web.API.Tests";

        [Fact]
        public void Domain_Shuld_Not_HaveDependencyOnOtherProjects()
        {
            var assembly = typeof(Askstatus.Domain.AssemblyReference).Assembly;

            var otherProjects = new[]
            {
                ApplicationNamespace,
                InfrastructureNamespace,
                WebUiNamespace,
                ArchitectureTestsNamespace,
                ApplicationTestsNamespace,
                InfrastructureTestsNamespace,
            };

            var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(otherProjects).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Shuld_Not_HaveDependencyOnOtherProjects()
        {
            var assembly = typeof(Askstatus.Application.AssemblyReference).Assembly;

            var otherProjects = new[]
            {
                InfrastructureNamespace,
                WebAPINamespace,
                WebUiNamespace,
                ArchitectureTestsNamespace,
                ApplicationTestsNamespace,
                InfrastructureTestsNamespace,
            };

            var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(otherProjects).GetResult();

            result.IsSuccessful.Should().BeTrue();

        }

        [Fact]
        public void Infrastructure_Shuld_Not_HaveDependencyOnOtherProjects()
        {
            var assembly = typeof(Askstatus.Infrastructure.AssemblyReference).Assembly;

            var otherProjects = new[]
            {
                InfrastructureNamespace,
                WebAPINamespace,
                WebUiNamespace,
                ArchitectureTestsNamespace,
                ApplicationTestsNamespace,
                InfrastructureTestsNamespace,
            };

            var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(otherProjects).GetResult();

            result.IsSuccessful.Should().BeTrue();

        }

        [Fact]
        public void WebAPI_Shuld_Not_HaveDependencyOnOtherProjects()
        {
            var assembly = typeof(Askstatus.Web.API.AssemblyReference).Assembly;

            var otherProjects = new[]
            {
                WebUiNamespace,
                ArchitectureTestsNamespace,
                ApplicationTestsNamespace,
                InfrastructureTestsNamespace,
            };

            var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(otherProjects).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void WebUI_Shuld_Not_HaveDependencyOnOtherProjects()
        {
            var assembly = typeof(Askstatus.Web.App.AssemblyReference).Assembly;

            var otherProjects = new[]
            {
                WebAPINamespace,
                ArchitectureTestsNamespace,
                ApplicationTestsNamespace,
                InfrastructureTestsNamespace,
            };

            var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(otherProjects).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

    }
}
