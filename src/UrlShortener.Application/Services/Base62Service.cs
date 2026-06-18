namespace UrlShortener.Application.Services;

public class Base62Service
{
    private const string Base62Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public string ToBase62(long number)
    {
        if (number == 0) return "0";

        var result = new System.Text.StringBuilder();
        while (number > 0)
        {
            result.Insert(0, Base62Alphabet[(int)(number % 62)]);
            number /= 62;
        }

        return result.ToString();
    }

    public long FromBase62(string encoded)
    {
        long result = 0;
        foreach (var c in encoded)
        {
            result = result * 62 + Base62Alphabet.IndexOf(c);
        }

        return result;
    }
}