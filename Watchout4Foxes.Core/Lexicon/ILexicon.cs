using watchout4foxes.Core.Models;

namespace watchout4foxes.Core.Lexicon;

public interface ILexicon
{
    int Count { get; }
    string? GetRandomWord(PartOfSpeech pos = PartOfSpeech.Any, CommonalityLevel commonality = CommonalityLevel.Any);
    string? GetRandomWord(WordFilter filter);
    string GeneratePhrase(IReadOnlyList<PhraseSlot> slots);
}