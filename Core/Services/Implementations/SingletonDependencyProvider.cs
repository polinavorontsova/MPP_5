using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core.Exceptions;

namespace Core.Services.Implementations
{
    public class SingletonDependencyProvider : IDependenciesProvider
    {
        private readonly IDictionary<Type, IEnumerable<object>> _instantiatedDependencies =
            new ConcurrentDictionary<Type, IEnumerable<object>>();

        public IDependenciesConfiguration DependenciesConfiguration { get; init; }

        public T Resolve<T>()
        {
            return (T) Resolve(typeof(T));
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            var resolve = (ImmutableList<object>) Resolve(typeof(IEnumerable<T>));
            return resolve.Select(item => (T) item).ToImmutableList();
        }

        private object Resolve(Type type)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(type))
                return type.IsGenericType
                    ? CreateGenericType(type)
                    : CreateNonGenericType(type);

            var targetType = type.GetGenericArguments().First();
            return targetType.IsGenericType
                ? CreateGenericTypes(targetType)
                : CreateNonGenericTypes(targetType);
        }

        private IEnumerable<object> CreateNonGenericTypes(Type type)
        {
            if (_instantiatedDependencies.ContainsKey(type)) return _instantiatedDependencies[type].ToImmutableList();

            var dependenciesMap = DependenciesConfiguration.GetDependenciesMap();
            if (!dependenciesMap.ContainsKey(type))
                throw new DependencyProviderException($"No realizations for type {type.Name} found.");

            var dependencies = dependenciesMap[type];
            var nonGenericTypes = dependencies.Select(Create).ToImmutableList();
            _instantiatedDependencies[type] = nonGenericTypes;
            return nonGenericTypes;
        }

        private object CreateNonGenericType(Type type)
        {
            if (_instantiatedDependencies.ContainsKey(type)) return _instantiatedDependencies[type].First();

            var dependenciesMap = DependenciesConfiguration.GetDependenciesMap();
            if (!dependenciesMap.ContainsKey(type))
                throw new DependencyProviderException($"No realizations for type {type.Name} found.");

            var dependencies = dependenciesMap[type];
            var nonGenericType = Create(dependencies.First());
            _instantiatedDependencies[type] = ImmutableList.Create(nonGenericType);
            return nonGenericType;
        }

        private object CreateGenericType(Type type)
        {
            if (_instantiatedDependencies.ContainsKey(type)) return _instantiatedDependencies[type].First();

            var dependenciesMap = DependenciesConfiguration.GetDependenciesMap();
            var dependencies = dependenciesMap.ContainsKey(type)
                ? dependenciesMap[type]
                : new List<Type>();

            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var dependenciesGeneralized =
                dependenciesMap.ContainsKey(genericTypeDefinition)
                    ? dependenciesMap[genericTypeDefinition]
                    : new List<Type>();

            var totalAvailableRealizations =
                dependencies.Concat(dependenciesGeneralized).ToImmutableList();

            if (totalAvailableRealizations.IsEmpty)
                throw new DependencyProviderException($"No realizations for type {type.Name} found.");

            var genericType = Create(totalAvailableRealizations.First());
            _instantiatedDependencies[type] = ImmutableList.Create(genericType);
            return genericType;
        }

        private object CreateGenericTypes(Type type)
        {
            if (_instantiatedDependencies.ContainsKey(type)) return _instantiatedDependencies[type].ToImmutableList();

            var dependenciesMap = DependenciesConfiguration.GetDependenciesMap();
            var dependencies = dependenciesMap.ContainsKey(type)
                ? dependenciesMap[type]
                : new List<Type>();

            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var dependenciesGeneralized =
                dependenciesMap.ContainsKey(genericTypeDefinition)
                    ? dependenciesMap[genericTypeDefinition]
                    : new List<Type>();

            var totalAvailableRealizations =
                dependencies.Concat(dependenciesGeneralized).ToImmutableList();

            if (totalAvailableRealizations.IsEmpty)
                throw new DependencyProviderException($"No realizations for type {type.Name} found.");

            var genericTypes = totalAvailableRealizations.Select(Create).ToImmutableList();
            _instantiatedDependencies[type] = genericTypes;
            return genericTypes;
        }

        private object Create(Type type)
        {
            var constructorInfos = type.GetConstructors()
                .ToImmutableList()
                .Sort((firstConstructor, secondConstructor) =>
                    secondConstructor.GetParameters().Length - firstConstructor.GetParameters().Length);

            if (constructorInfos.IsEmpty)
                throw new DependencyProviderException($"No constructors for type {type.Name} found.");

            var constructor = constructorInfos.First();
            var parametersType = constructor
                .GetParameters().Select(parameter => parameter.ParameterType).Select(parameterType =>
                    parameterType.IsTypeDefinition
                        ? parameterType
                        : parameterType.GetInterfaces().First()
                );
            var parameters = parametersType.Select(Resolve).ToArray();

            var areGenericTypesHaveNoTypeDefinition = type.IsGenericType &&
                                                      type.GetGenericArguments().Any(argument =>
                                                          !argument.IsTypeDefinition);

            return areGenericTypesHaveNoTypeDefinition
                ? Activator.CreateInstance(type.MakeGenericType(parametersType.ToArray()), parameters)
                : Activator.CreateInstance(type, parameters);
        }
    }
}