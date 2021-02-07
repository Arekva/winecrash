using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using WEngine;

namespace Winecrash.Entities
{
    public class PlayerEntity : Entity
    {
        #region Animation
        public static double HeadMaxAngleToBody { get; set; } = 25.0D;
        public static double WalkAnimationMaxAngle { get; set; } = 55D;
        public static double WalkAnimationSpeedCoef { get; set; } = 2.45D;
        public static double WalkAnimationTorsoOrientCoef { get; set; } = 12D;

        public static double WalkAnimationLegCoef { get; set; } = 1.4D;
        
        public static double WalkAnimationTimeStandBy { get; set; } = 0.5D;
        public static double CurrentWalkAnimationTime { get; set; } = WalkAnimationTimeStandBy;

        public static double IdleAnimationArmMinAngle { get; set; } = 2.0D;
        public static double IdleAnimationArmMaxAngle { get; set; } = 5.0D;
        public static double IdleAnimationSpeed { get; set; } = 1.1D;
        public static double CurrentIdleAnimationTime { get; set; } = 0.0D;
        
        #endregion
        
        #region WObjects
        public WObject ModelWObject { get; set; }
        public WObject PlayerHead { get; set; }
        public WObject PlayerTorso { get; set; }
        public WObject PlayerRightArm { get; set; }
        public WObject PlayerLeftArm { get; set; }
        public WObject PlayerRightLeg { get; set; }
        public WObject PlayerLeftLeg { get; set; }
        
        public static WObject PlayerHandWobject { get; set; }
        #endregion

        #region Meshes
        public static Mesh PlayerHeadMesh { get; set; }
        public static Mesh PlayerTorsoMesh { get; set; }
        public static Mesh PlayerRightArmMesh { get; set; }
        public static Mesh PlayerLeftArmMesh { get; set; }
        public static Mesh PlayerRightLegMesh { get; set; }
        public static Mesh PlayerLeftLegMesh { get; set; }
        #endregion
        
        public static Texture DefaultTexture { get; set; }
        public Material SkinMaterial { get; set; }

        public bool AnyMoveInputOnFrame { get; set; } = false;
        
        public double JumpCD = 0.0D;

        static PlayerEntity()
        {
            if (Engine.DoGUI)
            {
                PlayerHeadMesh = Mesh.LoadFile("assets/Models/Player_Head.obj", MeshFormats.Wavefront);
                PlayerTorsoMesh = Mesh.LoadFile("assets/Models/Player_Torso.obj", MeshFormats.Wavefront);
                PlayerRightArmMesh = Mesh.LoadFile("assets/Models/Player_RightArm.obj", MeshFormats.Wavefront);
                PlayerLeftArmMesh = Mesh.LoadFile("assets/Models/Player_LeftArm.obj", MeshFormats.Wavefront);
                
                PlayerRightLegMesh = Mesh.LoadFile("assets/Models/Player_RightLeg.obj", MeshFormats.Wavefront);
                PlayerLeftLegMesh = Mesh.LoadFile("assets/Models/Player_LeftLeg.obj", MeshFormats.Wavefront);
                
                DefaultTexture = new Texture("assets/textures/steve.png", "Steve", true);
            }
        }

        public void CreateModel()
        {
            ModelWObject = new WObject("Player Model"){ Parent = this.WObject };
            ModelWObject.LocalPosition = Vector3D.Zero;

            PlayerHead = new WObject("Head") { Parent = this.ModelWObject };
            PlayerHead.LocalPosition = Vector3D.Up * 1.3875D;//1.62D;
            
            MeshRenderer mr = PlayerHead.AddModule<MeshRenderer>();
            mr.Mesh = PlayerHeadMesh;
            mr.Material = SkinMaterial = new Material(Shader.Find("Player"));
            mr.Material.SetData("albedo", DefaultTexture);
            mr.Material.SetData("color", Color256.White);
            mr.Material.SetData("tiling", Vector2D.One);
            
            PlayerTorso = new WObject("Torso") { Parent = this.ModelWObject };
            PlayerTorso.LocalPosition = Vector3D.Up * 0.69375D;
            mr = PlayerTorso.AddModule<MeshRenderer>();
            mr.Mesh = PlayerTorsoMesh;
            mr.Material = SkinMaterial;
            
            PlayerRightArm = new WObject("Right Arm") {Parent = this.PlayerTorso };
            PlayerRightArm.LocalPosition = Vector3D.Up * 0.578125D + Vector3D.Left * (0.4625D - (0.4625D / 4.0D));
            mr = PlayerRightArm.AddModule<MeshRenderer>();
            mr.Mesh = PlayerRightArmMesh;
            mr.Material = SkinMaterial;
            
            PlayerLeftArm = new WObject("Left Arm") { Parent = this.PlayerTorso };
            PlayerLeftArm.LocalPosition = Vector3D.Up * 0.578125D - Vector3D.Left * (0.4625D - (0.4625D / 4.0D));
            mr = PlayerLeftArm.AddModule<MeshRenderer>();
            mr.Mesh = PlayerLeftArmMesh;
            mr.Material = SkinMaterial;
            
            PlayerRightLeg = new WObject("Right Leg") { Parent = this.PlayerTorso };
            PlayerRightLeg.LocalPosition =  Vector3D.Left * (0.4625D / 4.0D);
            mr = PlayerRightLeg.AddModule<MeshRenderer>();
            mr.Mesh = PlayerRightLegMesh;
            mr.Material = SkinMaterial;
            
            PlayerLeftLeg = new WObject("Left Leg") { Parent = this.PlayerTorso };
            PlayerLeftLeg.LocalPosition =  -Vector3D.Left * (0.4625D / 4.0D);
            mr = PlayerLeftLeg.AddModule<MeshRenderer>();
            mr.Mesh = PlayerLeftLegMesh;
            mr.Material = SkinMaterial;

            
            
            OnRotate += rotation =>
            {
                if (PlayerHead) PlayerHead.LocalRotation = rotation;
            };
        }
        
        public void AnimateIdle()
        {
            if (PlayerLeftArm == null || PlayerRightArm == null) return;
            
            PlayerLeftArm.LocalRotation = PlayerRightArm.LocalRotation = Quaternion.Identity;
            
            CurrentIdleAnimationTime += Time.Delta * IdleAnimationSpeed;

            double t = WMath.Remap(Math.Cos(CurrentIdleAnimationTime), -1, 1, 0, 1);

            double currentZAngle = WMath.Lerp(IdleAnimationArmMinAngle, IdleAnimationArmMaxAngle, t);

            double currentXAngle = currentZAngle * 0.25D;
            
            PlayerLeftArm.LocalRotation = new Quaternion(currentXAngle,0,currentZAngle);
            PlayerRightArm.LocalRotation = new Quaternion(-currentXAngle,0,-currentZAngle);
        }

        public void AnimateHead()
        {
            if (PlayerHead == null) return;
            Quaternion hRot = PlayerHead.LocalRotation;
            Quaternion tRot = PlayerTorso.LocalRotation;

            double deltaA = Quaternion.AngleY(hRot, tRot);

            if (deltaA > HeadMaxAngleToBody)
            {
                PlayerTorso.LocalRotation *= new Quaternion(0, -(deltaA - HeadMaxAngleToBody), 0);
            }
            else if (deltaA < -HeadMaxAngleToBody)
            {
                PlayerTorso.LocalRotation *= new Quaternion(0, -(deltaA + HeadMaxAngleToBody), 0);
            }
        }

        public void AnimateWalk(Vector3D velocity)
        {
            if (PlayerLeftLeg == null || PlayerRightLeg == null) return;
            double speed = WMath.Clamp(velocity.XZ.Length,0,Player.WalkSpeed);

            CurrentWalkAnimationTime += Time.Delta * speed * WalkAnimationSpeedCoef;

            double t = WMath.Remap(Math.Cos(CurrentWalkAnimationTime), -1, 1, 0, 1);

            double speedCoef = speed <= 0.05D ? 0.0D : speed / Player.WalkSpeed;

            double currentAngle = WMath.Lerp(-WalkAnimationMaxAngle * speedCoef, WalkAnimationMaxAngle * speedCoef, t);//WMath.Remap(CurrentWalkAnimationTime, 0, 1, -WalkAnimationMaxAngle * speedCoef, WalkAnimationMaxAngle * speedCoef);


            Quaternion leftRotLeg = new Quaternion(-currentAngle * WalkAnimationLegCoef,0,0);
            Quaternion rightRotLeg = new Quaternion(currentAngle * WalkAnimationLegCoef,0,0);
            
            Quaternion rightRotArm = new Quaternion(-currentAngle,0,0);
            Quaternion leftRotArm = new Quaternion(currentAngle,0,0);

            
            PlayerLeftLeg.LocalRotation = leftRotLeg;
            PlayerRightLeg.LocalRotation = rightRotLeg;

            PlayerRightArm.LocalRotation *= rightRotArm;
            PlayerLeftArm.LocalRotation *= leftRotArm;


            Vector3D flattenVel = new Vector3D(velocity.X, 0,velocity.Z);
            Vector3D flattenDir = flattenVel.Normalized;
            double flattenSpeed = flattenVel.Length;
            
            if (flattenSpeed >= 0.1D)
            {
                double currentTorsoAngle = Quaternion.AngleY(PlayerTorso.Rotation, Quaternion.Identity);
                double rawNewAngle = (Math.Atan2(flattenDir.X, flattenDir.Z) * WMath.RadToDeg) * -1;
                
                double deltaAngle = WMath.DeltaAngle(currentTorsoAngle, rawNewAngle);

                deltaAngle = WMath.Clamp(deltaAngle, -HeadMaxAngleToBody, HeadMaxAngleToBody);
                
                PlayerTorso.LocalRotation *= new Quaternion(0,-deltaAngle * WalkAnimationTorsoOrientCoef * Time.Delta,0);
            }
        }

        private void PickItems()
        {
            if (!this.Chunk) this.Chunk = World.GetChunk(this.ChunkCoordinates, this.Dimension);
            if (!this.Chunk) return;
            
            Player player = null;
            lock (Player.PlayersLocker)
                player = Player.Players.FirstOrDefault(p => p.Entity == this);
            
            double pickRangeSqr = Player.PickRange * Player.PickRange;
            Vector3D pos = this.WObject.Position + Player.PickLocalPoint;


            
            List<Chunk> searchChunks = this.Chunk.GetNeighbors();
            searchChunks.Add(this.Chunk);
            
            Entity[] entities = null;

            // search only in this and neighbors chunks
            foreach (Chunk chunk in searchChunks)
            {
                lock (chunk.EntityLocker)
                    entities = chunk.Entities.ToArray();

                for (int i = 0; i < entities.Length; i++)
                {
                    // sort by item entities
                    if (entities[i] is ItemEntity entity && 
                        entity.IsPickable &&
                        Vector3D.SquaredDistance(pos, entity.WObject.Position) < pickRangeSqr)
                    {
                        entity.PickingPlayer = player;
                        entity.IsPicked = true;
                    }
                }
            }
        }
        
        protected override void Creation()
        {
            base.Creation();

            this.RigidBody.UseGravity = true;
            //this.RigidBody.Velocity += new Vector3D(0, -0.5, 0);
            this.Collider.Extents = new Vector3D(0.6,1.8,0.6)/2.0D;
            this.Collider.Offset = new Vector3D(0,1.8,0)/2.0D;

            this.FixedExecutionOrder = -1;
            
            this.OnChunkChange += (previous, current) =>
            {
                Task.Run(() =>
                {
                    Chunk[] chunks = World.GetChunks("winecrash:overworld");
                    Vector2I[] previousChunks = new Vector2I[chunks.Length];//World.GetCoordsInRange(previous, Winecrash.RenderDistance);
                    for (int i = 0; i < chunks.Length; i++)
                    {
                        previousChunks[i] = chunks[i].Coordinates;
                    }
                    Vector2I[] currentChunks = World.GetCoordsInRange(current, Winecrash.RenderDistance).OrderBy(c => Vector2I.Distance(c, current)).ToArray();

                    /*Vector2I[] toDelete = previousChunks.Except(currentChunks).ToArray();

                    for (int i = 0; i < toDelete.Length; i++)
                    {
                        World.UnloadChunk(World.GetChunk(toDelete[i], "winecrash:overworld"));
                        //World.GetChunk(toDelete[i], "winecrash:overworld")?.Delete();
                    }
                    
                    Debug.Log("AAA " + ++aa);
*/
                    for (int i = 0; i < currentChunks.Length; i++)
                    {
                        World.GetOrCreateChunk(currentChunks[i], "winecrash:overworld");
                    }

                    currentChunks = null;
                    previousChunks = null;

                    //Parallel.ForEach(currentChunks, vec => { World.GetOrCreateChunk(vec, "winecrash:overworld"); });
                });
            };

            if (Engine.DoGUI)
            {
                CreateModel();
            }
        }

        protected override void FirstFrame()
        {
            base.FirstFrame();

            if (ModelWObject)
            {
                ModelWObject.Enabled = !(Player.LocalPlayer != null && Player.LocalPlayer.Entity == this);

                if (!ModelWObject.Enabled && PlayerHandWobject == null) // local player
                {
                    PlayerHandWobject = new WObject("FPS Hand");
                    PlayerHandWobject.Parent = Camera.Main.WObject;
                    PlayerHandWobject.LocalPosition =
                        Vector3D.Left * 0.4 + Vector3D.Forward * 0.4 + Vector3D.Down * 0.75;
                    PlayerHandWobject.LocalRotation = new Quaternion(180 + 45, -20, 0);
                    PlayerHandWobject.LocalRotation *= new Quaternion(0, -25, 0);
                    MeshRenderer mr = PlayerHandWobject.AddModule<MeshRenderer>();
                    mr.UseDepth = false;
                    mr.DrawOrder = 1000;

                    mr.Mesh = PlayerRightArmMesh;
                    mr.Material = SkinMaterial;
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Engine.DoGUI)
            {
                if (this.ModelWObject != null && this.ModelWObject.Enabled)
                {
                    AnimateHead();
                    AnimateIdle();
                    AnimateWalk(this.RigidBody.Velocity);
                }
            }

            PickItems();
        }
        
        protected override void LatePhysicsUpdate()
        {
            //Debug.Log(Time.PhysicsDelta);
            if (this.RigidBody == null || this.Collider == null) return;

            double xzVel = this.RigidBody.Velocity.XZ.Length;
            

            Vector2D xzDir = this.RigidBody.Velocity.Normalized.XZ;
            
            

            uint currentHeight = 1+World.GetSurface(this.WObject.Position, "winecrash:overworld");
            
            if (!Player.NoClipping)
            {
                if (xzVel > Player.WalkSpeed)
                {
                    this.RigidBody.Velocity *= new Vector3D(0, 1, 0);

                    this.RigidBody.Velocity += new Vector3D(xzDir.X, 0, xzDir.Y) * Player.WalkSpeed;
                }
            }


            if (AnyMoveInputOnFrame)
            {
                
            }
            else
            {
                if (xzVel <= Player.StopSpeed)
                {
                    this.RigidBody.Velocity *= new Vector3D(0, 1, 0);
                }

                else
                {
                    Vector2D xySpeedDecay = xzDir * xzVel * Player.WalkDeaccelerationFactor * Time.PhysicsDelta;

                    Vector2D newSpeed = (xzDir * xzVel) - xySpeedDecay;
                    
                    this.RigidBody.Velocity = new Vector3D(newSpeed.X, this.RigidBody.Velocity.Y, newSpeed.Y);
                }
            }

            if (!Player.NoClipping)
            {
                RaycastChunkHit h = ChunkBoxCollisionProvider.CollideWorld(Collider);
            }
        }

        protected override void OnDelete()
        {
            ModelWObject?.Delete();
            ModelWObject = null;
            PlayerHead = null;
            PlayerTorso = null;
            SkinMaterial = null;
            PlayerRightArm = null;
            PlayerLeftArm = null;
            PlayerRightLeg = null;
            PlayerLeftLeg = null;
            
            base.OnDelete();
        }
    }
}