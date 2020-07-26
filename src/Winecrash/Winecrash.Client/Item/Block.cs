using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Game
{
    public class Block : Item
    {
        public bool Transparent { get; set; } = false;

        public void Tick(TickType type, Chunk chunk, Vector3I position) 
        {
            switch(type)
            {
                case TickType.World:
                    WorldTick(chunk, position);
                    break;

                case TickType.Block:
                    BlockTick(chunk, position);
                    break;
            }
        }

        protected virtual void WorldTick(Chunk chunk, Vector3I position)
        {

        }

        protected virtual void BlockTick(Chunk chunk, Vector3I position)
        {

        }

        public static void PlayerTickNeighbors(Chunk chunk, Vector3I position)
        {
            Chunk tickChunk;
            Vector3I tickPosition;

            // West block //
            if (position.X == 0) // if edge
            {
                tickChunk = Ticket.GetTicket(chunk.Position.XY + Vector2I.Left)?.Chunk;

                tickPosition = new Vector3I(15, position.Y, position.Z);
            }
            else
            {
                tickChunk = chunk;

                tickPosition = new Vector3I(position.X - 1, position.Y, position.Z);
            }

            if (tickChunk) World.TickOnNextTick.Add(new TickWaitItem(TickType.Block, tickChunk, tickPosition, chunk[tickPosition.X, tickPosition.Y, tickPosition.Z]));


            // East block //
            if (position.X == 15) // if edge
            {
                tickChunk = Ticket.GetTicket(chunk.Position.XY + Vector2I.Right)?.Chunk;

                tickPosition = new Vector3I(0, position.Y, position.Z);
            }
            else
            {
                tickChunk = chunk;

                tickPosition = new Vector3I(position.X + 1, position.Y, position.Z);
            }

            if (tickChunk) World.TickOnNextTick.Add(new TickWaitItem(TickType.Block, tickChunk, tickPosition, chunk[tickPosition.X, tickPosition.Y, tickPosition.Z]));


            // North block //
            if (position.Z == 15) // if edge
            {
                tickChunk = Ticket.GetTicket(chunk.Position.XY + Vector2I.Up)?.Chunk;

                tickPosition = new Vector3I(position.X, position.Y, 0);
            }
            else
            {
                tickChunk = chunk;

                tickPosition = new Vector3I(position.X, position.Y, position.Z + 1);
            }

            if (tickChunk) World.TickOnNextTick.Add(new TickWaitItem(TickType.Block, tickChunk, tickPosition, chunk[tickPosition.X, tickPosition.Y, tickPosition.Z]));


            // South block //
            if (position.Z == 0) // if edge
            {
                tickChunk = Ticket.GetTicket(chunk.Position.XY + Vector2I.Down)?.Chunk;

                tickPosition = new Vector3I(position.X, position.Y, 15);
            }
            else
            {
                tickChunk = chunk;

                tickPosition = new Vector3I(position.X, position.Y, position.Z - 1);
            }

            if (tickChunk) World.TickOnNextTick.Add(new TickWaitItem(TickType.Block, tickChunk, tickPosition, chunk[tickPosition.X, tickPosition.Y, tickPosition.Z]));


            if (position.Y != 0)
            {
                tickChunk = chunk;

                tickPosition = new Vector3I(position.X, position.Y - 1, position.Z);

                World.TickOnNextTick.Add(new TickWaitItem(TickType.Block, chunk, tickPosition, chunk[tickPosition.X, tickPosition.Y, tickPosition.Z]));
            }

            if (position.Y != 255)
            {
                tickChunk = chunk;

                tickPosition = new Vector3I(position.X, position.Y + 1, position.Z);

                World.TickOnNextTick.Add(new TickWaitItem(TickType.Block, tickChunk, tickPosition, chunk[tickPosition.X, tickPosition.Y, tickPosition.Z]));
            }
        }
    }
}
