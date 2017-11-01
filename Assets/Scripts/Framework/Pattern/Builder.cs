using System.Collections.Generic;

namespace FrameWork
{
    public class BuilderManager<TKey, TBase> where TBase : new()
    {
        public interface IBuilder<TBase1>
        {
            TBase1 Build();
        }

        public class Builder<T> : IBuilder<TBase> where T : TBase, new()
        {
            public TBase Build() { return new T(); }
        }

        // Constructor.
        public BuilderManager()
        {
            RegisterAll();
        }

        // what you should do is to override this function and register your own builders.
        public virtual void RegisterAll()
        {
        }

        protected void RegisterBuilder<T>(TKey key) where T : TBase, new()
        {
            if (m_Builders.ContainsKey(key))
            {
                LogUtil.ERROR(string.Format("Builder has already been registered! {0}", key.ToString()));
                return;
            }

            m_Builders.Add(key, new Builder<T>());
        }

        public TBase Build(TKey key)
        {
            IBuilder<TBase> builder = GetBuilder(key);
            if (builder != null)
            {
                return builder.Build();
            }

            LogUtil.ERROR("Trying to build an object without registering builder first!");
            return default(TBase);
        }

        protected IBuilder<TBase> GetBuilder(TKey key)
        {
            if (m_Builders.ContainsKey(key))
                return m_Builders[key];

            return null;
        }

        protected Dictionary<TKey, IBuilder<TBase>> m_Builders = new Dictionary<TKey, IBuilder<TBase>>();
    }
}