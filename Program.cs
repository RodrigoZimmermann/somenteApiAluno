var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

var alunos = new List<Aluno>();
var alunoLogado = new List<AlunoLogado>();

app.MapGet("/alunos", () =>
{
    return Results.Ok(alunos);
});

app.MapGet("/alunos/{id}", (int id) =>
{
    var aluno = alunos.FirstOrDefault(a => a.Id == id);

    if (aluno == null)
    {
        return Results.NotFound("Aluno não encontrado.");
    }

    return Results.Ok(aluno);
});

app.MapPost("/alunos", (Aluno aluno) =>
{
    aluno.Id = alunos.Any() ? alunos.Max(a => a.Id) + 1 : 1;
    alunos.Add(aluno);
    return Results.Created($"/alunos/{aluno.Id}", aluno);
});

app.MapPut("/alunos/{id}", (int id, Aluno inputAluno) =>
{
    var aluno = alunos.FirstOrDefault(a => a.Id == id);

    if (aluno == null)
    {
        return Results.NotFound();
    }

    aluno.Nome = inputAluno.Nome;
    aluno.Email = inputAluno.Email;
    aluno.Senha = inputAluno.Senha;
    aluno.Turma = inputAluno.Turma;

    return Results.NoContent();
});

app.MapDelete("/alunos/{id}", (int id) =>
{
    var aluno = alunos.FirstOrDefault(a => a.Id == id);

    if (aluno == null)
    {
        return Results.NotFound();
    }

    alunos.Remove(aluno);
    return Results.Ok(aluno);
});

app.MapGet("/alunos/{email}/{senha}", (string email, string senha) =>
{
    var aluno = alunos.FirstOrDefault(a => a.Email == email && a.Senha == senha);

    if (aluno == null)
    {
        return Results.NotFound("Aluno não encontrado.");
    }

    return Results.Ok(aluno.Id);
});

app.MapPost("/alunos/logado", (AlunoLogado alunoLogadoRequest) =>
{
    alunoLogado.Clear();
    alunoLogado.Add(alunoLogadoRequest);
    return Results.Created($"/alunos/logado", alunoLogadoRequest);
});

app.MapGet("/alunos/logado", () =>
{
    var logado = alunoLogado.FirstOrDefault();
    if (logado == null)
    {
        return Results.NotFound(0);
    }
    return Results.Ok(logado.Id);
});

app.Run();

public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string Turma { get; set; }
}

public class AlunoLogado
{
    public int Id { get; set; }
}
