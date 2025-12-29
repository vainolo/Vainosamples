Random r = Random.Shared;
var ctxProvider = new ContextProvider<Context>();

async Task InitializeAndPrintContextAsync(int id)
{
    var ctx = ctxProvider.InitContext();
    var guid = Guid.NewGuid();
    Console.WriteLine($"Init context for id: {id} - {guid}.");
    ctx.id = guid;
    await Task.Delay(r.Next(1000));
    ctx = ctxProvider.GetContext();
    Console.WriteLine($"Current context for id: {id} - {ctx.id}.");
    await Level1(id);
}

async Task Level1(int id)
{
    await Task.Delay(r.Next(1000));
    var ctx = ctxProvider.GetContext();
    ctx.name = $"MyName{id}";
    Console.WriteLine($"Level1 - Current context for id: {id} - {ctx.id}.");
    await Level2(id);
}

async Task Level2(int id)
{
    await Task.Delay(r.Next(1000));
    var ctx = ctxProvider.GetContext();
    Console.WriteLine($"Level2 - Current context for id: {id} - {ctx.id}, name {ctx.name}.");
}

var task1 = InitializeAndPrintContextAsync(1);
var task2 = InitializeAndPrintContextAsync(2);
var task3 = InitializeAndPrintContextAsync(3);

Console.Read();

await Task.WhenAll(task1, task2, task3);

public class Context
{
    public Guid id { get; set; }
    public string name { get; set; }
}
