using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class VirtualScene : BaseScene
    {
        public VirtualScene(string name_, string path_) : base(name_, path_)
        {
        }

        public override void Load()
        {
            throw new NotImplementedException();
        }
        
        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void Unload()
        {
            throw new NotImplementedException();
        }

        public override void OnLoaded()
        {
            throw new NotImplementedException();
        }

        public override void OnBeforeUnload()
        {
            throw new NotImplementedException();
        }
    }
}
