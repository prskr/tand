namespace Tand.Core.Models
{
    public readonly struct MethodArgument
    {
        public MethodArgument(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }

        public void Deconstruct(out string name, out object value)
        {
            name = Name;
            value = Value;
        }
    }
}