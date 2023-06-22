namespace Loxifi.Attributes
{
    public class FlagParameterAttribute : ParameterAttribute
    {
        public FlagParameterAttribute(string name) : base(name, false)
        {
        }

        public override string Serialize(object value)
        {
            if (value is not bool b)
            {
                throw new ArgumentException($"{nameof(FlagParameterAttribute)} can only be used on property of type bool");
            }

            if (b)
            {
                return $"{this.Name}";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}