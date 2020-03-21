using System.Collections.Generic;
using System.Linq;
using Tand.Core.Api;

namespace Tand.Core.Benchmarks
{
    public interface IRationalsProvider
    {
        [Tand(typeof(ExecutionTimeTand<IRationalsProvider>))]
        ICollection<Rational> Generate(IEnumerable<(int n, int d)> input);
    }

    public class RationalsProvider : IRationalsProvider
    {
        public ICollection<Rational> Generate(IEnumerable<(int n, int d)> input) => input.Select(tuple => new Rational(tuple.n, tuple.d)).ToList();
    }
}