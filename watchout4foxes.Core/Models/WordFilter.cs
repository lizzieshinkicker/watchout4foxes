namespace watchout4foxes.Core.Models;

public readonly record struct WordFilter(
    PartOfSpeech Pos = PartOfSpeech.Any,
    CommonalityLevel Commonality = CommonalityLevel.Any,
    string? StartsWith = null,
    int? MinLength = null,
    int? MaxLength = null
);