namespace watchout4foxes.Core.Models;

public readonly record struct WordEntry(
    string Text,
    PartOfSpeech Pos,
    CommonalityLevel Commonality
);