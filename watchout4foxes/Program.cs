using watchout4foxes.Core.Lexicon;
using watchout4foxes.Core.Models;
using watchout4foxes.Core;

var builder = WebApplication.CreateBuilder(args);

const string tsvPath = "lexicon.tsv";

if (!File.Exists(tsvPath))
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("[watchout4foxes] Production lexicon not found! Automatically compiling clean dictionary...");
    Console.ResetColor();

    var progress = new Progress<string>(msg => Console.WriteLine($"  -> {msg}"));
    await LexiconSync.GenerateLexiconTsvAsync(tsvPath, progress);
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"[watchout4foxes] Loading dictionary from '{tsvPath}'...");
Console.ResetColor();

var lexicon = LexiconLoader.LoadFromFile(tsvPath);

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"[watchout4foxes] Successfully loaded {lexicon.Count:N0} pristine words into indexed memory!\n");
Console.ResetColor();

var sentenceGen = new SentenceGenerator(lexicon);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
    }
});

app.MapPost("/Random/RandomPhrase", async context =>
{
    var form = await context.Request.ReadFormAsync();
    var slots = new List<PhraseSlot>();

    for (var i = 1; i <= 4; i++)
    {
        var posStr = form[$"Pos{i}"].ToString();
        var levelStr = form[$"Level{i}"].ToString();

        if (string.IsNullOrWhiteSpace(posStr)) continue;

        slots.Add(new PhraseSlot(
            WireMapper.ParsePos(posStr),
            WireMapper.ParseLevel(levelStr)
        ));
    }

    if (slots.Count == 0)
    {
        slots.Add(new PhraseSlot(PartOfSpeech.Adjective, CommonalityLevel.SomewhatCommon));
        slots.Add(new PhraseSlot(PartOfSpeech.Noun, CommonalityLevel.SomewhatCommon));
    }

    var result = lexicon.GeneratePhrase(slots);
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync(result);
});

app.MapPost("/Random/RandomWordPlus", async context =>
{
    var form = await context.Request.ReadFormAsync();
    var filter = new WordFilter(
        Pos: WireMapper.ParsePos(form["Pos"].ToString()),
        Commonality: WireMapper.ParseLevel(form["Level"].ToString())
    );

    var word = lexicon.GetRandomWord(filter) ?? "fox";
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync(word);
});

app.MapPost("/Random/RandomWord", async context =>
{
    var word = lexicon.GetRandomWord() ?? "fox";
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync(word);
});

app.MapPost("/Random/NewRandomSentence", async context =>
{
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync(sentenceGen.GenerateSentence());
});

app.MapPost("/Random/RandomSentence", async context =>
{
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync(sentenceGen.GenerateSentence());
});

app.MapPost("/Random/RandomParagraph", async context =>
{
    var form = await context.Request.ReadFormAsync();
    var s1 = form["Subject1"].ToString();
    var s2 = form["Subject2"].ToString();
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync(sentenceGen.GenerateParagraph(s1, s2));
});

app.MapPost("/Random/NewRandomParagraph", async context =>
{
    var form = await context.Request.ReadFormAsync();
    var s1 = form["Subject1"].ToString();
    var s2 = form["Subject2"].ToString();
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync(sentenceGen.GenerateParagraph(s1, s2));
});

app.MapGet("/", () => Results.Redirect("/hello.html"));
app.MapGet("/Random/RandomWord", () => Results.Redirect("/word.html"));
app.MapGet("/Random/RandomPhrase", () => Results.Redirect("/phrase.html"));
app.MapGet("/Random/RandomWordPlus", () => Results.Redirect("/wordplus.html"));
app.MapGet("/Random/RandomSentence", () => Results.Redirect("/sentence.html"));
app.MapGet("/Random/RandomParagraph", () => Results.Redirect("/paragraph.html"));

Console.WriteLine("Server ready! Listening on configured endpoints.");
app.Run();