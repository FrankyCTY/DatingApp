namespace API.Entities
{
  public class AppUser
  {
    public int Id { get; set; } // Recognized by Entity Framework as Primary Key
    public string UserName { get; set; } // Carmel Case because if ASP.net Identity naming convention 
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
  }
}