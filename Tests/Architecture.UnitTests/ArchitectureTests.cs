using System.Reflection;

namespace Architecture.UnitTests;

public class ArchitectureTests
{
    private const string DomainAssemblyName = "CleanArchitecture.Domain";
    private const string PresentationAssemblyName = "CleanArchitecture.Presentation";
    private const string ApplicationAssemblyName = "CleanArchitecture.Application";
    private const string InfrastructureAssemblyName = "CleanArchitecture.Infrastructure";
    private const string InfrastructurePersistenceAssemblyName = "CleanArchitecture.Infrastructure.Persistence";

    // External assembly prefixes that must never appear in the inner layers.
    // These represent persistence, web-framework, and database concerns that
    // belong exclusively in Infrastructure/Persistence or Presentation.
    private static readonly string[] DomainForbiddenExternalPrefixes =
    [
        "Microsoft.EntityFrameworkCore",
        "Npgsql",
        "Microsoft.AspNetCore",
        "FluentValidation",
    ];

    private static readonly string[] ApplicationForbiddenExternalPrefixes =
    [
        "Npgsql",
        "Microsoft.AspNetCore",
    ];

    [Fact]
    public void DomainShouldNotHaveAnyDependencies()
    {
        // Arrange
        Assembly domainAssembly = Assembly.Load(DomainAssemblyName);

        // Act
        AssemblyName[] references = domainAssembly.GetReferencedAssemblies();

        // Assert
        foreach (AssemblyName reference in references)
        {
            string? referencedAssemblyName = reference.Name;
            Assert.NotEqual(ApplicationAssemblyName, referencedAssemblyName);
            Assert.NotEqual(InfrastructureAssemblyName, referencedAssemblyName);
            Assert.NotEqual(PresentationAssemblyName, referencedAssemblyName);
        }
    }

    [Fact]
    public void DomainShouldNotReferenceExternalInfrastructurePackages()
    {
        Assembly domainAssembly = Assembly.Load(DomainAssemblyName);
        AssemblyName[] references = domainAssembly.GetReferencedAssemblies();

        foreach (AssemblyName reference in references)
        {
            string name = reference.Name ?? string.Empty;
            foreach (string prefix in DomainForbiddenExternalPrefixes)
            {
                Assert.False(
                    name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase),
                    $"Domain must not reference '{name}' (forbidden prefix '{prefix}'). Move it to Infrastructure or Persistence.");
            }
        }
    }

    [Fact]
    public void ApplicationShouldOnlyDependOnDomain()
    {
        // Arrange
        Assembly applicationAssembly = Assembly.Load(ApplicationAssemblyName);

        // Act
        AssemblyName[] references = applicationAssembly.GetReferencedAssemblies();

        // Assert
        foreach (AssemblyName reference in references)
        {
            string? referencedAssemblyName = reference.Name;
            Assert.NotEqual(InfrastructureAssemblyName, referencedAssemblyName);
            Assert.NotEqual(InfrastructurePersistenceAssemblyName, referencedAssemblyName);
            Assert.NotEqual(PresentationAssemblyName, referencedAssemblyName);
        }

        Assert.Contains(references, r => r.Name == DomainAssemblyName);
    }

    [Fact]
    public void ApplicationShouldNotReferenceExternalInfrastructurePackages()
    {
        Assembly applicationAssembly = Assembly.Load(ApplicationAssemblyName);
        AssemblyName[] references = applicationAssembly.GetReferencedAssemblies();

        foreach (AssemblyName reference in references)
        {
            string name = reference.Name ?? string.Empty;
            foreach (string prefix in ApplicationForbiddenExternalPrefixes)
            {
                Assert.False(
                    name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase),
                    $"Application must not reference '{name}' (forbidden prefix '{prefix}'). Move it to Infrastructure or Persistence.");
            }
        }
    }

    [Fact]
    public void InfrastructureShouldOnlyDependOnApplication()
    {
        // Arrange
        Assembly infrastructureAssembly = Assembly.Load(InfrastructureAssemblyName);

        // Act
        AssemblyName[] references = infrastructureAssembly.GetReferencedAssemblies();

        // Assert
        foreach (AssemblyName reference in references)
        {
            string? referencedAssemblyName = reference.Name;
            Assert.NotEqual(PresentationAssemblyName, referencedAssemblyName);
            Assert.NotEqual(InfrastructurePersistenceAssemblyName, referencedAssemblyName);
        }

        Assert.Contains(references, r => r.Name == ApplicationAssemblyName);
    }

    [Fact]
    public void InfrastructurePersistenceShouldOnlyDependOnApplication()
    {
        // Arrange
        Assembly infrastructurePersistenceAssembly = Assembly.Load(InfrastructurePersistenceAssemblyName);

        // Act
        AssemblyName[] references = infrastructurePersistenceAssembly.GetReferencedAssemblies();

        // Assert
        foreach (AssemblyName reference in references)
        {
            string? referencedAssemblyName = reference.Name;
            Assert.NotEqual(PresentationAssemblyName, referencedAssemblyName);
            Assert.NotEqual(InfrastructureAssemblyName, referencedAssemblyName);
        }

        Assert.Contains(references, r => r.Name == DomainAssemblyName);
        Assert.Contains(references, r => r.Name == ApplicationAssemblyName);
    }
}
