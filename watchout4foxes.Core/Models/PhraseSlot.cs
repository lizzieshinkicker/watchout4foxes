namespace watchout4foxes.Core.Models;

public readonly record struct PhraseSlot(
    PartOfSpeech Pos = PartOfSpeech.Any,
    CommonalityLevel Commonality = CommonalityLevel.Any
);