namespace Farmacia_InfoWorld
{
    public class Comanda
    {
        public int ID { get; set; }

        public string NumePacient { get; set; } = string.Empty;

        public List<Medicament> Medicamente { get; set; } = new List<Medicament>();

        public int Cantitate { get; set; }

        public string? Status { get; set; }
    }

    public class ComandaMedicament
    {
        public int ID { get; set; }

        public int ID_Comanda { get; set; }

        public int ID_Medicament { get; set; }

        public int Cantitate { get; set; }
    }
}