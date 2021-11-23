namespace Tests.TestClasses
{
    internal interface ITest
    {
    }

    internal class Test : ITest
    {
    }

    internal class TestNested : ITest
    {
    }

    internal interface IRepository
    {
    }

    internal class RepositoryImpl : IRepository
    {
        public RepositoryImpl(ITest test)
        {
        }
    }

    internal interface IService<TRepository> where TRepository : IRepository
    {
    }

    internal class ServiceImpl<TRepository> : IService<TRepository>
        where TRepository : IRepository
    {
        public ServiceImpl(TRepository repository)
        {
            Repository = repository;
        }

        public TRepository Repository { get; }
    }

    internal interface ITestSecond
    {
    }

    internal abstract class TestSecond : ITestSecond
    {
    }

    internal interface IServiceWithDependency
    {
    }

    internal class ServiceWithDependency : IServiceWithDependency
    {
        public ServiceWithDependency(IRepository repository)
        {
            Repository = repository;
        }

        public IRepository Repository { get; }
    }

    internal interface INotRegisteredInterface
    {
    }
}