namespace FinX.Scripts
{
    /// Script para criar dados de teste com pacientes duplicados
    /// Útil para demonstrar e testar o processo de unificação
    public class TestDataResult
    {
        public int HospitalsCreated { get; set; }
        public int PatientsCreated { get; set; }
        public int GroupsCreated { get; set; }
        public int AppointmentsCreated { get; set; }
        public List<string> Errors { get; set; } = new();
    }

}
