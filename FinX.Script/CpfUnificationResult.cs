namespace FinX.Scripts
{
    public class CpfUnificationResult
    {
        public string CPF { get; set; } = string.Empty;
        public int PatientsRemoved { get; set; }
        public int GroupsUpdated { get; set; }
        public int AppointmentsUpdated { get; set; }
        public int MedicalRecordsUpdated { get; set; }
        public List<string> Errors { get; set; } = new();
    }

}
