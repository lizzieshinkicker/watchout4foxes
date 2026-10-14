using watchout4foxes.Core.Models;

namespace watchout4foxes.Core.Lexicon;

public sealed class SentenceGenerator(ILexicon lexicon)
{
    private static readonly string[] Prepositions =
    [
        "beside", "within", "outside", "on top of", "across", "throughout",
        "beneath", "underneath", "opposite", "near", "past", "after", "before", "behind"
    ];

    private static readonly string[] Possessives = ["your", "his", "her", "its", "my", "our"];
    private static readonly string[] Modals = ["Why can't", "Why won't", "Will", "Can", "How will", "Should", "Does"];

    private static string GetDeterminer(string? followingWord, bool capitalize)
    {
        var roll = Random.Shared.NextDouble();
        string det;

        if (roll < 0.50)
            det = "the";
        else if (roll < 0.75)
        {
            var firstChar = !string.IsNullOrWhiteSpace(followingWord) ? char.ToLowerInvariant(followingWord.Trim()[0]) : 'c';
            det = "aeiou".Contains(firstChar) ? "an" : "a";
        }
        else if (roll < 0.90)
            det = Possessives[Random.Shared.Next(Possessives.Length)];
        else
        {
            string[] rare = ["every", "another", "each", "this"];
            det = rare[Random.Shared.Next(rare.Length)];
        }

        return capitalize ? char.ToUpperInvariant(det[0]) + det[1..] : det;
    }

    public string GenerateSentence(string? subject = null, string? otherSubject = null, int? archetype = null)
    {
        var noun1 = lexicon.GetRandomWord(PartOfSpeech.Noun, CommonalityLevel.Common) ?? "fox";
        var noun2 = lexicon.GetRandomWord(PartOfSpeech.Noun, CommonalityLevel.Common) ?? "world";
        var adj1 = lexicon.GetRandomWord(PartOfSpeech.Adjective, CommonalityLevel.Common) ?? "silent";
        var adj2 = lexicon.GetRandomWord(PartOfSpeech.Adjective, CommonalityLevel.Common) ?? "luminous";

        var verbT_raw = lexicon.GetRandomWord(PartOfSpeech.VerbTransitive, CommonalityLevel.Common) ?? "mend";
        var verbI_raw = lexicon.GetRandomWord(PartOfSpeech.VerbIntransitive, CommonalityLevel.Common) ?? "sleep";
        var verbT = ConjugateThirdPerson(verbT_raw);
        var verbI = ConjugateThirdPerson(verbI_raw);
        var prep = Prepositions[Random.Shared.Next(Prepositions.Length)];

        var s1 = subject ?? $"{GetDeterminer(noun1, true)} {noun1}";
        var obj = otherSubject ?? $"{GetDeterminer(adj2, false)} {adj2} {noun2}";
        var prepPhrase = $"{prep} {GetDeterminer(adj1, false)} {adj1} {noun1}";

        var type = archetype ?? Random.Shared.Next(5);

        return type switch
        {
            0 => $"{s1} {verbT} {obj} {prepPhrase}.",
            1 => $"{s1} {verbI} {prepPhrase}.",
            2 => $"{Capitalize(prepPhrase)} {verbI} {s1}.",
            3 => $"{Modals[Random.Shared.Next(Modals.Length)]} {s1} {(Random.Shared.NextDouble() < 0.5 ? $"{verbT_raw} {obj}" : $"{verbI_raw} {prepPhrase}")}?",
            _ => $"{s1} {verbI}{(Random.Shared.NextDouble() < 0.5 ? "!" : ".")}"
        };
    }

    public string GenerateParagraph(string? subject1 = null, string? subject2 = null)
    {
        var s1 = string.IsNullOrWhiteSpace(subject1) ? null : Capitalize(subject1.Trim());
        var s2 = string.IsNullOrWhiteSpace(subject2) ? null : Capitalize(subject2.Trim());

        var count = Random.Shared.Next(5, 7);
        var sentences = new List<string>(count);
        int[] archetypes = [0, 1, 2, 3, 4, 0];

        for (var i = 0; i < count; i++)
        {
            var current = (i % 2 == 0) ? s1 : s2;
            var other = (i % 2 == 0) ? s2 : s1;
            sentences.Add(GenerateSentence(current, other, archetypes[i % archetypes.Length]));
        }

        return string.Join(" ", sentences);
    }

    private static string ConjugateThirdPerson(string verb)
    {
        if (verb.EndsWith("ing") || verb.EndsWith("ed")) return verb;
        if (verb.EndsWith("ss") || verb.EndsWith("sh") || verb.EndsWith("ch") || verb.EndsWith("x") || verb.EndsWith("z"))
            return verb + "es";
        if (verb.EndsWith('y') && verb.Length > 1 && !"aeiou".Contains(verb[^2]))
            return verb[..^1] + "ies";
        return verb + "s";
    }

    private static string Capitalize(string text) =>
        string.IsNullOrEmpty(text) ? text : char.ToUpperInvariant(text[0]) + text[1..];
}