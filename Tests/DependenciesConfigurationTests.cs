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

        [Test]
        public void Register_RegisterDependencySuccessfully()
        {
            DependenciesConfiguration.Register<ITest, Test>();

            Assert.Contains(typeof(Test).FullName,
                DependenciesConfiguration.DependenciesMap.Values.First().Select(type => type.FullName)
                    .ToImmutableList());
        }

        [Test]
        public void Register_RegisterNestedInheritanceDependencySuccessfully()
        {
            DependenciesConfiguration.Register<ITest, TestNested>();

            Assert.Contains(typeof(TestNested).FullName,
                DependenciesConfiguration.DependenciesMap.Values.First().Select(type => type.FullName)
                    .ToImmutableList());
        }

        [Test]
        public void Register_RegisterParametrizedDependencySuccess()
        {
            DependenciesConfiguration.Register<IService<IRepository>, ServiceImpl<IRepository>>();

            Assert.Contains(typeof(ServiceImpl<IRepository>).FullName,
                DependenciesConfiguration.DependenciesMap.Values.First().Select(type => type.FullName)
                    .ToImmutableList());
        }

        [Test]
        public void Register_RegisterParametrizedOpenGenericsSuccess()
        {
            DependenciesConfiguration.Register(typeof(IService<>), typeof(ServiceImpl<>));

            Assert.Contains(typeof(ServiceImpl<>).FullName,
                DependenciesConfiguration.DependenciesMap.Values.First().Select(type => type.FullName)
                    .ToImmutableList());
        }

        [Test]
        public void Register_RegisterParametrizedOpenGenericsDependencyOnlySuccess()
        {
            DependenciesConfiguration.Register(typeof(IService<>), typeof(ServiceImpl<IRepository>));

            Assert.Contains(typeof(ServiceImpl<IRepository>).FullName,
                DependenciesConfiguration.DependenciesMap.Values.First().Select(type => type.FullName)
                    .ToImmutableList());
        }

        [Test]
        public void Register_RegisterParametrizedOpenGenericsRealizationOnlySuccess()
        {
            DependenciesConfiguration.Register(typeof(IService<IRepository>), typeof(ServiceImpl<>));

            Assert.Contains(typeof(ServiceImpl<>).FullName,
                DependenciesConfiguration.DependenciesMap.Values.First().Select(type => type.FullName)
                    .ToImmutableList());
        }

        [Test]
        public void Register_RegisterNotAssignableTypesFailure()
        {
            Assert.Throws<DependencyRegistrationException>(() => DependenciesConfiguration
                .Register(typeof(ITest), typeof(RepositoryImpl)));
        }

        [Test]
        public void Register_RegisterAbstractRealizationTypeFailure()
        {
            Assert.Throws<DependencyRegistrationException>(() => DependenciesConfiguration
                .Register<ITestSecond, TestSecond>());
        }

        [Test]
        public void Register_RegisterInterfaceRealizationTypeFailure()
        {
            Assert.Throws<DependencyRegistrationException>(() => DependenciesConfiguration
                .Register<ITestSecond, ITestSecond>());
        }
    }
}