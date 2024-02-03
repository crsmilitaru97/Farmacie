namespace Farmacie.Classes
{
    public class Comanda
    {
        public int ID { get; set; }

        public int? Status { get; set; }

        public string? NumePacient { get; set; }

        public decimal Pret { get; set; }

        public DateTime Data { get; set; }

        public int ID_Pacient { get; set; }

        public List<ComandaMedicament> ComandaMedicamente { get; set; } = new List<ComandaMedicament>();
    }

    public class ComandaMedicament
    {
        public int ID { get; set; }

        public int ID_Comanda { get; set; }

        public int ID_Medicament { get; set; }

        public int Cantitate { get; set; }

        public int _status { get; set; }

        public Medicament? _medicament { get; set; }

        public static class Status
        {
            public const int nemodificat = 0;
            public const int modificat = 1;
            public const int nou = 2;
        }
    }
}