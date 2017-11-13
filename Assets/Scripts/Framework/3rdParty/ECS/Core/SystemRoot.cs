using ECS.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ECS {
    public sealed class StandardSystemRoot : SystemRoot<EntityManager> { }

    [InjectableDependency(LifeTime.PerInstance)]
    public class SystemRoot<TEntityManager> where TEntityManager : EntityManager {
        private readonly List<ComponentSystem> _componentSystemList = new List<ComponentSystem>();
        
        [InjectDependency]
        private TEntityManager _entityManager;

        public SystemRoot() {
            InjectionManager.ResolveDependency(this);
        }

        public virtual void AddSystem<TComponentSystem>() where TComponentSystem : ComponentSystem {
            TComponentSystem componentSystem = InjectionManager.CreateObject<TComponentSystem>();
            _componentSystemList.Add(componentSystem);
            HandleTupleInjection(componentSystem);
        }

        public virtual void AddSystem(ComponentSystem system) {
            _componentSystemList.Add(system);
            InjectionManager.ResolveDependency(system);
            HandleTupleInjection(system);
        }

        protected void HandleTupleInjection(ComponentSystem system) {
            Type systemType = system.GetType();

            Type injectTupleAttributeType = typeof(InjectTupleAttribute);
            Type iComponentArrayType = typeof(ComponentArray);
            FieldInfo[]  allFields = systemType.GetFieldsRecursive(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).ToArray();
            FieldInfo[] injectionTypeFields = systemType.GetFieldsRecursive(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
                .Where(field => field.GetCustomAttributes(injectTupleAttributeType, false).Any())
                .Where(field => iComponentArrayType.IsAssignableFrom(field.FieldType))
                .ToArray();


            Type[] injectionComponentTypes = injectionTypeFields
                .Select(field => field.FieldType.GetGenericArguments()[0])
                .ToArray();

            ComponentGroup group = _entityManager.GetComponentGroup(injectionComponentTypes);

            for (int i = 0; i < injectionComponentTypes.Length; i++) {
                ComponentArray componentArray = group.GetComponent(injectionComponentTypes[i]);
                injectionTypeFields[i].SetValue(system, componentArray);
            }

            IComponentSystemSetup systemSetup = system;
            systemSetup.AddGroup(group);
        }

        public virtual void RemoveSystem(ComponentSystem system) {
            _componentSystemList.Remove(system);

            IComponentSystemSetup systemSetup = system;
            systemSetup.RemoveGroup();
        }

        public virtual void Start() {
            for (int i = 0; i < _componentSystemList.Count; i++) {
                _componentSystemList[i].OnStart();
            }
        }
        public virtual void Update() {
            for (int i = 0; i < _componentSystemList.Count; i++) {
                _componentSystemList[i].OnUpdate();
            }
        }

        public virtual void FixedUpdate() {
            for (int i = 0; i < _componentSystemList.Count; i++) {
                _componentSystemList[i].OnFixedUpdate();
            }
        }
    }
}