namespace Calcular.CoreApi.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
    }
}
