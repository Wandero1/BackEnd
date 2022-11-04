namespace WanderoBackEnd.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        //Amikor a Felhasznalo regisztral, letrehozunk egy ilyen tokent
        //Ez egy veletlenszeru karakter sorozat
        //A felhasznalo, el kell erjen egy adott vegpontot(az oldalon)
        //Ahol ezt a tokent hasznalja majd, es ahol leellenorizhetjuk,
        //Hogy letezik olyan felhasznalo amelyhez ez a token van rendelve
        //Ebbol all a regisztracio megerositese
        public string? VerificationToken { get; set; }
        //Elemnetjuk a datumot es az idot amikor a felhasznalo az adott vegpontot
        //eleri, azert, hogy biztosan tudjuk, hogy a megerosites lezajlott
        public DateTime? VerifiedAt { get; set; }
        //A jelszovisszaallito token tulajdonkeppen ugyanugy mukodik mint a
        //megerosito token
        public string? PasswordResetToken { get; set; }
        //Ebben az esetben a Date tipus a lejaratat tarolja a Jelszo visszaallito
        //tokennek (pl: csak 1 napig kellene elerheto legyen, biztonsagi okokbol)
        public DateTime? ResetTokenExpires { get; set; }
    }
}
