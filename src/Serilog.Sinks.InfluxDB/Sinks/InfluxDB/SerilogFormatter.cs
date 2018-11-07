using System.Linq;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Formatters;
using Serilog.Events;

namespace Serilog.Sinks.InfluxDB
{
    public class SerilogFormatter : PointFormatter
    {
        protected override string FormatPointField(string key, object value)
        {
            //TODO Hanlde Non Scaler

            var type = value.GetType();
            if (type == typeof(StructureValue))
            {
                var structure = (StructureValue) value;
                return structure.Properties.Select(c => FormatPointField($"{key}.{c.Name}", c.Value)).ToCommaSeparatedString();
            }
            else if (type == typeof(SequenceValue))
            {
                var seq = (SequenceValue) value;
                return base.FormatPointField(key, seq.Elements.Select(k => k.ToString()).ToCommaSeparatedString());
            }
            else if (type == typeof(ScalarValue))
            {
                var scalarValue = ((ScalarValue) value);
                if (scalarValue.Value is null)
                {
                    return "";
                }
                else
                {
                    return base.FormatPointField(key, scalarValue.Value);
                }
            }


            return base.FormatPointField(key, value);
        }
    }
}