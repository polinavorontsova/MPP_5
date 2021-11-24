using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core.Entities;
using Core.Exceptions;

namespace Core.Services.Implementations
{
    public class DependenciesConfiguration : IDependenciesConfiguration
    {
        public readonly IDictionary<Type, IEnumerable<Type>> DependenciesMap =
            new ConcurrentDictionary<Type, IEnumerable<Type>>();

        public DependenciesInstantiationType InstantiationType { get; init; }

        public void Register<T, TR>() where TR : T
        {
            Register(typeof(T), typeof(TR));
        }

        public void Register(Type dependencyType, Type realizationType)
        {
            
        }
    }
}