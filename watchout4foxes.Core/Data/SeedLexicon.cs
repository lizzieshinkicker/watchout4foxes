using watchout4foxes.Core.Models;

namespace watchout4foxes.Core.Data;

public static class SeedLexicon
{
    public static IReadOnlyList<WordEntry> GetDefaultEntries() =>
    [
        new("water", PartOfSpeech.Noun, CommonalityLevel.VeryCommon),
        new("person", PartOfSpeech.Noun, CommonalityLevel.VeryCommon),
        new("house", PartOfSpeech.Noun, CommonalityLevel.VeryCommon),
        new("forest", PartOfSpeech.Noun, CommonalityLevel.VeryCommon),
        new("fox", PartOfSpeech.Noun, CommonalityLevel.VeryCommon),

        new("engine", PartOfSpeech.Noun, CommonalityLevel.Common),
        new("palace", PartOfSpeech.Noun, CommonalityLevel.Common),
        new("shadow", PartOfSpeech.Noun, CommonalityLevel.Common),
        new("feather", PartOfSpeech.Noun, CommonalityLevel.Common),
        new("compass", PartOfSpeech.Noun, CommonalityLevel.Common),

        new("labyrinth", PartOfSpeech.Noun, CommonalityLevel.SomewhatCommon),
        new("citadel", PartOfSpeech.Noun, CommonalityLevel.SomewhatCommon),
        new("silhouette", PartOfSpeech.Noun, CommonalityLevel.SomewhatCommon),
        new("constellation", PartOfSpeech.Noun, CommonalityLevel.SomewhatCommon),
        new("talisman", PartOfSpeech.Noun, CommonalityLevel.SomewhatCommon),

        new("sepulcher", PartOfSpeech.Noun, CommonalityLevel.Uncommon),
        new("effigy", PartOfSpeech.Noun, CommonalityLevel.Uncommon),
        new("phantasm", PartOfSpeech.Noun, CommonalityLevel.Uncommon),
        new("automaton", PartOfSpeech.Noun, CommonalityLevel.Uncommon),
        new("cartographer", PartOfSpeech.Noun, CommonalityLevel.Uncommon),

        new("chimerism", PartOfSpeech.Noun, CommonalityLevel.Obscure),
        new("tesseract", PartOfSpeech.Noun, CommonalityLevel.Obscure),
        new("tenebrism", PartOfSpeech.Noun, CommonalityLevel.Obscure),
        new("petrichor", PartOfSpeech.Noun, CommonalityLevel.Obscure),
        new("apophenia", PartOfSpeech.Noun, CommonalityLevel.Obscure),

        new("make", PartOfSpeech.VerbTransitive, CommonalityLevel.VeryCommon),
        new("find", PartOfSpeech.VerbTransitive, CommonalityLevel.VeryCommon),
        new("build", PartOfSpeech.VerbTransitive, CommonalityLevel.VeryCommon),
        new("watch", PartOfSpeech.VerbTransitive, CommonalityLevel.VeryCommon),

        new("uncover", PartOfSpeech.VerbTransitive, CommonalityLevel.Common),
        new("shatter", PartOfSpeech.VerbTransitive, CommonalityLevel.Common),
        new("capture", PartOfSpeech.VerbTransitive, CommonalityLevel.Common),
        new("dissolve", PartOfSpeech.VerbTransitive, CommonalityLevel.Common),

        new("dismantle", PartOfSpeech.VerbTransitive, CommonalityLevel.SomewhatCommon),
        new("obliterate", PartOfSpeech.VerbTransitive, CommonalityLevel.SomewhatCommon),
        new("transmute", PartOfSpeech.VerbTransitive, CommonalityLevel.SomewhatCommon),
        new("illuminate", PartOfSpeech.VerbTransitive, CommonalityLevel.SomewhatCommon),

        new("excoriate", PartOfSpeech.VerbTransitive, CommonalityLevel.Uncommon),
        new("coruscate", PartOfSpeech.VerbTransitive, CommonalityLevel.Uncommon),
        new("adumbrate", PartOfSpeech.VerbTransitive, CommonalityLevel.Uncommon),
        new("eviscerate", PartOfSpeech.VerbTransitive, CommonalityLevel.Uncommon),

        new("defenestrate", PartOfSpeech.VerbTransitive, CommonalityLevel.Obscure),
        new("disembogue", PartOfSpeech.VerbTransitive, CommonalityLevel.Obscure),
        new("obnubilate", PartOfSpeech.VerbTransitive, CommonalityLevel.Obscure),

        new("run", PartOfSpeech.VerbIntransitive, CommonalityLevel.VeryCommon),
        new("sleep", PartOfSpeech.VerbIntransitive, CommonalityLevel.VeryCommon),
        new("wander", PartOfSpeech.VerbIntransitive, CommonalityLevel.Common),
        new("slumber", PartOfSpeech.VerbIntransitive, CommonalityLevel.Common),

        new("meander", PartOfSpeech.VerbIntransitive, CommonalityLevel.SomewhatCommon),
        new("languish", PartOfSpeech.VerbIntransitive, CommonalityLevel.SomewhatCommon),
        new("abscond", PartOfSpeech.VerbIntransitive, CommonalityLevel.Uncommon),
        new("reverberate", PartOfSpeech.VerbIntransitive, CommonalityLevel.Uncommon),

        new("somnambulate", PartOfSpeech.VerbIntransitive, CommonalityLevel.Obscure),
        new("deliquesce", PartOfSpeech.VerbIntransitive, CommonalityLevel.Obscure),

        new("bright", PartOfSpeech.Adjective, CommonalityLevel.VeryCommon),
        new("silent", PartOfSpeech.Adjective, CommonalityLevel.Common),
        new("luminous", PartOfSpeech.Adjective, CommonalityLevel.SomewhatCommon),
        new("spectral", PartOfSpeech.Adjective, CommonalityLevel.Uncommon),
        new("iridescent", PartOfSpeech.Adjective, CommonalityLevel.Uncommon),
        new("tenebrous", PartOfSpeech.Adjective, CommonalityLevel.Obscure),
        new("sempiternal", PartOfSpeech.Adjective, CommonalityLevel.Obscure),

        new("quickly", PartOfSpeech.Adverb, CommonalityLevel.VeryCommon),
        new("gracefully", PartOfSpeech.Adverb, CommonalityLevel.Common),
        new("relentlessly", PartOfSpeech.Adverb, CommonalityLevel.SomewhatCommon),
        new("surreptitiously", PartOfSpeech.Adverb, CommonalityLevel.Uncommon),
        new("mercurially", PartOfSpeech.Adverb, CommonalityLevel.Obscure),

        new("above", PartOfSpeech.Preposition, CommonalityLevel.VeryCommon),
        new("beneath", PartOfSpeech.Preposition, CommonalityLevel.Common),
        new("amidst", PartOfSpeech.Preposition, CommonalityLevel.SomewhatCommon),
        new("athwart", PartOfSpeech.Preposition, CommonalityLevel.Obscure),

        new("behold", PartOfSpeech.Interjection, CommonalityLevel.Common),
        new("alas", PartOfSpeech.Interjection, CommonalityLevel.SomewhatCommon),
        new("lo", PartOfSpeech.Interjection, CommonalityLevel.Uncommon)
    ];
}