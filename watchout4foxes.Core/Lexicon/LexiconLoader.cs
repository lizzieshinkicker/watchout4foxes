using watchout4foxes.Core.Models;

namespace watchout4foxes.Core.Lexicon;

public static class LexiconLoader
{
    public static ILexicon LoadFromFile(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using var reader = new StreamReader(stream);
        return new IndexedLexicon(LoadFromTsv(reader));
    }

    private static IEnumerable<WordEntry> LoadFromTsv(TextReader reader)
    {
        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            var parts = line.Split('\t', StringSplitOptions.TrimEntries);
            if (parts.Length < 3) continue;

            var text = parts[0];
            if (!Enum.TryParse<PartOfSpeech>(parts[1], ignoreCase: true, out var pos))
                continue;

            if (!Enum.TryParse<CommonalityLevel>(parts[2], ignoreCase: true, out var commonality))
                continue;

            yield return new WordEntry(text, pos, commonality);
        }
    }
}