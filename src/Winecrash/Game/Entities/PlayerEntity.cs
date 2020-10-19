using System;
using WEngine;

namespace Winecrash.Entities
{
    public class PlayerEntity : Entity
    {
        public static double HeadMaxAngleToBody { get; set; } = 45.0D;

        public bool AnyMoveInputOnFrame { get; set; } = false;
        
        public WObject ModelWObject { get; set; }
        public WObject PlayerHead { get; set; }
        public WObject PlayerTorso { get; set; }
        public WObject PlayerRightArm { get; set; }
        public WObject PlayerLeftArm { get; set; }
        public WObject PlayerRightLeg { get; set; }
        public WObject PlayerLeftLeg { get; set; }
        
        public Material SkinMaterial { get; set; }
        
        public static Mesh PlayerHeadMesh { get; set; }
        public static Mesh PlayerTorsoMesh { get; set; }
        public static Mesh PlayerRightArmMesh { get; set; }
        public static Mesh PlayerLeftArmMesh { get; set; }
        
        public static Mesh PlayerRightLegMesh { get; set; }
        public static Mesh PlayerLeftLegMesh { get; set; }
        
        
        
        public static Texture DefaultTexture { get; set; }

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
                Quaternion hRot = PlayerHead.LocalRotation = rotation;
                Quaternion tRot = PlayerTorso.LocalRotation;
                
                double deltaA = Quaternion.AngleY(hRot, tRot);

                if (deltaA > HeadMaxAngleToBody)
                {
                    PlayerTorso.LocalRotation *= new Quaternion(0,-(deltaA - HeadMaxAngleToBody),0);
                }
                else if (deltaA < -HeadMaxAngleToBody)
                {
                    PlayerTorso.LocalRotation *= new Quaternion(0,-(deltaA + HeadMaxAngleToBody),0);
                }
            };
        }

        protected override void Creation()
        {
            base.Creation();

            this.RigidBody.UseGravity = true;
        }

        protected override void Update()
        {
            base.Update();

            JumpCD -= Time.DeltaTime;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.RigidBody == null) return;

            double xzVel = this.RigidBody.Velocity.XZ.Length;

            Vector2D xzDir = this.RigidBody.Velocity.Normalized.XZ;
            

            if (xzVel > Player.WalkSpeed)
            {
                this.RigidBody.Velocity *= new Vector3D(0, 1, 0);
                
                this.RigidBody.Velocity += new Vector3D(xzDir.X, 0, xzDir.Y) * Player.WalkSpeed;
            }

            if (this.WObject.Position.Y < 64.0D)
            {
                this.RigidBody.Velocity *= new Vector3D(1,0,1);
                this.WObject.Position *= new Vector3D(1,0,1);
                this.WObject.Position += new Vector3D(0,64,0);
            }

            if (!AnyMoveInputOnFrame)
            {
                if (xzVel <= Player.StopSpeed)
                {
                    this.RigidBody.Velocity *= new Vector3D(0, 1, 0);
                }

                else
                {
                    Vector2D xySpeedDecay = xzDir * xzVel * 16.0D * Time.FixedDeltaTime;

                    Vector2D newSpeed = (xzDir * xzVel) - xySpeedDecay;
                    this.RigidBody.Velocity = new Vector3D(newSpeed.X, this.RigidBody.Velocity.Y, newSpeed.Y);
                }
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