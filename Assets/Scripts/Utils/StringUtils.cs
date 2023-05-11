using System;
using System.Text;

namespace Utils
{
	/// <summary>
	/// A static class used for Short Number functions.
	/// </summary>
	public static class StringUtils
	{
		/// <summary>
		/// Shortens a number and returns it as a string.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static string GetShortenedNumberAsString(int number)
		{
			int i = (int)Math.Pow(10, (int)Math.Max(0, Math.Log10(number) - 2));

			if (i == 0)
				return "0";

			number = number / i * i;

			if (number >= 1000000000)
				return (number / 1000000000D).ToString("0.##") + "B";
			if (number >= 1000000)
				return (number / 1000000D).ToString("0.##") + "M";
			if (number >= 1000)
				return (number / 1000D).ToString("0.##") + "K";

			return number.ToString("#,0");
		}

		/// <summary>
		/// Formats a string so that a space is inserted before any Uppcase, excluuding the first letter.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string FormatStringByUpperCase(string text)
		{
			// If string is empty, return empty.
			if (string.IsNullOrWhiteSpace(text))
				return default;

			// Create new string builder.
			StringBuilder newText = new StringBuilder(text.Length * 2);

			// Add first letter.
			newText.Append(text[0]);

			// Loop through all letters and check for spaces.
			for (int i = 1; i < text.Length; i++)
			{
				// Checks if a letter is uppercase and inserts a space between
				if (char.IsUpper(text[i]) && text[i - 1] != ' ')
					newText.Append(' ');
				newText.Append(text[i]);
			}

			return newText.ToString();
		}

		/// <summary>
		/// Converts a string to Camel Case
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToCamelCase(string value)
		{
			string[] temp = value.Split('_');
			StringBuilder sb = new StringBuilder(value.Length);

			foreach (string s in temp)
				sb.Append(s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower());

			return sb.ToString();
		}

		// Source https://stackoverflow.com/a/37368176
		/// <summary>
		/// Returns true if character is whitespace.
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		public static bool IsWhitespace(this char character)
		{
			switch (character)
			{
				case '\u0020':
				case '\u00A0':
				case '\u1680':
				case '\u2000':
				case '\u2001':
				case '\u2002':
				case '\u2003':
				case '\u2004':
				case '\u2005':
				case '\u2006':
				case '\u2007':
				case '\u2008':
				case '\u2009':
				case '\u200A':
				case '\u202F':
				case '\u205F':
				case '\u3000':
				case '\u2028':
				case '\u2029':
				case '\u0009':
				case '\u000A':
				case '\u000B':
				case '\u000C':
				case '\u000D':
				case '\u0085':
					{
						return true;
					}

				default:
					{
						return false;
					}
			}
		}

		// Source https://stackoverflow.com/a/37368176
		/// <summary>
		/// Returns a string with all whitespace removed.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string RemoveWhitespaces(this string text)
		{
			int textLength = text.Length;

			char[] textCharacters = text.ToCharArray();

			int currentWhitespacelessTextLength = 0;

			for (int currentCharacterIndex = 0; currentCharacterIndex < textLength; ++currentCharacterIndex)
			{
				char currentTextCharacter = textCharacters[currentCharacterIndex];

				if (currentTextCharacter.IsWhitespace())
				{
					continue;
				}

				textCharacters[currentWhitespacelessTextLength++] = currentTextCharacter;
			}

			return new string(textCharacters, 0, currentWhitespacelessTextLength);
		}

		// Source https://stackoverflow.com/a/37368176
		// See here for alternatives: https://stackoverflow.com/questions/3210393/how-do-i-remove-all-non-alphanumeric-characters-from-a-string-except-dash
		/// <summary>
		/// Removes all special characters from a string.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string RemoveSpecialCharacters(this string text)
		{
			int textLength = text.Length;

			char[] textCharacters = text.ToCharArray();

			int currentWhitespacelessTextLength = 0;

			for (int currentCharacterIndex = 0; currentCharacterIndex < textLength; ++currentCharacterIndex)
			{
				char currentTextCharacter = textCharacters[currentCharacterIndex];

				if (!char.IsLetterOrDigit(currentTextCharacter) && !currentTextCharacter.IsWhitespace())
				{
					continue;
				}

				textCharacters[currentWhitespacelessTextLength++] = currentTextCharacter;
			}

			return new string(textCharacters, 0, currentWhitespacelessTextLength);
		}
	}
}