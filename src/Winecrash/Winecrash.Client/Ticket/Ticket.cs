using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Winecrash.Engine;
using System.Threading;
using System.Diagnostics;

namespace Winecrash.Client
{
    public class Ticket : BaseObject, IEquatable<Ticket>
    {
        public const uint MaxLevel = 38;
        public const uint InaccessibleLevel = 34;
        public const uint BorderLevel = 33;
        public const uint TickingLevel = 32;

        internal static List<Ticket> _Tickets { get; private set; } = new List<Ticket>();

        public uint Level { get; internal set; }

        public TicketTypes InvokeType { get; internal set; }

        public int LifeTime { get; private set; }

        public Vector2I Position { get; }

        public Chunk Chunk { get; set; }

        public delegate void TicketCreationDelegate(Ticket created);

        public static TicketCreationDelegate OnCreation;

        public static Ticket CreateTicket(int x, int y, uint level, TicketTypes invokeType, TicketPreviousDirection previousDir, bool propagates = false, int lifeTime = -1)
        {
            if (level >= MaxLevel) return null;

            

            Ticket ticket = GetTicket(new Vector2I(x, y));

            if (ticket != null)
            {
                ticket.EditLevel(level, TicketEditTypes.Inferior);
            }

            else
            {
                ticket = new Ticket(level, invokeType, lifeTime, new Vector2I(x, y));
                _Tickets.Add(ticket);
            }

            //Debug.Log("Creating ticket " + x  + ";" + y);
            //  Recursively create tickets around in a square shape
            //
            //  36 36 36 36 36 36 36   a.k.a    n+3 n+3 n+3 n+3 n+3 n+3 n+3
            //  36 35 35 35 35 35 36            n+3 n+2 n+2 n+2 n+2 n+2 n+3 
            //  36 35 34 34 34 35 36            n+3 n+2 n+1 n+1 n+1 n+2 n+3
            //  36 35 34 33 34 35 36            n+3 n+2 n+1  n  n+1 n+2 n+3
            //  36 35 34 34 34 35 36            n+3 n+2 n+1 n+1 n+1 n+2 n+3
            //  36 35 35 35 35 35 36            n+3 n+2 n+2 n+2 n+2 n+2 n+3
            //  36 36 36 36 36 36 36            n+3 n+3 n+3 n+3 n+3 n+3 n+3
            //
            //  So create tickets in x+1, y+1, x-1, y-1, x+1 y+1, x-1 y-1, x+1 y-1, x-1 y+1
            //  (do not create if already exiting, edit its level if existing
            if (propagates)
            {
                // tickets chunks in square


                // for each level until max is reached
                for (int lvl = (int)level + 1, dist = 1; lvl < MaxLevel + 2; lvl++, dist++)
                {
                    // limits
                    int minx = x - dist;
                    int maxx = x + dist;
                    int maxy = y + dist;
                    int miny = y - dist;

                    // top line
                    for (int i = minx + 1; i < maxx + 1; i++)
                    {
                        CreateTicket(i, maxy, (uint)lvl, invokeType, TicketPreviousDirection.None, false, lifeTime);
                    }

                    // right line
                    for (int i = maxy - 1; i > miny - 1; i--)
                    {
                        CreateTicket(maxx, i, (uint)lvl, invokeType, TicketPreviousDirection.None, false, lifeTime);
                    }

                    // bottom line
                    for (int i = maxx - 1; i > minx - 1; i--)
                    {
                        CreateTicket(i, miny, (uint)lvl, invokeType, TicketPreviousDirection.None, false, lifeTime);
                    }

                    // left line
                    for (int i = miny + 1; i < maxy + 1; i++)
                    {
                        CreateTicket(minx, i, (uint)lvl, invokeType, TicketPreviousDirection.None, false, lifeTime);
                    }
                }
            }

            return ticket;
        }

        private Ticket(uint level, TicketTypes invokeType, int lifeTime, Vector2I position)
        {
            this.Position = position;
            this.Level = level;
            this.InvokeType = invokeType;
            this.LifeTime = lifeTime;

            Chunk = CreateChunk();

            OnCreation?.Invoke(this);
        }

        public bool Equals(Ticket ticket)
        {
            if (ticket is Ticket)
                return this.Position == ticket.Position;
            return false;
        }

        public void EditLevel(uint newLevel, TicketEditTypes type = TicketEditTypes.Override)
        {
            uint levelToSet = this.Level;

            switch (type)
            {
                case TicketEditTypes.Inferior:
                    {
                        if (newLevel < this.Level)
                        {
                            levelToSet = newLevel;
                        }
                    }
                    break;
                case TicketEditTypes.Superior:
                    {
                        if (newLevel > this.Level)
                        {
                            levelToSet = newLevel;
                        }
                    }
                    break;
            }

            this.Level = levelToSet;
        }

        public TicketLoadTypes LoadType
        {
            get
            {
                uint lvl = this.Level;
                if (lvl < TickingLevel) return TicketLoadTypes.EntityTicking;
                if (lvl < BorderLevel) return TicketLoadTypes.Ticking;
                if (lvl < InaccessibleLevel) return TicketLoadTypes.Border;
                return TicketLoadTypes.Inaccessible;
            }
        }

        

        /*public void Tick()
        {
            if (LifeTime == 0) return;

            if (LifeTime != -1) LifeTime--;

            TicketLoadTypes loadType = this.LoadType;
            if (loadType == TicketLoadTypes.Ticking)
            {
                Chunk.Tick();
            }
        }*/

        public override void Delete()
        {
            this.Chunk.Delete();
            this.Chunk = null;
            _Tickets.Remove(this);

            base.Delete();
        }

        private Chunk CreateChunk()
        {
            //Debug.Log("Creating chunk " + this.Position);
            WObject chunkwobj = new WObject($"Chunk [{this.Position.X};{this.Position.Y}]");
            chunkwobj.AddModule<MeshRenderer>();
            Chunk chunk = chunkwobj.AddModule<Chunk>();
            chunk.Ticket = this;
            //chunk.Position = new Vector3I(Position, 0);
            chunkwobj.Parent = World.Instance.WObject;
            
            chunk.Group = (this.Position.X / 4) * 1000 + ((this.Position.Y / 4) * 1000);

            //chunk.RunAsync = true;

            Task.Run(() =>
            {
                //try
                //{
                    chunk.Position = new Vector3I(chunk.Ticket.Position.X, chunk.Ticket.Position.Y, 0);
                    chunk.WObject.Position = new Vector3F(this.Position.X * Chunk.Width, 0, this.Position.Y * Chunk.Depth);
                    Chunk.TriggerAnyChunkCreated(chunk);

                    chunk.NorthNeighbor = Ticket.GetTicket(this.Position.XY + Vector2I.Up)?.Chunk;
                    chunk.SouthNeighbor = Ticket.GetTicket(this.Position.XY + Vector2I.Down)?.Chunk;
                    chunk.EastNeighbor = Ticket.GetTicket(this.Position.XY + Vector2I.Right)?.Chunk;
                    chunk.WestNeighbor = Ticket.GetTicket(this.Position.XY + Vector2I.Left)?.Chunk;


                    chunk._Blocks = Generator.GetChunk(this.Position.X, this.Position.Y, out bool generated);

#if RELEASE
                if(generated)
                {
                    Save();
                }
#endif
                    chunk.BuiltOnce = true;

                //Task.Run(chunk.GenerateLights);

                chunk.ForceNextConstruct = true;
                chunk.BuildEndFrame = true;
                //chunk.Construct();
                    //chunk.BuildEndFrame = true;
                    Chunk.TriggerAnyChunkFirstBuilt(chunk);
                /*}
                catch(Exception e)
                {
                    var st = new StackTrace(e, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);


                    Engine.Debug.LogError("Error when building chunk " + this.Position + " : " + st);
                }*/
            });

            /*if (gen)
            {
                Save(chunk, this.Position.X, this.Position.Y);
            }*/
            return chunk;
        }

        private static void Save(Chunk chunk, int x, int y)
        {
            string worldPath = "save";
            string extention = ".json";
            string fullPath = worldPath + $"/c{x}_{y}" + extention;

            File.WriteAllText(fullPath, chunk.ToJSON());
        }

        public void Save()
        {
            Save(this.Chunk, this.Position.X, this.Position.Y);
        }

        public static Ticket GetTicket(Vector2I pos)
        {
            if (_Tickets == null)
            {
                return null;
            }
            object obj = new object();

            int n = _Tickets.Count();
            Ticket[] tickets = new Ticket[n];
            lock (obj)
            {
                _Tickets.CopyTo(0, tickets, 0, n);
            }

            return tickets.FirstOrDefault(t => t.Position == pos);
        }
    }
}
