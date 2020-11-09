using System;

namespace WEngine.Dab.Shader
{
    /// <summary>
    /// The capabilities of a shader
    /// </summary>
    [Flags]
    public enum Capabilities
    {
        /// <summary>
        /// No capability at all.
        /// </summary>
        None = 0,
        /// <summary>
        /// Shader is able to move input vertices.
        /// </summary>
        Vertex = 1,
        /// <summary>
        /// Shader is able to apply tesselation.
        /// </summary>
        [Obsolete("Work In Progress. Tesselation is not supported yet and will be ignored.")]
        Tesselation = 2,
        /// <summary>
        /// Shader is able to change the input geometry.
        /// </summary>
        [Obsolete("Work In Progress. Geometry shaders are not supported yet and will be ignored.")]
        Geometry = 4,
        /// <summary>
        /// Shader is able to change the output color
        /// </summary>
        Fragment = 8,
        /// <summary>
        /// Shader is able to be used as compute
        /// </summary>
        [Obsolete("Work In Progress. Compute shaders are not supported yet and will be ignored.")]
        Compute = 16
    }
}