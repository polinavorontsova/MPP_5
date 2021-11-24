using System.Collections.Immutable;
using System.Linq;
using Core.Entities;
using Core.Exceptions;
using Core.Services.Implementations;
using NUnit.Framework;
using Tests.TestClasses;

namespace Tests
{
    public class DependenciesConfigurationTests
    {
        private DependenciesConfiguration DependenciesConfiguration { get; set; }

        [SetUp]
        public void Setup()
        {
            DependenciesConfiguration = new DependenciesConfiguration
                {InstantiationType = DependenciesInstantiationType.Singleton};
        }

        [Test]
        public void Init_DependenciesConfigurationCreatedSuccessfully()
        {
            Assert.NotNull(DependenciesConfiguration);
            Assert.AreEqual(DependenciesConfiguration.InstantiationType, DependenciesInstantiationType.Singleton);
            Assert.IsEmpty(DependenciesConfiguration.DependenciesMap);
        }
    }
}