namespace Farmacie.Classes
{
    public class Utilizator
    {
        public int ID { get; set; }

        public string? Email { get; set; } = string.Empty;

        public string? Parola { get; set; } = string.Empty;

        public string? Tip { get; set; } = string.Empty;

        public int ID_Pacient { get; set; }
    }
}