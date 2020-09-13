namespace WEngine
{
    /// <summary>
    /// Implements very basic vectors properties.
    /// </summary>
    public interface IVectorable
    {
        /// <summary>
        /// The number of dimensions of the vector.
        /// </summary>
        int Dimensions { get; }

        /// <summary>
        /// The squared length of the vector. Faster to compute but has to been square rooted.
        /// </summary>
        double SquaredLength { get; }
        /// <summary>
        /// The length of the vector.
        /// </summary>
        double Length { get; }
    }
}
