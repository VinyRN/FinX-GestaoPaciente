using FinX.Core.Dto.Response.Exame;
using FinX.Core.Interface.Service;
namespace FinX.Service;

public class ExamesExternosSimuladosService : IExamesExternosService
{
    public ExamesExternosResponseDto ConsultarPorCpf(string cpf)
    {
        var types = new[] { "Hemograma", "Colesterol", "Glicemia de Jejum", "Ureia/Creatinina", "TSH", "PCR", "Raio-X Torácico", "USG Abdominal", "Ressonância Magnética" };
        var labs = new[] { "LabVida", "Unilabs", "LabSaude", "InstitutoDiagnostico" };
        using var sha = System.Security.Cryptography.SHA256.Create();
        var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(cpf ?? string.Empty));
        var seed = System.BitConverter.ToInt32(hash, 0);
        var rng = new System.Random(seed);
        var count = rng.Next(1, 4);
        var exams = new List<ExameExternoResponseDto>();
        for (int i = 0; i < count; i++)
        {
            var type = types[rng.Next(types.Length)];
            var provider = labs[rng.Next(labs.Length)];
            var date = System.DateTime.UtcNow.AddDays(-rng.Next(1, 365)).ToString("yyyy-MM-dd");

            var examSeed = $"{cpf}-{i}";
            var examHash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(examSeed));
            var guidBytes = new byte[16];
            Array.Copy(examHash, guidBytes, 16);
            var examId = new System.Guid(guidBytes);

            ExameResultadoResponseDto result;
            if (type == "Hemograma")
            {
                var hb = Math.Round(11 + rng.NextDouble() * 6, 1);
                result = new ExameResultadoResponseDto { Summary = hb < 12 ? "Baixa hemoglobina" : "Dentro do esperado", Values = new { Hemoglobina = hb, Leucocitos = rng.Next(4000, 11000) } };
            }
            else if (type == "Colesterol")
            {
                var total = rng.Next(150, 280);
                result = new ExameResultadoResponseDto { Summary = total >= 240 ? "Alto" : "Normal", Values = new { Total = total, HDL = rng.Next(35, 70), LDL = rng.Next(80, 190) } };
            }
            else if (type == "Glicemia de Jejum")
            {
                var g = Math.Round(70 + rng.NextDouble() * 100, 1);
                result = new ExameResultadoResponseDto { Summary = g >= 126 ? "Diabetes" : g >= 100 ? "Pré-diabetes" : "Normal", Values = new { Jejum = g } };
            }
            else if (type == "Raio-X Torácico" || type == "USG Abdominal" || type == "Ressonância Magnética")
            {
                var findings = new[] { "Sem achados relevantes", "Opacidade focal discreta", "Alteração sugestiva de atelectasia", "Cisto simples" };
                result = new ExameResultadoResponseDto { Summary = findings[rng.Next(findings.Length)] ?? string.Empty, ReportUrl = null };
            }
            else
            {
                result = new ExameResultadoResponseDto { Summary = "Normal", Values = Array.Empty<object>() };
            }

            exams.Add(new ExameExternoResponseDto { Id = examId, Provider = provider, Date = date, Type = type, Result = result });
        }

        return new ExamesExternosResponseDto { Cpf = cpf ?? string.Empty, Exams = exams };
    }
}
