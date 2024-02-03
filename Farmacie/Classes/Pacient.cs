namespace Farmacie.Classes
{
    public class Pacient
    {
        public int ID { get; set; } = 0;

        public string Nume { get; set; } = string.Empty;

        public string Prenume { get; set; } = string.Empty;

        public string CNP { get; set; } = string.Empty;

        public DateTime Data_Nastere { get; set; }

        public string? Telefon { get; set; }

        public string? Email { get; set; }

        public List<Adresa> Adrese { get; set; } = new List<Adresa>();
    }

    public class Adresa
    {
        public int ID { get; set; }

        public int Tip_Adresa { get; set; }

        public string Linie_Adresa { get; set; } = string.Empty;

        public string Localitate { get; set; } = string.Empty;

        public string Judet { get; set; } = string.Empty;

        public string? Cod_Postal { get; set; }

        public int? ID_Pacient { get; set; }
    }
}