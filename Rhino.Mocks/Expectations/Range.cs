
namespace Rhino.Mocks.Expectations
{
    /// <summary>
    /// Range for expected method calls
    /// </summary>
    public class Range
    {
        private readonly int minimum;
        private readonly int? maximum;

        /// <summary>
        /// Gets the minimum number of calls
        /// </summary>
        public int Minimum
        {
            get { return minimum; }
        }

        /// <summary>
        /// Gets the maximum number of calls
        /// </summary>
        public int? Maximum
        {
            get { return maximum; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public Range(int minimum, int? maximum)
        {
            this.minimum = minimum;
            this.maximum = maximum;
        }

        /// <summary>
        /// Returns the expected range in 
        /// string form
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (minimum == 0)
                return maximum.ToString();

            if (maximum != null && minimum != maximum.Value)
                return string.Format("{0}..{1}", minimum, maximum);

            return minimum.ToString();
        }
    }
}
