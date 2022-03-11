using System.Text.RegularExpressions;

namespace CoffeeBreak.ThirdParty;
public class HumanizeDuration
{
    public DateTimeOffset time = new DateTimeOffset(DateTime.Now);
    public HumanizeDuration(string input)
    {
        var units = new Dictionary<string, int>()
        {
            {@"(\d+)(ms|mili[|s]|milisecon[|s])", 1 },
            {@"(\d+)(s|sec|second[|s])", 1000 },
            {@"(\d+)(m|min[|s])", 60000 },
            {@"(\d+)(h|hour[|s])", 3600000 },
            {@"(\d+)(d|day[|s])", 86400000 },
            {@"(\d+)(w|week[|s])", 604800000 },
        };

        foreach (var unit in units)
        {
            var matches = Regex.Matches(input, unit.Key);
            foreach (Match match in matches)
            {
                var amount = System.Convert.ToInt64(match.Groups[1].Value);
                this.time = this.time.AddMilliseconds(unit.Value * amount);
            }
        }
    }

    public TimeSpan ToTimeSpan() => this.time.Offset;

    public DateTime ToDateTime() => this.time.DateTime;
}