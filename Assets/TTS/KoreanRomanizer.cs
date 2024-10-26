using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class KoreanRomanizer
{
    private const int HANGUL_BASE = 0xAC00;
    private const int HANGUL_END = 0xD7A3;
    private const int CHOSUNG_BASE = 588;
    private const int JUNGSUNG_BASE = 28;

    private static readonly string[] CHOSUNG = new string[]
    {
        "g", "kk", "n", "d", "tt", "r", "m", "b", "pp", "s",
        "ss", "", "j", "jj", "ch", "k", "t", "p", "h"
    };

    private static readonly string[] JUNGSUNG = new string[]
    {
        "a", "ae", "ya", "yae", "eo", "e", "yeo", "ye", "o",
        "wa", "wae", "oe", "yo", "u", "wo", "we", "wi",
        "yu", "eu", "ui", "i"
    };

    private static readonly string[] JONGSUNG = new string[]
    {
        "", "k", "g", "gs", "n", "nj", "nh", "d", "l", "lg",
        "lm", "lb", "ls", "lt", "lp", "lh", "m", "b", "bs",
        "s", "ss", "ng", "j", "ch", "k", "t", "p", "h"
    };

    // Family names with special romanization
    private static readonly Dictionary<char, string> FAMILY_NAMES = new Dictionary<char, string>
    {
        {'김', "kim"},
        {'박', "park"},
        {'이', "lee"},
        {'최', "choi"},
        {'정', "jung"},
        {'강', "kang"},
        {'조', "cho"},
        {'윤', "yoon"},
        {'장', "jang"},
        {'임', "lim"}
    };

    public static string ConvertToRomanization(string koreanText)
    {
        if (string.IsNullOrEmpty(koreanText)) return string.Empty;

        StringBuilder result = new StringBuilder();
        bool isWordStart = true;

        for (int i = 0; i < koreanText.Length; i++)
        {
            char c = koreanText[i];

            // Check for word boundaries
            if (char.IsWhiteSpace(c))
            {
                isWordStart = true;
                result.Append(c);
                continue;
            }

            // Check if this character is the start of a family name
            if (isWordStart && FAMILY_NAMES.ContainsKey(c))
            {
                result.Append(FAMILY_NAMES[c]);
                result.Append('-');
                isWordStart = false;
                continue;
            }

            if (IsHangul(c))
            {
                int charCode = (int)c;
                int charIndex = charCode - HANGUL_BASE;
                int chosungIndex = charIndex / CHOSUNG_BASE;
                int jungsungIndex = (charIndex % CHOSUNG_BASE) / JUNGSUNG_BASE;
                int jongsungIndex = charIndex % JUNGSUNG_BASE;

                // Handle ㅇ and other consonants
                if (chosungIndex == 11) // ㅇ
                {
                    result.Append(JUNGSUNG[jungsungIndex]);
                }
                else
                {
                    result.Append(CHOSUNG[chosungIndex]);
                    result.Append(JUNGSUNG[jungsungIndex]);
                }

                if (jongsungIndex != 0)
                {
                    result.Append(JONGSUNG[jongsungIndex]);
                }

                // Add hyphen between syllables within the same word
                if (i < koreanText.Length - 1 && IsHangul(koreanText[i + 1]))
                {
                    result.Append('-');
                }

                isWordStart = false;
            }
            else
            {
                result.Append(c);
                isWordStart = true;
            }
        }

        return CleanupHyphens(result.ToString());
    }

    private static bool IsHangul(char c)
    {
        int charCode = (int)c;
        return charCode >= HANGUL_BASE && charCode <= HANGUL_END;
    }

    private static string CleanupHyphens(string text)
    {
        // Remove double hyphens
        text = System.Text.RegularExpressions.Regex.Replace(text, "-+", "-");
        // Remove hyphens before spaces
        text = System.Text.RegularExpressions.Regex.Replace(text, "-\\s", " ");
        // Remove trailing hyphens
        return text.TrimEnd('-');
    }
}