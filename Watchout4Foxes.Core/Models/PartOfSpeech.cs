namespace watchout4foxes.Core.Models;

[Flags]
public enum PartOfSpeech : ushort
{
    None = 0,
    Noun = 1 << 0,
    VerbTransitive = 1 << 1,
    VerbIntransitive = 1 << 2,
    Adjective = 1 << 3,
    Adverb = 1 << 4,
    Preposition = 1 << 5,
    Interjection = 1 << 6,
    
    Verb = VerbTransitive | VerbIntransitive,
    
    Any = Noun | VerbTransitive | VerbIntransitive | Adjective | Adverb | Preposition | Interjection
}