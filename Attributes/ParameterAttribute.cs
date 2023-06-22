namespace Loxifi.Attributes
{
    public class ParameterAttribute : Attribute
    {
        public ParameterAttribute(string name, bool serializeDefault = true)
        {
            this.Name = name;
            this.SerializeDefault = serializeDefault;
        }

        public string Name { get; set; }

        public bool SerializeDefault { get; set; } = false;

        public virtual string Serialize(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (!this.SerializeDefault)
            {
                Type oType = value.GetType();

                if (oType.IsPrimitive)
                {
                    object oInstance = Activator.CreateInstance(oType);
                    if (Equals(oInstance, value))
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    if (value is null)
                    {
                        return string.Empty;
                    }
                }
            }

            string sValue = value.ToString();

            if (sValue.Contains("\""))
            {
                sValue = sValue.Replace("\"", "\\\"");
            }

            if (sValue.Contains(" "))
            {
                sValue = $"{sValue}";
            }

            return $"{this.Name} {sValue}";
        }
    }
}