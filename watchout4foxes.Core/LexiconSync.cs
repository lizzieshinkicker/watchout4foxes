using System.Net.Http.Headers;
using watchout4foxes.Core.Models;

namespace watchout4foxes.Core;

public static class LexiconSync
{
    private const string NorvigFrequencyUrl = "https://norvig.com/ngrams/count_1w.txt";
    private const string MobyPosUrl = "https://raw.githubusercontent.com/isoraqathedh/cl-acronyms/master/mobyposi.i";

    private const string LdnoobwUrl = "https://raw.githubusercontent.com/LDNOOBW/List-of-Dirty-Naughty-Obscene-and-Otherwise-Bad-Words/master/en";
    private const string SlursUrl = "https://raw.githubusercontent.com/zacanger/profane-words/master/words.json";

    public static async Task GenerateLexiconTsvAsync(string outputPath, IProgress<string>? progress = null)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("watchout4foxes", "1.0"));

        progress?.Report("Downloading multi-corpus sanitization filters (slurs, epithets, hate speech)...");
        var exactBlocklist = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await LoadBlocklistFromUrlAsync(client, LdnoobwUrl, exactBlocklist, progress);

        await LoadExtendedBlocklistAsync(client, SlursUrl, exactBlocklist, progress);

        LoadLocalBlocklist("blocked-words.txt", exactBlocklist, progress);
        LoadLocalBlocklist("watchout4foxes/blocked-words.txt", exactBlocklist, progress);

        progress?.Report($"Sanitization loaded with {exactBlocklist.Count:N0} filtered terms.");

        progress?.Report("Downloading Norvig frequency ranks (Google 1-Trillion corpus)...");
        var frequencyRanks = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        await using (var freqStream = await client.GetStreamAsync(NorvigFrequencyUrl))
        using (var reader = new StreamReader(freqStream))
        {
            var rank = 1;
            while (await reader.ReadLineAsync() is { } line)
            {
                var tab = line.IndexOf('\t');
                if (tab <= 0) continue;

                var word = line[..tab].Trim();
                if (IsValidWord(word) && !IsOffensive(word, exactBlocklist))
                {
                    frequencyRanks.TryAdd(word, rank++);
                }
            }
        }
        progress?.Report($"Loaded {frequencyRanks.Count:N0} clean frequency rankings.");

        progress?.Report("Downloading Moby Part-of-Speech database...");
        var entries = new List<WordEntry>();

        await using (var mobyStream = await client.GetStreamAsync(MobyPosUrl))
        using (var reader = new StreamReader(mobyStream))
        {
            while (await reader.ReadLineAsync() is { } line)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var delimiter = line.IndexOfAny(['*', '×']);
                if (delimiter <= 0) continue;

                var word = line[..delimiter].Trim();

                if (!IsValidWord(word) || char.IsUpper(word[0]) || IsOffensive(word, exactBlocklist))
                    continue;

                var tagStr = line[(delimiter + 1)..];
                var pos = MapMobyTags(tagStr);
                if (pos == PartOfSpeech.None) continue;

                var commonality = CommonalityLevel.Obscure;
                if (frequencyRanks.TryGetValue(word, out var rank))
                {
                    commonality = rank switch
                    {
                        <= 1_500 => CommonalityLevel.VeryCommon,
                        <= 6_000 => CommonalityLevel.Common,
                        <= 18_000 => CommonalityLevel.SomewhatCommon,
                        <= 45_000 => CommonalityLevel.Uncommon,
                        _ => CommonalityLevel.Obscure
                    };
                }

                entries.Add(new WordEntry(word.ToLowerInvariant(), pos, commonality));
            }
        }

        progress?.Report($"Compiling {entries.Count:N0} sanitized dictionary entries to {outputPath}...");

        await using var outStream = File.Create(outputPath);
        await using var writer = new StreamWriter(outStream);
        await writer.WriteLineAsync("# watchout4foxes Production Lexicon");
        await writer.WriteLineAsync("# Text\tPartOfSpeech\tCommonalityLevel");

        foreach (var entry in entries.DistinctBy(e => e.Text, StringComparer.OrdinalIgnoreCase))
        {
            await writer.WriteLineAsync($"{entry.Text}\t{entry.Pos}\t{entry.Commonality}");
        }

        progress?.Report("Sanitization compilation complete!");
    }

    private static bool IsOffensive(string word, HashSet<string> blocklist) =>
        blocklist.Contains(word) ||
        (word.EndsWith('s') && blocklist.Contains(word[..^1])) ||
        (word.EndsWith("es") && blocklist.Contains(word[..^2])) ||
        (word.EndsWith("ed") && blocklist.Contains(word[..^2])) ||
        (word.EndsWith("ing") && blocklist.Contains(word[..^3])) ||
        (word.EndsWith('y') && blocklist.Contains(word[..^1]));

    private static bool IsValidWord(string word) =>
        word.Length >= 3 &&
        word.All(char.IsAsciiLetter) &&
        word.Any(c => "aeiouy".Contains(char.ToLowerInvariant(c)));

    private static async Task LoadBlocklistFromUrlAsync(HttpClient client, string url, HashSet<string> set, IProgress<string>? progress)
    {
        try
        {
            await using var stream = await client.GetStreamAsync(url);
            using var reader = new StreamReader(stream);
            while (await reader.ReadLineAsync() is { } line)
            {
                var trimmed = line.Trim();
                if (trimmed.Length >= 2 && !trimmed.StartsWith('#'))
                {
                    set.Add(trimmed);
                }
            }
        }
        catch (Exception ex)
        {
            progress?.Report($"Notice: Blocklist source ({url}) skipped: {ex.Message}");
        }
    }

    private static async Task LoadExtendedBlocklistAsync(HttpClient client, string url, HashSet<string> set, IProgress<string>? progress)
    {
        try
        {
            var json = await client.GetStringAsync(url);
            var tokens = json.Split([',', '[', ']', '"', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var token in tokens)
            {
                if (token.Length >= 2 && token.All(char.IsLetter))
                {
                    set.Add(token);
                }
            }
        }
        catch (Exception ex)
        {
            progress?.Report($"Notice: Extended slur source skipped: {ex.Message}");
        }
    }

    private static void LoadLocalBlocklist(string path, HashSet<string> set, IProgress<string>? progress)
    {
        if (!File.Exists(path)) return;

        foreach (var line in File.ReadLines(path))
        {
            var trimmed = line.Trim();
            if (trimmed.Length >= 2 && !trimmed.StartsWith('#'))
            {
                set.Add(trimmed);
            }
        }
        progress?.Report($"Loaded custom local blocklist rules from '{path}'.");
    }

    private static PartOfSpeech MapMobyTags(string tags)
    {
        var pos = PartOfSpeech.None;
        foreach (var ch in tags)
        {
            pos |= ch switch
            {
                'N' => PartOfSpeech.Noun,
                't' => PartOfSpeech.VerbTransitive,
                'i' => PartOfSpeech.VerbIntransitive,
                'A' => PartOfSpeech.Adjective,
                'v' => PartOfSpeech.Adverb,
                'P' => PartOfSpeech.Preposition,
                '!' => PartOfSpeech.Interjection,
                _ => PartOfSpeech.None
            };
        }
        return pos;
    }
}