using TaskManagement.API.Models;

namespace TaskManagement.API.Repository.Specifications
{
    public class RefreshTokenSpecifications
        : BaseSpecification<RefreshToken>
    {
        public RefreshTokenSpecifications(string token)
        {
            AddCriteria(x => x.Token == token);
        }
    }
}
