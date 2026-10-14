using watchout4foxes.Core.Models;

namespace watchout4foxes.Core.Lexicon;

public static class WireMapper
{
    public static PartOfSpeech ParsePos(string? code) => code?.ToLowerInvariant() switch
    {
        "n" or "noun" => PartOfSpeech.Noun,
        "a" or "adj" or "adjective" => PartOfSpeech.Adjective,
        "t" or "transitive" => PartOfSpeech.VerbTransitive,
        "i" or "intransitive" => PartOfSpeech.VerbIntransitive,
        "v" or "verb" => PartOfSpeech.Verb,
        "e" or "adv" or "adverb" => PartOfSpeech.Adverb,
        "z" or "interjection" => PartOfSpeech.Interjection,
        "s" or "prep" or "preposition" => PartOfSpeech.Preposition,
        _ => PartOfSpeech.Any
    };

    public static CommonalityLevel ParseLevel(string? code) => code switch
    {
        "10" => CommonalityLevel.VeryCommon,
        "20" => CommonalityLevel.Common,
        "35" => CommonalityLevel.SomewhatCommon,
        "50" or "60" => CommonalityLevel.Uncommon,
        "70" or "95" => CommonalityLevel.Obscure,
        _ => CommonalityLevel.Any
    };
}