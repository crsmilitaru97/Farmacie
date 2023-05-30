namespace Farmacia_InfoWorld
{
    public class Medicament
    {
        public bool Selected { get; set; }

        public int ID { get; set; }

        public string Denumire { get; set; } = string.Empty;

        public int Gramaj { get; set; }

        public string Forma { get; set; } = string.Empty;

        public string? Descriere { get; set; }

        public int Cantitate { get; set; }

    }

    public class ComandaMedicament
    {
        public int ID { get; set; }

        public string ID_Comanda { get; set; } = string.Empty;

        public string ID_Medicament { get; set; } = string.Empty;

        public int Cantitate { get; set; }
    }
}