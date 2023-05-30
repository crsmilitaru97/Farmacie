namespace Farmacia_InfoWorld
{
    public class Pacient
    {
        public int ID { get; set; }

        public string Nume { get; set; } = string.Empty;

        public string Prenume { get; set; } = string.Empty;

        public string CNP { get; set; } = string.Empty;

        public DateTime Data_Nastere { get; set; }

        public string? Telefon { get; set; }

        public string? Email { get; set; }
    }
}