namespace FinX.Scripts
{
    /// Script principal para unificação de pacientes duplicados
    public class UnificationResult
    {
        public int TotalDuplicateCpfs { get; set; }
        public int TotalPatientsRemoved { get; set; }
        public int TotalGroupsUpdated { get; set; }
        public int TotalAppointmentsUpdated { get; set; }
        public int TotalMedicalRecordsUpdated { get; set; }
        public List<string> ProcessedCpfs { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }

}
