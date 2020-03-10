namespace Tand.Core.Tests
{

    public interface ITandSample
    {
        [Tand(typeof(LogTarget<ITandSample>))]
        int LogMyParams(string s, int i);
    }

    public class TandSample : ITandSample
    {

        private int _counter;
        
        public string ContextSample { get; set; }
        
        public int LogMyParams(string s, int i)
        {
            return ++_counter;
        }
    }
}