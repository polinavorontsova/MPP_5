using System.Collections.Immutable;
using System.Linq;
using Core.Exceptions;
using Core.Services;
using Core.Services.Implementations;
using NUnit.Framework;
using Tests.TestClasses;

namespace Tests
{
    public class SingletonDependencyProviderTests
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

            DependenciesProvider = new SingletonDependencyProvider
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
        public void Resolve_ResolveMultipleRealizationsWithNoInnerDependenciesSuccessfully()
        {
            var result = DependenciesProvider.ResolveAll<ITest>().ToImmutableList();

            Assert.AreEqual(2, result.Count);
            Assert.NotNull(result[0]);
            Assert.NotNull(result[1]);
        }

        [Test]
        public void Resolve_ResolveRealizationWithGenericDependenciesSuccessfully()
        {
            var result = DependenciesProvider.Resolve<IService<IRepository>>();

            Assert.NotNull(result);
        }

        [Test]
        public void Resolve_ResolveMultipleRealizationWithGenericDependenciesSuccessfully()
        {
            var result = DependenciesProvider.ResolveAll<IService<IRepository>>().ToImmutableList();

            Assert.AreEqual(4, result.Count);
            Assert.NotNull(result[0]);
            Assert.NotNull(result[1]);
            Assert.NotNull(result[2]);
            Assert.NotNull(result[3]);
        }

        [Test]
        public void Resolve_CompletelyNewInstancesAreCreatedSuccess()
        {
            var firstResult = DependenciesProvider.Resolve<ITest>();
            var secondResult = DependenciesProvider.Resolve<ITest>();

            Assert.AreSame(firstResult, secondResult);
        }

        [Test]
        public void Resolve_CompletelyNewEnumerableInstancesAreCreatedSuccess()
        {
            var firstResult = DependenciesProvider.ResolveAll<IService<IRepository>>().ToImmutableList();
            var secondResult = DependenciesProvider.ResolveAll<IService<IRepository>>().ToImmutableList();

            Assert.AreEqual(firstResult.Count, secondResult.Count);
            foreach (var dependency in firstResult)
                Assert.True(secondResult.Any(secondItem => secondItem.Equals(dependency)));
        }

        [Test]
        public void Resolve_ResolveNotRegisteredDependencyFailure()
        {
            Assert.Throws<DependencyProviderException>(() =>
                DependenciesProvider.Resolve<INotRegisteredInterface>());
        }
    }
}