using System;
using System.Collections.Generic;

namespace ECS {
    public class InvalidTComponentException : Exception {
        private const string TComponentIsIComponentTypeException = "TComponent cant be IComponent directly! Please cast to the specific Type";
        public InvalidTComponentException() : base(TComponentIsIComponentTypeException) { }
    }

    [InjectableDependency(LifeTime.Singleton)]
    public class EntityManager {

        private readonly static Type iComponentType = typeof(IComponent);

        private int nextEntityId = 0;
        private readonly Stack<int> freeEntityIds = new Stack<int>();
        
        private readonly List<Entity> _entities = new List<Entity>();
        private readonly Dictionary<Entity, HashSet<Type>> _entityComponents = new Dictionary<Entity, HashSet<Type>>();
        private readonly Dictionary<Type, ComponentArray> _components = new Dictionary<Type, ComponentArray>(TypeComparer.typeComparer);
        private readonly Dictionary<GroupMatcher, ComponentGroup> componentGroups = new Dictionary<GroupMatcher, ComponentGroup>();

        public event Action<Entity> EntityCreated;
        public event Action<Entity> EntityDestroyed;

        //[Obsolete("Garbage Collection intensive operation! Boxing of structure type components! Only for debugging purpose!!! Dont use this in operative System!")]
        public event Action<Entity, Type> CompoentAdded;

        //[Obsolete("Garbage Collection intensive operation! Boxing of structure type components! Only for debugging purpose!!! Dont use this in operative System!")]
        public event Action<Entity, Type> CompoentRemoved;

        //[Obsolete("Garbage Collection intensive operation! Boxing of structure type components! Only for debugging purpose!!! Dont use this in operative System!")]
        public event Action<Entity, Type> CompoentChanged;

        public Entity CreateEntity() {
            Entity entity;
            if (freeEntityIds.Count != 0) {
                entity = new Entity(freeEntityIds.Pop());
            } else {
                entity = new Entity(nextEntityId);
                nextEntityId++;
            }
            _entityComponents.Add(entity, new HashSet<Type>());
            _entities.Add(entity);

            if (EntityCreated != null) {
                EntityCreated.Invoke(entity);
            }

            return entity;
        }

        public void DestroyEntity(Entity entity) {
            foreach (Type componentType in _entityComponents[entity]) {
                _components[componentType].Remove(entity);
            }
            _entities.Remove(entity);
            _entityComponents.Remove(entity);
            InspectComponentGroups(entity);
            freeEntityIds.Push(entity.Id);

            if (EntityDestroyed != null) {
                EntityDestroyed.Invoke(entity);
            }
        }

        public void AddComponent<TComponent>(Entity entity, TComponent component) where TComponent : IComponent {
            if(typeof(TComponent) == iComponentType) {
                throw new InvalidTComponentException();
            }

            Type componentType = typeof(TComponent);
            ComponentArray<TComponent> entityComponentMap = GetComponentMap<TComponent>(true);
            entityComponentMap.Add(entity, component);
            var type = component.GetType();
            _entityComponents[entity].Add(componentType);
            InspectComponentGroups(entity);

            if (CompoentAdded != null) {
                CompoentAdded.Invoke(entity, componentType);
            }
        }

        public void SetComponent<TComponent>(Entity entity, TComponent component) where TComponent : struct, IComponent {
            if (typeof(TComponent) == iComponentType) {
                throw new InvalidTComponentException();
            }
            ComponentArray<TComponent> entityComponentMap = GetComponentMap<TComponent>(false);
            entityComponentMap.Update(entity, component); 
            UpdateComponentGroups(entity, component);

            if (CompoentChanged != null) {
                CompoentChanged.Invoke(entity, typeof(TComponent));
            }
        }


        public void RemoveComponent<TComponent>(Entity entity, TComponent component) where TComponent : IComponent {
            if (typeof(TComponent) == iComponentType) {
                throw new InvalidTComponentException();
            }
            Type componentType = typeof(TComponent);
            ComponentArray<TComponent> entityComponentMap = GetComponentMap<TComponent>(false);
            if (entityComponentMap != null) {
                entityComponentMap.Remove(entity);
                _entityComponents[entity].Remove(componentType);
            }
            InspectComponentGroups(entity);

            if (CompoentRemoved != null) {
                CompoentRemoved.Invoke(entity, componentType);
            }
        }

        public bool HasComponent(Entity entity, Type componentType) {
            HashSet<Type> componentTypes;
            if (!_entityComponents.TryGetValue(entity, out componentTypes)) {
                return false;
            }
            return componentTypes.Contains(componentType);
        }

        public IComponent GetComponent(Entity entity, Type componentType) {
            ComponentArray componentEntityMap;
            if (_components.TryGetValue(componentType, out componentEntityMap)) {
                IComponent component;
                componentEntityMap.TryGetValue(entity, out component);
                return component;
            }
            return null;
        }

        public TComponent GetComponent<TComponent>(Entity entity) where TComponent : IComponent {
            if (typeof(TComponent) == iComponentType) {
                throw new InvalidTComponentException();
            }
            Type componentType = typeof(TComponent);

            ComponentArray componentEntityMap;
            if (_components.TryGetValue(componentType, out componentEntityMap)) {
                return ((ComponentArray<TComponent>)componentEntityMap).GetComponent(entity);
            }
            throw new Exception("Entity doesn't have component " + componentType);
        }

        public IEnumerable<IComponent> GetComponents(Entity entity) {
            IEnumerable<Type> componentTypes = _entityComponents[entity];
            List<IComponent> componentList = new List<IComponent>();

            foreach (Type componentType in componentTypes) {
                IComponent component = GetComponent(entity, componentType);
                if (component != null) {
                    componentList.Add(component);
                }
            }

            return componentList;
        }

        public ComponentGroup GetComponentGroup(params Type[] componentTypes) {
            if (componentTypes.Length == 0) {
                return null;
            }

            GroupMatcher groupMatcher = new GroupMatcher(componentTypes);
            ComponentGroup componentGroup;
            if (!componentGroups.TryGetValue(groupMatcher, out componentGroup)) {
                componentGroup = new ComponentGroup(this, componentTypes);
                for (int i = 0; i < _entities.Count; i++) {
                    componentGroup.Inspect(_entities[i]);
                }
                componentGroups.Add(groupMatcher, componentGroup);
            }
            return componentGroup;
        }


        private ComponentArray<TComponent> GetComponentMap<TComponent>(bool createIfNotFound) where TComponent : IComponent {
            ComponentArray entityComponentMap;
            Type componentType = typeof(TComponent);
            if (!_components.TryGetValue(componentType, out entityComponentMap) && createIfNotFound) {
                entityComponentMap = new ComponentArray<TComponent>();
                _components.Add(componentType, entityComponentMap);
            }
            return (ComponentArray<TComponent>)entityComponentMap;
        }

        private void InspectComponentGroups(Entity entity) {
            foreach (KeyValuePair<GroupMatcher, ComponentGroup> group in componentGroups) {
                group.Value.Inspect(entity);
            }
        }

        private void UpdateComponentGroups<TComponent>(Entity entity, TComponent component) where TComponent : struct, IComponent {
            foreach (KeyValuePair<GroupMatcher, ComponentGroup> group in componentGroups) {
                group.Value.Update(entity, component);
            }
        }

    }

    class TypeComparer : IEqualityComparer<Type> {
        public static readonly TypeComparer typeComparer = new TypeComparer();
        public bool Equals(Type x, Type y) {
            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(Type obj) {
            return obj.GetHashCode();
        }
    }
}