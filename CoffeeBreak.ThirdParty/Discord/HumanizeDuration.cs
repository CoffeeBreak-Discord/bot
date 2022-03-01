using System.Text.RegularExpressions;

namespace CoffeeBreak.ThirdParty.Discord;
public class HumanizeDuration
{
    private string _input;
    public HumanizeDuration(string input)
    {
        _input = input;
    }

    private TimeSpan Parse()
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

        var timeSpan = new TimeSpan();

        foreach (var unit in units)
        {
            var matches = Regex.Matches(_input, unit.Key);
            foreach (Match match in matches)
            {
                var amount = System.Convert.ToInt32(match.Groups[1].Value);
                timeSpan = timeSpan.Add(TimeSpan.FromMilliseconds(unit.Value * amount));
            }
        }

        return timeSpan;
    }

    public TimeSpan ToTimeSpan() => this.Parse();

    public DateTime ToDateTime()
    {
        return DateTime.Now.Add(this.Parse());
    }
}