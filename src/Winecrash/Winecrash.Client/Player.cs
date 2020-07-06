using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Client
{
    public class Player : Module
    {
        public Camera FPSCamera;

        public float WalkMaxSpeed = 2.8F;
        public float RunMaxSpeed = 5.0F;
        public float FallMaxSpeed = -10.0F;

        public float WalkForce = 10.0F;
        public float JumpForce = 5.0F;

        public float JumpCooldown = 0.25F;
        private float TimeSinceLastJump = 0.0F;

        public float WalkSlowFactor = 0.9F;

        public float CameraRotationSpeed = 5.0F;

        public bool Grounded { get; private set; } = false;

        private Vector2D Angles = new Vector2D();

        private RigidBody _Rb;
        private BoxCollider _Bc;

        public Vector3F CameraShift = new Vector3F(0, 0.7F, 0);

        public RaycastChunkHit? ViewRayHit = null;

        public double HitRange = 30.0D;

        public Chunk CurrentChunk { get; private set; }

        public WObject Cursor3D;
        public WObject HitSphere;

        protected override void Creation()
        {
            this.FPSCamera = Camera.Main;
            this._Rb = this.WObject.GetModule<RigidBody>();
            this._Bc = this.WObject.GetModule<BoxCollider>();


            Cursor3D = new WObject("Block Cursor");
            MeshRenderer mr = Cursor3D.AddModule<MeshRenderer>();
            mr.Material = new Material(Shader.Find("Unlit"));
            mr.Material.SetData<Vector4>("color", new Color256());
            mr.Mesh = Mesh.LoadFile("assets/models/Cube.obj", MeshFormats.Wavefront);
            mr.Wireframe = true;
            Cursor3D.Scale *= 0.505F;

            HitSphere = new WObject("Debug hit sphere");
            MeshRenderer hmr = HitSphere.AddModule<MeshRenderer>();
            hmr.Material = new Material(Shader.Find("Unlit"));
            hmr.Material.SetData<Vector4>("color", new Color256(0, 1, 0, 1));
            hmr.Mesh = Mesh.LoadFile("assets/models/Skysphere.obj", MeshFormats.Wavefront);
            hmr.Wireframe = true;
            HitSphere.Scale *= 0.05F;

            Cursor3D.Enabled = HitSphere.Enabled = false;
        }

        private void CameraRotation()
        {
            Vector2D deltas = Input.MouseDelta;

            double ax = (Angles.X + (deltas.X * Input.MouseSensivity * (float)Time.DeltaTime)) % 360.0F;
            double ay = WMath.Clamp((Angles.Y + (deltas.Y * Input.MouseSensivity * (float)Time.DeltaTime)), -90.0F, 90.0F);

            Angles = new Vector2D(ax, ay);

            this.FPSCamera.WObject.Rotation = new Engine.Quaternion(-ay, ax, 0.0F);
        }
        private void Move()
        {
            Vector3D walkdir = Vector3D.Zero;

            Vector3D lookDir = FPSCamera.WObject.Forward;

            Vector3D walkForward = new Vector3D(lookDir.X, 0, lookDir.Z).Normalize();

            Vector3D rightDir = FPSCamera.WObject.Right;

            Vector3D walkRight = new Vector3D(rightDir.X, 0, rightDir.Z).Normalize();


            bool dirInputPressed = false;
            if (Input.IsPressed(GameInput.Key("Forward")))
            {
                walkdir += walkForward;
                dirInputPressed = true;
            }
            if (Input.IsPressed(GameInput.Key("Backward")))
            {
                walkdir -= walkForward;
                dirInputPressed = true;
            }

            if (Input.IsPressed(GameInput.Key("Right")))
            {
                walkdir -= walkRight;
                dirInputPressed = true;
            }
            if (Input.IsPressed(GameInput.Key("Left")))
            {
                walkdir += walkRight;
                dirInputPressed = true;
            }



            walkdir.Normalize();
            walkdir *= WalkForce;

            this._Rb.Velocity += walkdir * Time.DeltaTime;



            CheckGrounded();


            if (this._Rb.Velocity.XZ.Length > this.RunMaxSpeed)
            {
                double velY = this._Rb.Velocity.Y;
                this._Rb.Velocity = new Vector3D(this._Rb.Velocity.X, 0, this._Rb.Velocity.Z).Normalized * RunMaxSpeed;

                this._Rb.Velocity += Vector3D.Up * velY;

            }

            if (this._Rb.Velocity.Y < this.FallMaxSpeed)
            {
                double fallSpeed = this._Rb.Velocity.Y;

                this._Rb.Velocity *= new Vector3D(1, 0, 1);
                this._Rb.Velocity += new Vector3D(0, fallSpeed, 0);
            }

            // slow down
            if (!dirInputPressed)
            {
                /*if (this._Rb.Velocity.XZ.Length < 0.05F)
                {
                    this._Rb.Velocity *= Vector3D.Up;
                }

                else
                {*/
                Vector2D xz = this._Rb.Velocity.XZ * this.WalkSlowFactor * (Time.DeltaTime);

                this._Rb.Velocity *= Vector3D.Up;

                //this._Rb.Velocity += new Vector3D(xz.X, this._Rb.Velocity.Y, xz.Y);
                //}
            }
        }



        private void ViewHit()
        {
            if (RaycastChunk(new Ray(this.FPSCamera.WObject.Position, this.FPSCamera.WObject.Forward), HitRange, out RaycastChunkHit hit))
            {
                ViewRayHit = new RaycastChunkHit?(hit);
            }
            else
            {
                ViewRayHit = null;
            }
        }

        private void MainInteraction()
        {
            Cursor3D.Enabled = HitSphere.Enabled = ViewRayHit != null;
            if (Cursor3D.Enabled)
            {
                //HitSphere.Position = ViewRayHit.Value.GlobalPosition;
                //HitSphere.Up = ViewRayHit.Value.Normal;

                Cursor3D.Position = Vector3F.One * 0.5F + (Vector3F)(ViewRayHit.Value.LocalPosition + new Vector3I(ViewRayHit.Value.Chunk.Position.X * 16, 0, ViewRayHit.Value.Chunk.Position.Y * 16));

                if (Input.IsPressing(Keys.MouseLeftButton))
                {
                    Vector3I p = ViewRayHit.Value.LocalPosition;
                    ViewRayHit.Value.Chunk.Edit(p.X, p.Y, p.Z, ItemCache.Get<Block>("winecrash:air"));
                }

                if (Input.IsPressing(Keys.MouseRightButton))
                {
                    World.GlobalToLocal(ViewRayHit.Value.GlobalPosition + ViewRayHit.Value.Normal, out Vector3I cpos, out Vector3I bpos);

                    Ticket tck = Ticket.GetTicket(new Vector2I(cpos.X, cpos.Y));

                    tck?.Chunk.Edit(bpos.X, bpos.Y, bpos.Z, ItemCache.Get<Block>("winecrash:stone"));
                }
            }

        }

        protected override void Update()
        {
            CameraRotation();

            Move();

            ViewHit();

            MainInteraction();

            if (CurrentChunk == null || !CurrentChunk.BuiltOnce || Grounded)
            {
                this._Rb.Velocity *= new Vector3D(1, 0, 1);
            }

            TimeSinceLastJump += (float)Time.DeltaTime;

            if (Input.IsPressed(GameInput.Key("Jump")))
            {
                if (Grounded && TimeSinceLastJump >= JumpCooldown)
                {
                    TimeSinceLastJump = 0.0F;
                    this._Rb.Velocity += Vector3D.Up * JumpForce;
                }
            }

            Collisions();

            if (Input.IsPressed(Keys.F3) && Input.IsPressing(Keys.A))
            {
                Debug.Log("Reconstructing");
                Debug.Log(Chunk.Chunks.Count);
                Debug.Log("Total Light size: " + (Chunk.Chunks.Count * sizeof(uint) * 8192));
                foreach (Chunk chunk in Chunk.Chunks)
                {
                    Task.Run(chunk.Construct);
                }
            }
        }

        private void Collisions()
        {
            CheckTop();
            CheckRight();
            CheckLeft();
            CheckForward();
            ChechBackward();
            //CheckLeft();
        }

        #region Rays
        private void ChechBackward()
        {
            if (this._Rb.Velocity.Z > 0) return;

            if (
            //right up
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Up * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Right * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Backward * (float)_Bc.Extents.Z),
                Vector3D.Backward), 0.05D, out RaycastChunkHit hit)

            ||
            //right down
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Down * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Right * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Backward * (float)_Bc.Extents.Z),
                Vector3D.Backward), 0.05D, out hit)
            ||
            //left down
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Down * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Left * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Backward * (float)_Bc.Extents.Z),
                Vector3D.Backward), 0.05D, out hit)
            ||
            //left up
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Up * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Left * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Backward * (float)_Bc.Extents.Z),
                Vector3D.Backward), 0.05D, out hit))
            {
                this._Rb.Velocity *= new Vector3D(1, 1, 0);

                this.WObject.Position *= new Vector3F(1, 1, 0);

                this.WObject.Position -= Vector3F.Backward * ((float)hit.GlobalPosition.Z + (float)_Bc.Extents.Z);
            }
        }
        private void CheckForward()
        {
            if (this._Rb.Velocity.Z < 0) return;

            if (
            //right up
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Up * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Right * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Forward * (float)_Bc.Extents.Z),
                Vector3D.Forward), 0.05D, out RaycastChunkHit hit)

            ||
            //right down
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Down * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Right * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Forward * (float)_Bc.Extents.Z),
                Vector3D.Forward), 0.05D, out hit)
            ||
            //left down
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Down * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Left * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Forward * (float)_Bc.Extents.Z),
                Vector3D.Forward), 0.05D, out hit)
            ||
            //left up
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Up * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Left * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Forward * (float)_Bc.Extents.Z),
                Vector3D.Forward), 0.05D, out hit))
            {
                this._Rb.Velocity *= new Vector3D(1, 1, 0);

                this.WObject.Position *= new Vector3F(1, 1, 0);

                this.WObject.Position += Vector3F.Forward * ((float)hit.GlobalPosition.Z - (float)_Bc.Extents.Z);
            }
        }
        private void CheckLeft()
        {
            if (this._Rb.Velocity.X < 0) return;

            if (
            //front up
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Up * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Right * (float)_Bc.Extents.X) +
                    (Vector3F.Forward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Left), 0.05D, out RaycastChunkHit hit)

            ||
            //front down
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Down * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Right * (float)_Bc.Extents.X) +
                    (Vector3F.Forward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Left), 0.05D, out hit)
            ||
            //back down
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Down * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Right * (float)_Bc.Extents.X) +
                    (Vector3F.Backward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Left), 0.05D, out hit)
            ||
            //back up
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Up * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Right * (float)_Bc.Extents.X) +
                    (Vector3F.Backward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Left), 0.05D, out hit))
            {
                this._Rb.Velocity *= new Vector3D(0, 1, 1);

                this.WObject.Position *= new Vector3F(0, 1, 1);

                this.WObject.Position -= Vector3F.Left * ((float)hit.GlobalPosition.X - (float)_Bc.Extents.X);
            }
        }
        private void CheckRight()
        {
            if (this._Rb.Velocity.X > 0) return;

            if (
            //front up
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Up * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Left * (float)_Bc.Extents.X) +
                    (Vector3F.Forward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Right), 0.05D, out RaycastChunkHit hit)

            ||
            //front down
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Down * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Left * (float)_Bc.Extents.X) +
                    (Vector3F.Forward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Right), 0.05D, out hit)
            ||
            //back down
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Down * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Left * (float)_Bc.Extents.X) +
                    (Vector3F.Backward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Right), 0.05D, out hit)
            ||
            //back up
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Up * (float)_Bc.Extents.Y) * 0.8F +
                    (Vector3F.Left * (float)_Bc.Extents.X) +
                    (Vector3F.Backward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Right), 0.05D, out hit))
            {
                this._Rb.Velocity *= new Vector3D(0, 1, 1);

                this.WObject.Position *= new Vector3F(0, 1, 1);

                this.WObject.Position += Vector3F.Right * ((float)hit.GlobalPosition.X + (float)_Bc.Extents.X);
            }
        }
        private void CheckTop()
        {
            if(
            //front right
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Up * (float)_Bc.Extents.Y) +
                    (Vector3F.Right * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Forward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Up), 0.05D, out RaycastChunkHit hit)

            ||
            //front left
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Up * (float)_Bc.Extents.Y) +
                    (Vector3F.Left * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Forward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Up), 0.05D, out hit)
            ||
            //back left
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Up * (float)_Bc.Extents.Y) +
                    (Vector3F.Left * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Backward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Up), 0.05D, out hit)
            ||
            //back right
            RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Up * (float)_Bc.Extents.Y) +
                    (Vector3F.Right * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Backward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Up), 0.05D, out hit))
            {
                this._Rb.Velocity *= new Vector3D(1, 0, 1);

                this.WObject.Position *= new Vector3F(1, 0, 1);

                this.WObject.Position += Vector3F.Up * ((float)hit.LocalPosition.Y - (float)_Bc.Extents.Y - 0.05F);
            }
        }
        private void CheckGrounded()
        {
            if (this._Rb.Velocity.Y > 0)
            {
                Grounded = false;
                return;
            }
            //front right
            Grounded = RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Down * (float)_Bc.Extents.Y) +
                    (Vector3F.Right * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Forward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Down), 0.05D, out RaycastChunkHit hit);


            //front left
            if (!Grounded)
                Grounded = RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Down * (float)_Bc.Extents.Y) +
                    (Vector3F.Left * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Forward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Down), 0.05D, out hit);

            //back left
            if (!Grounded)
                Grounded = RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Down * (float)_Bc.Extents.Y) +
                    (Vector3F.Left * (float)_Bc.Extents.X) * 0.8F +
                    (Vector3F.Backward * (float)_Bc.Extents.Z) * 0.8F,
                Vector3D.Down), 0.05D, out hit);

            //back right
            if (!Grounded)
                Grounded = RaycastChunk(
                new Ray(
                    this.WObject.Position + (Vector3F.Down * (float)_Bc.Extents.Y) +
                    (Vector3F.Right * (float)_Bc.Extents.X) * 0.9F +
                    (Vector3F.Backward * (float)_Bc.Extents.Z) * 0.9F,
                Vector3D.Down), 0.05D, out hit);


            if (Grounded)
            {
                float ypos = this.WObject.Position.Y;

                this._Rb.Velocity *= new Vector3D(1, 0, 1);
                this.WObject.Position *= new Vector3F(1, 0, 1);
                this.WObject.Position += Vector3F.Up * ((float)hit.LocalPosition.Y + (float)_Bc.Extents.Y * 2 + 0.05F);
            }
            CurrentChunk = hit.Chunk;
        }
#endregion


        protected override void LateUpdate()
        {
            this.FPSCamera.WObject.Position = this.WObject.Position + CameraShift;
        }

        public bool RaycastChunk(Ray ray, double length, out RaycastChunkHit hit)
        {
            Vector3D pos = ray.Origin;
            double distance = 0.0D;

            double step = 0.1D;

            Vector3I cpos;
            Vector3I bpos;

            Ticket ticket = null;
            Chunk chunk = null;

            Block block = null;

            while (distance < length)
            {
                World.GlobalToLocal(pos, out cpos, out bpos);


                ticket = Ticket.GetTicket(cpos.XY);

                if(ticket != null)
                {
                    chunk = ticket.Chunk;

                    if(chunk != null)
                    {
                        block = chunk[bpos.X, bpos.Y, bpos.Z];
                        if (!block.Transparent)
                        {
                            Vector3F blockGlobalPos = (Vector3F)bpos + (Vector3F)cpos * new Vector3F(16, 1, 16) + Vector3F.One * 0.5F;


                            Vector3D relDir = (Vector3D)(blockGlobalPos - (Vector3F)pos).Normalized;

                            

                            hit = new RaycastChunkHit(pos, relDir.Face().Direction(), distance, block, chunk, bpos);

                            return true;
                        }
                    }
                }

                else
                {
                    hit = new RaycastChunkHit();
                    return false;
                }

                pos += ray.Direction * step;
                distance += step;
            }

            hit = new RaycastChunkHit(pos, Vector3D.Up, distance, block, chunk, Vector3I.Zero);
            return false;
        }
    }
}
