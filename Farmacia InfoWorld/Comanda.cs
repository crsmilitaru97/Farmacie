namespace Farmacia_InfoWorld
{
    public class Comanda
    {
        public int ID { get; set; }

        public int? Status { get; set; }

        public string? NumePacient { get; set; }

        public decimal Pret { get; set; }

        public DateTime Data { get; set; }

        public int ID_Pacient { get; set; }

        public List<Medicament> Medicamente { get; set; } = new List<Medicament>();
    }

    public class ComandaMedicament
    {
        public int ID { get; set; }

        public int ID_Comanda { get; set; }

        public int ID_Medicament { get; set; }

        public int Cantitate { get; set; }
    }
}