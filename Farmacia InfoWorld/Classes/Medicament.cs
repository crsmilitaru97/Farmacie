namespace Farmacia_InfoWorld.Classes
{
    public class Medicament
    {
        public bool Selected { get; set; }

        public int ID { get; set; }

        public string Denumire { get; set; } = string.Empty;

        public string Forma { get; set; } = string.Empty;

        public string? Descriere { get; set; }

        public decimal Pret { get; set; }

        public List<Lot> Loturi { get; set; } = new List<Lot>();
    }

    public class Lot
    {
        public int ID { get; set; }

        public DateTime Data_Expirare { get; set; }

        public int Cantitate { get; set; }

        public int ID_Medicament { get; set; }
    }
}