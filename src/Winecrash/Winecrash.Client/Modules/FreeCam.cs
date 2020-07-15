using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine;

namespace Winecrash.Client
{
    public class FreeCam : Module
    {
        public float MoveSpeed = 20.0F;
        public float MoveSpeedSensivity = 5.0F;
        private Vector2D Angles = new Vector2D();

        public Camera EditedCamera { get; set; }


        public static bool FreeCTRL = false;

        protected override void Creation()
        {
            EditedCamera = Camera.Main;

            //Input.MouseSensivity *= 5.0F;
        }

        protected override void Update()
        {
            if(Input.IsPressing(Keys.End))
            {
                FreeCTRL = !FreeCTRL;
            }

            if (!FreeCTRL) return;

            float newSpeed = (float)Input.MouseScrollDelta * MoveSpeedSensivity;

            this.MoveSpeed += newSpeed;
            //if(Input.IsPressed(Keys.Mouse))
            //Debug.Log(this.WObject.Position);

            /*Vector2D deltas = Input.MouseDelta;

            double ax = (Angles.X + (deltas.X * Input.MouseSensivity * (float)Time.DeltaTime)) % 360.0F;
            double ay = WMath.Clamp((Angles.Y + (deltas.Y * Input.MouseSensivity * (float)Time.DeltaTime)), -90.0F, 90.0F);

            Angles = new Vector2D(ax, ay);

            this.WObject.Rotation = new Quaternion(-ay, ax, 0.0F);*/

            Vector3F fwd = this.WObject.Forward;
            Vector3F rght = this.WObject.Right;
            Vector3F up = this.WObject.Up;

            Vector3F pos = this.WObject.Position;

            if (Input.IsPressed(Keys.Z))
            {
                fwd *= MoveSpeed * (float)Time.DeltaTime;
            }
            else if (Input.IsPressed(Keys.S))
            {
                fwd *= -MoveSpeed * (float)Time.DeltaTime;
            }
            else
            {
                fwd *= 0.0F;
            }

            if (Input.IsPressed(Keys.Q))
            {
                rght *= -MoveSpeed * (float)Time.DeltaTime;
            }
            else if (Input.IsPressed(Keys.D))
            {
                rght *= MoveSpeed * (float)Time.DeltaTime;
            }
            else
            {
                rght *= 0.0F;
            }

            if (Input.IsPressed(Keys.Space))
            {
                up *= MoveSpeed * (float)Time.DeltaTime;
            }
            else if (Input.IsPressed(Keys.LeftShift))
            {
                up *= -MoveSpeed * (float)Time.DeltaTime;
            }
            else
            {
                up *= 0.0F;
            }

            Player.Instance.WObject.Position += fwd - rght + up;
             
            
            if(Input.IsPressed(Keys.F3) && Input.IsPressing(Keys.A))
            {
                Debug.Log("Reconstructing");
                Debug.Log(Chunk.Chunks.Count);
                foreach (Chunk chunk in Chunk.Chunks)
                {
                    Task.Run(chunk.Construct);
                }
            }
            
            /*Vector3D finalForce = Vector3D.Zero;
            Vector3D flattenFwd = fwd;
            flattenFwd.Y = 0.0D;
            flattenFwd.Normalize();

            Vector3D flattenRght = rght;
            flattenFwd.Y = 0.0D;
            flattenFwd.Normalize();*/



            /*if(Input.IsPressed(Keys.Delete))
            {
                World.Instance.WObject.Delete();

                WEngine.TraceLayers();
            }*/

            /*if (Input.IsPressing(Keys.MouseLeftButton))
            {
                if(RaycastChunk(new Ray(this.WObject.Position, this.WObject.Forward), 5.0D, out _, out _, out _, out Chunk target, out Vector3D targetCoords))
                {
                    target[(int)targetCoords.X, (int)targetCoords.Y, (int)targetCoords.Z] = ItemCache.GetCacheIndex("winecrash:air");
                    target.BuildChunk(true);
                }
            }

            if (Input.IsPressing(Keys.MouseRightButton))
            {
                if (RaycastChunk(new Ray(this.WObject.Position, this.WObject.Forward), 5.0D, out Vector3D norm, out Vector3D gpos, out _, out Chunk target, out Vector3D targetCoords))
                {
                    ChunkCoords(gpos + norm * 0.5F, out Vector3D applyLocalPos, out Vector2I applyChunk);

                    Ticket.GetTicket(applyChunk).Chunk[(int)applyLocalPos.X, (int)applyLocalPos.Y, (int)applyLocalPos.Z] = ItemCache.GetCacheIndex("winecrash:stone");
                    Ticket.GetTicket(applyChunk).Chunk.BuildChunk(true);
                }
            }*/


            /*if (Input.IsPressing(Keys.Space) && RaycastChunk(new Ray(this.WObject.Position, this.WObject.Down), 1.05D, out _, out _, out _, out _, out _))
            {
                finalForce.Y += 200.0F;
            }



            if(Input.IsPressing(Keys.Z))
            {
                finalForce += flattenFwd * 10.0D * Time.DeltaTime;
            }

            if (Input.IsPressing(Keys.S))
            {
                finalForce += flattenFwd * -10.0D * Time.DeltaTime;
            }

            if(Input.IsPressing(Keys.D))
            {
                finalForce += flattenRght * -10.0D * Time.DeltaTime;
            }

            if(Input.IsPressing(Keys.Q))
            {
                finalForce += flattenRght * 10.0D * Time.DeltaTime;
            }


            RigidBody rb = this.WObject.GetModule<RigidBody>();
            rb.Velocity += finalForce;


            const double maxHorizontalSpeed = 3.0D;
            const double maxVerticalSpeed = 5.0D;

            Vector3D horVel = (rb.Velocity * new Vector3D(1, 0, 1));
            if (horVel.Length > maxHorizontalSpeed) horVel = horVel.Normalized * maxHorizontalSpeed;

            double vertVel = rb.Velocity.Y;

            if (vertVel > maxVerticalSpeed) vertVel = maxVerticalSpeed;

            rb.Velocity = new Vector3D(horVel.X, vertVel, horVel.Z);




            //bottom collider
            if (RaycastChunk(new Ray(this.WObject.Position, this.WObject.Down), 1D, out _, out _, out _, out _, out _))
            {
                rb.Velocity *= new Vector3D(1,0,1);

                this.WObject.Position += new Vector3F(0, 0.1F * (float)Time.DeltaTime, 0F);
            }

            //up collider
            if (RaycastChunk(new Ray(this.WObject.Position, this.WObject.Up), 1D, out _, out _, out _, out _, out _))
            {
                rb.Velocity *= new Vector3D(1, 0, 1);
            }

            //fwd collider
            if (RaycastChunk(new Ray(this.WObject.Position, this.WObject.Forward), 0.3D, out _, out _, out _, out _, out _))
            {
                rb.Velocity *= new Vector3D(1, 1, 0);
            }

            //back collider
            if (RaycastChunk(new Ray(this.WObject.Position, this.WObject.Backward), 0.3D, out _, out _, out _, out _, out _))
            {
                rb.Velocity *= new Vector3D(1, 1, 0);
            }


            //right collider
            if (RaycastChunk(new Ray(this.WObject.Position, this.WObject.Right), 0.3D, out _, out _, out _, out _, out _))
            {
                rb.Velocity *= new Vector3D(0, 1, 1);


                this.WObject.Position += new Vector3F(0.3F, 0, 0);
            }

            //left collider
            if (RaycastChunk(new Ray(this.WObject.Position, this.WObject.Left), 0.3D, out _, out _, out _, out _, out _))
            {
                rb.Velocity *= new Vector3D(0, 1, 1);

                this.WObject.Position += new Vector3F(-0.3F, 0, 0);
            }*/
        }

        /*public static bool RaycastChunk(Ray ray, double distance, out Vector3D normal, out Vector3D position, out double dist, out Chunk chunk, out Vector3D localCoords)
        {
            double epsilon = 0.1;
            Vector3D pos = ray.Origin;
            double dst = 0.0;

            Vector3D global;
            Vector3D local;

            while (dst < distance)
            {
                global = pos;

                ChunkCoords(global, out local, out Vector2I ochunk);

                Ticket tck = Ticket.GetTicket(ochunk);

                if (tck != null)
                {
                    Chunk cnk = tck.Chunk;

                    if (cnk != null && cnk.Blocks != null)
                    {

                        //Debug.Log(local);
                        if (ItemCache.Get(cnk[(int)local.X, (int)local.Y, (int)local.Z]).Identifier != "winecrash:air")
                        {
                            normal = Block.FaceToDirection(Block.DirectionToFace(ray.Direction));
                            position = pos;
                            dist = dst;
                            chunk = cnk;
                            localCoords = local;
                            return true;
                        }
                    }
                }

                else
                {
                    goto end;
                }

                pos += ray.Direction * epsilon;
                dst += epsilon;
            }

            end:
            normal = Vector3F.Up;
            position = Vector3F.Zero;
            dist = dst;
            chunk = null;
            localCoords = Vector3F.Zero;
            return false;

        }*/
    }
}
