using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECS.Injection {
    public class IocContainer {

        public readonly IList<RegisteredObject> registeredObjects = new List<RegisteredObject>();
        
        public void Register<TTypeToResolve, TConcrete>(LifeTime lifetime) {
            registeredObjects.Add(new RegisteredObject(typeof(TTypeToResolve), typeof(TConcrete), lifetime));
        }

        public void Register(Type typeToResolve, Type concreteType, LifeTime lifetime) {
            registeredObjects.Add(new RegisteredObject(typeToResolve, concreteType, lifetime));
        }

        public T Resolve<T>() {
            return (T)ResolveObject(typeof(T));
        }

        public object Resolve(Type type) {
            return ResolveObject(type);
        }

        private object ResolveObject(Type typeToResolve) {
            var registeredObject = registeredObjects.FirstOrDefault(o => o.TypeToResolve == typeToResolve);
            if (registeredObject == null) {
                throw new TypeNotRegisteredException(string.Format(
                    "The type {0} has not been registered", typeToResolve.Name));            
            }
            return GetInstance(registeredObject);
        }
        private object GetInstance(RegisteredObject registeredObject) {
            if (registeredObject.Instance == null ||
                registeredObject.Lifetime == LifeTime.PerInstance) {
                var parameters = ResolveConstructorParameters(registeredObject);
                registeredObject.CreateInstance(parameters.ToArray());
            }
            return registeredObject.Instance;
        }

        private IEnumerable<object> ResolveConstructorParameters(RegisteredObject registeredObject) {
            var constructorInfo = registeredObject.ConcreteType.GetConstructors().First();
            foreach (var parameter in constructorInfo.GetParameters()) {
                yield return ResolveObject(parameter.ParameterType);
            }
            yield break;
        }
    }
}
