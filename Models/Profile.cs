using System.Collections.Generic;

namespace Calcular.CoreApi.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<UserProfile> UserProfiles { get; set; }
    }
}
