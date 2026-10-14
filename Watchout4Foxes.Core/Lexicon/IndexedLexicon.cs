using System.Collections.Frozen;
using watchout4foxes.Core.Models;

namespace watchout4foxes.Core.Lexicon;

public sealed class IndexedLexicon : ILexicon
{
    private static readonly PartOfSpeech[] AtomicPartsOfSpeech =
    [
        PartOfSpeech.Noun,
        PartOfSpeech.VerbTransitive,
        PartOfSpeech.VerbIntransitive,
        PartOfSpeech.Adjective,
        PartOfSpeech.Adverb,
        PartOfSpeech.Preposition,
        PartOfSpeech.Interjection
    ];

    private readonly FrozenDictionary<(PartOfSpeech Pos, CommonalityLevel Commonality), string[]> _buckets;
    private readonly FrozenDictionary<PartOfSpeech, string[]> _posBuckets;
    private readonly string[] _allWords;

    public int Count => _allWords.Length;

    public IndexedLexicon(IEnumerable<WordEntry> entries)
    {
        var rawList = entries.ToList();
        _allWords = rawList.Select(e => e.Text).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

        var tempBuckets = new Dictionary<(PartOfSpeech, CommonalityLevel), List<string>>();
        var tempPosBuckets = new Dictionary<PartOfSpeech, List<string>>();

        foreach (var pos in AtomicPartsOfSpeech)
        {
            tempPosBuckets[pos] = [];
            for (byte c = 1; c <= 5; c++)
            {
                tempBuckets[(pos, (CommonalityLevel)c)] = [];
            }
        }

        foreach (var entry in rawList)
        {
            foreach (var pos in AtomicPartsOfSpeech)
            {
                if ((entry.Pos & pos) == 0) continue;
                tempPosBuckets[pos].Add(entry.Text);

                if (entry.Commonality != CommonalityLevel.Any)
                {
                    tempBuckets[(pos, entry.Commonality)].Add(entry.Text);
                }
            }
        }

        _buckets = tempBuckets.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
        );

        _posBuckets = tempPosBuckets.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
        );
    }

    public string? GetRandomWord(PartOfSpeech pos = PartOfSpeech.Any, CommonalityLevel commonality = CommonalityLevel.Any)
    {
        return GetRandomWord(new WordFilter(pos, commonality));
    }

    public string? GetRandomWord(WordFilter filter)
    {
        if (_allWords.Length == 0) return null;

        var pool = ResolveCandidatePool(filter.Pos, filter.Commonality);
        if (pool.Length == 0) return null;

        if (string.IsNullOrEmpty(filter.StartsWith) && filter.MinLength is null && filter.MaxLength is null)
        {
            return pool[Random.Shared.Next(pool.Length)];
        }

        var matches = pool.Where(w =>
            (string.IsNullOrEmpty(filter.StartsWith) || w.StartsWith(filter.StartsWith, StringComparison.OrdinalIgnoreCase)) &&
            (filter.MinLength is null || w.Length >= filter.MinLength.Value) &&
            (filter.MaxLength is null || w.Length <= filter.MaxLength.Value)
        ).ToArray();

        return matches.Length > 0 ? matches[Random.Shared.Next(matches.Length)] : null;
    }

    private string[] ResolveCandidatePool(PartOfSpeech pos, CommonalityLevel commonality)
    {
        if (pos is PartOfSpeech.Any or PartOfSpeech.None && commonality is CommonalityLevel.Any)
        {
            return _allWords;
        }

        if (pos is not (PartOfSpeech.Any or PartOfSpeech.None) && commonality is not CommonalityLevel.Any)
        {
            var matchingBuckets = AtomicPartsOfSpeech
                .Where(p => (pos & p) != 0)
                .Select(p => _buckets.GetValueOrDefault((p, commonality)))
                .Where(arr => arr is { Length: > 0 })
                .ToArray();

            return matchingBuckets.Length switch
            {
                0 => [],
                1 => matchingBuckets[0]!,
                _ => matchingBuckets.SelectMany(x => x!).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
            };
        }

        if (pos is not (PartOfSpeech.Any or PartOfSpeech.None))
        {
            var matchingPos = AtomicPartsOfSpeech
                .Where(p => (pos & p) != 0)
                .Select(p => _posBuckets.GetValueOrDefault(p))
                .Where(arr => arr is { Length: > 0 })
                .ToArray();

            return matchingPos.Length switch
            {
                0 => [],
                1 => matchingPos[0]!,
                _ => matchingPos.SelectMany(x => x!).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
            };
        }

        var matchingTier = AtomicPartsOfSpeech
            .Select(p => _buckets.GetValueOrDefault((p, commonality)))
            .Where(arr => arr is { Length: > 0 })
            .ToArray();

        return matchingTier.Length switch
        {
            0 => [],
            1 => matchingTier[0]!,
            _ => matchingTier.SelectMany(x => x!).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
        };
    }

    public string GeneratePhrase(IReadOnlyList<PhraseSlot> slots)
    {
        if (slots.Count == 0) return string.Empty;

        var words = new string[slots.Count];
        for (var i = 0; i < slots.Count; i++)
        {
            words[i] = GetRandomWord(slots[i].Pos, slots[i].Commonality) ?? string.Empty;
        }

        return string.Join(" ", words);
    }
}