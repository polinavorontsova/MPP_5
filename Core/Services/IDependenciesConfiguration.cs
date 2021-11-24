using System;
using System.Collections.Generic;

namespace Core.Services
{
    public interface IDependenciesConfiguration
    {
        void Register<T, TR>() where TR : T;

        void Register(Type dependencyType, Type realizationType);
    }
}