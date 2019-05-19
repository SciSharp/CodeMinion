namespace CodeMinion.Core.Models
{
    public class Argument
    {
        public bool IsNullable { get; set; }
        public bool IsValueType { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public bool IsNamedArg { get; set; }
        public string Description { get; set; }
    }
}