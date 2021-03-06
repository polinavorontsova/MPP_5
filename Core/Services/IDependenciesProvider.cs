using System.Collections.Generic;

namespace Core.Services
{
    public interface IDependenciesProvider
    {
        T Resolve<T>();

        IEnumerable<T> ResolveAll<T>();
    }
}