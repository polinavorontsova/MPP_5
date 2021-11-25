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
            if (!dependencyType.IsAssignableFrom(realizationType) &&
                !IsAssignableFromOpenGenerics(dependencyType, realizationType))
                throw new DependencyRegistrationException(
                    $"The dependency type {dependencyType.Name} is not assignable from the realization type {realizationType.Name}.");

            if (realizationType.IsAbstract)
                throw new DependencyRegistrationException(
                    $"$The realization type {realizationType.Name} must be overridden.");

            DependenciesMap.TryGetValue(dependencyType, out var implementations);
            if (implementations == null)
            {
                DependenciesMap.Add(dependencyType, ImmutableList.Create(realizationType));
                return;
            }

            DependenciesMap[dependencyType] = implementations.Concat(ImmutableList.Create(realizationType));
        }

        private static bool IsAssignableFromOpenGenerics(Type dependencyType, Type realizationType)
        {
            return dependencyType.IsGenericType && realizationType.IsGenericType &&
                   realizationType.GetInterfaces()
                       .Select(realizationInterfaceType => realizationInterfaceType.GetGenericTypeDefinition())
                       .Any(genericRealizationTypeDefinition =>
                           dependencyType.GetGenericTypeDefinition()
                               .IsAssignableFrom(genericRealizationTypeDefinition));
        }
    }
}