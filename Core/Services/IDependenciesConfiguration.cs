using System;
using System.Collections.Generic;

namespace Core.Services
{
    public interface IDependenciesConfiguration
    {
        IDictionary<Type, IEnumerable<Type>> GetDependenciesMap();
        void Register<T, TR>() where TR : T;

        void Register(Type dependencyType, Type realizationType);
    }
}