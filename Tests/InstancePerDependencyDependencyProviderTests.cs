using System.Collections.Immutable;
using Core.Exceptions;
using Core.Services;
using Core.Services.Implementations;
using NUnit.Framework;
using Tests.TestClasses;

namespace Tests
{
    public class InstancePerDependencyDependencyProviderTests
    {
        private IDependenciesProvider DependenciesProvider { get; set; }

        [SetUp]
        public void SetUp()
        {
            var dependenciesConfiguration = new DependenciesConfiguration();
            dependenciesConfiguration.Register<ITest, Test>();
            dependenciesConfiguration.Register<ITest, TestNested>();
            dependenciesConfiguration.Register<IRepository, RepositoryImpl>();
            dependenciesConfiguration.Register<IService<IRepository>, ServiceImpl<IRepository>>();
            dependenciesConfiguration.Register<IServiceWithDependency, ServiceWithDependency>();
            dependenciesConfiguration.Register(typeof(IService<>), typeof(ServiceImpl<>));
            dependenciesConfiguration.Register(typeof(IService<>), typeof(ServiceImpl<IRepository>));
            dependenciesConfiguration.Register(typeof(IService<IRepository>), typeof(ServiceImpl<>));

            DependenciesProvider = new InstancePerDependencyDependencyProvider
                {DependenciesConfiguration = dependenciesConfiguration};
        }

        [Test]
        public void Resolve_ResolveRealizationWithNoInnerDependenciesSuccessfully()
        {
            var result = DependenciesProvider.Resolve<ITest>();

            Assert.NotNull(result);
        }

        [Test]
        public void Resolve_ResolveRealizationWithInnerDependenciesSuccessfully()
        {
            var result = DependenciesProvider.Resolve<IServiceWithDependency>();

            Assert.NotNull(result);
        }

        [Test]
        public void Resolve_ResolveRealizationWithGenericDependenciesSuccessfully()
        {
            var result = DependenciesProvider.Resolve<IService<IRepository>>();

            Assert.NotNull(result);
        }

        [Test]
        public void Resolve_CompletelyNewInstancesAreCreatedSuccess()
        {
            var firstResult = DependenciesProvider.Resolve<ITest>();
            var secondResult = DependenciesProvider.Resolve<ITest>();

            Assert.AreNotSame(firstResult, secondResult);
        }

        [Test]
        public void Resolve_ResolveNotRegisteredDependencyFailure()
        {
            Assert.Throws<DependencyProviderException>(() =>
                DependenciesProvider.Resolve<INotRegisteredInterface>());
        }
    }
}