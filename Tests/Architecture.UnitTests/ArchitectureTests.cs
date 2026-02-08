using System.Reflection;

namespace Architecture.UnitTests;

public class UnitTest1
{
    private const string DomainAssemblyName = "CleanArchitecture.Domain";
    private const string PresentationAssemblyName = "CleanArchitecture.Presentation";
    private const string ApplicationAssemblyName = "CleanArchitecture.Application";
    private const string InfrastructureAssemblyName = "CleanArchitecture.Infrastructure";
    private const string InfrastructurePersistenceAssemblyName = "CleanArchitecture.Infrastructure.Persistence";

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
            Assert.NotEqual(PresentationAssemblyName, referencedAssemblyName);
        }
        
        Assert.Contains(references, r => r.Name == DomainAssemblyName);
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
        }
        
        Assert.Contains(references, r => r.Name == DomainAssemblyName);
        Assert.Contains(references, r => r.Name == ApplicationAssemblyName);
    }
}
