using System;
using System.ComponentModel;
using System.Globalization;

namespace Common.Unity
{
    public sealed class SerializableGUIDTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value is string stringValue && Guid.TryParse(stringValue, out Guid guid) ? new SerializableGUID(guid) : base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type targetType)
        {
            return targetType == typeof(string) && value is SerializableGUID guid ? guid.AsGuid().ToString() : base.ConvertTo(context, culture, value, targetType);
        }
    }
}
