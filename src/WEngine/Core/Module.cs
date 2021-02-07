using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WEngine
{
    public abstract class Module : BaseObject
    {
        public bool RunAsync { get; set; } = false;

        private int _group;
        public int Group 
        { 
            get => _group;

            set
            {
                int current = Group;
                if (value == current) return;

                Group previousGroup = WEngine.Group.GetGroup(current);
            
                previousGroup.RemoveModule(this);

                WEngine.Group.CreateOrGetGroup(value, null, new[] { this });

                this._group = value;
            }
        }

        private int _ExecutionOrder = 0;
        public int ExecutionOrder
        {
            get
            {
                return _ExecutionOrder;
            }

            set
            {
                SetOrder(value);
            }
        }

        public int FixedExecutionOrder { get; set; } = 0;

        private void SetOrder(int newOrder)
        {
            this._ExecutionOrder = newOrder;

            WEngine.Group.GetGroup(this.Group).SortModules();
        }
        public Module() : base()
        {
            WEngine.Group.CreateOrGetGroup(0, "Default Group", new[] { this });
        }

        public WObject WObject { get; internal set; }

        internal bool StartDone { get; set; } = false;

        internal protected virtual void Creation() { }
        internal protected virtual void FirstFrame() { }
        
        internal protected virtual void EarlyPhysicsUpdate() { }
        internal protected virtual void PhysicsUpdate() { }
        internal protected virtual void LatePhysicsUpdate() { }

        internal protected virtual void EarlyUpdate() { }
        internal protected virtual void Update() { }
        internal protected virtual void LateUpdate() { }
        
        internal protected virtual void OnDelete() { }
        internal protected virtual void OnEnable() { }
        internal protected virtual void OnDisable() { }
        internal protected virtual void OnRender() { }

        public override bool Enabled
        {
            get
            {
                return _Enabled && this.WObject && this.WObject.Enabled;
            }
            set
            {
                SetEnable(value);
            }
        }

        internal sealed override void SetEnable(bool status)
        {
            if (this.Enabled)
            {
                if (!status)
                {
                    this.OnDisable();
                }
            }
            else
            {
                if (status)
                {
                    this.OnEnable();
                }
            }

            base.SetEnable(status);
        }

        internal bool wasEnabled = false;
        internal void RecordEnabled() => wasEnabled = this.Enabled;

        internal void TriggerEnabledEvents()
        {
            if(this.Enabled && !wasEnabled) this.OnEnable();
            else if(!this.Enabled && wasEnabled) this.OnDisable();
        }

        internal sealed override void ForcedDelete()
        {
            InternalDelete();
        }
        public sealed override void Delete()
        {
            if (this.Undeletable)
            {
                Debug.LogWarning("Unable to delete " + this + " : undeletable.");
                return;
            }

            InternalDelete();
        }

        private void InternalDelete()
        {
            try
            {
                this.OnDelete();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            this.WObject?._Modules.Remove(this);
            this.WObject = null;
            
            WEngine.Group.GetGroup(this.Group)?.RemoveModule(this);
            base.Delete();
        }
    }
}
