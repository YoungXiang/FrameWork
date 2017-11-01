using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    // Data Types
    public abstract class IPlayableParams
    {
        #region basic params
        /// <summary>
        /// The name should be unique, in order to make the instance reusable.
        /// </summary>
        public string name;

        protected string m_nameForInstance;
        public string nameForInstance
        {
            get
            {
                if (m_nameForInstance == null) m_nameForInstance = GetNameForInstance(owner, name);
                return m_nameForInstance;
            }
        }

        /// <summary>
        /// Owner or target
        /// </summary>
        public GameObject owner;
        /// <summary>
        /// Play a new instance or replace the old instance.
        /// </summary>
        public bool newInstance;
        /// <summary>
        /// loopTime = -1 : loop forever, in this case you should stop it yourself; 
        /// loopTime = 0 : no loop; 
        /// loopTime = n (n > 0) : loop n times.
        /// </summary>
        public int loopTime;
        #endregion

        public IPlayableParams(GameObject owner_, bool newInstance_, int loopTime_)
        {
            owner = owner_;
            newInstance = newInstance_;
            loopTime = loopTime_;
        }

        public IPlayableParams(string name_, GameObject owner_, bool newInstance_, int loopTime_)
        {
            name = name_;
            owner = owner_;
            newInstance = newInstance_;
            loopTime = loopTime_;
        }

        public static string GetNameForInstance(GameObject owner_, string paramName_)
        {
            return string.Format("{0}{1}{2}", owner_.name, owner_.GetInstanceID(), paramName_);
        }
    }
}
