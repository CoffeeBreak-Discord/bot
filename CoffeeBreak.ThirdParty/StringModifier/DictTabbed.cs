namespace CoffeeBreak.ThirdParty.StringModifier;
public class DictTabbed
{
    public enum Align
    {
        Left, Right
    }

    public static string Generate(Dictionary<string, string> dict, Align align = Align.Left)
    {
        List<string> list = new List<string>();
        int maxKeyLen = 0;
        int maxValLen = 0;

        // Set maximum key and value length
        foreach (KeyValuePair<string, string> item in dict)
        {
            maxKeyLen = item.Key.Length > maxKeyLen ? item.Key.Length : maxKeyLen;
            maxValLen = item.Value.Length > maxValLen ? item.Value.Length : maxValLen;
        }

        // Print the string
        foreach (KeyValuePair<string, string> item in dict)
        {
            list.Add($"{item.Key.PadRight(maxKeyLen, ' ')}: " + (align == Align.Left ? item.Value : item.Value.PadLeft(maxValLen, ' ')));
        }

        return string.Join("\n", list);
    }
}
