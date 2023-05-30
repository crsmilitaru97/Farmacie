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
}