using WEngine;

namespace Winecrash.Generation
{
    public interface IGenerator
    {
        ushort[] GetBlocks(Vector2I position, uint chunkWidth = 16, uint chunkDepth = 16);
    }
}