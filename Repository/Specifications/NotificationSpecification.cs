using TaskManagement.API.Models;

namespace TaskManagement.API.Repository.Specifications
{
    public class NotificationSpecification
        : BaseSpecification<Notification>
    {
        public NotificationSpecification(string userId,int notificationId) 
        {
            AddCriteria(x=>x.Id == notificationId && x.ToUserId==userId);
        }
        public NotificationSpecification(string userId)
        {
            AddCriteria(x => x.ToUserId == userId);
            AddOrderByDescending(x => x.IsRead);
            AddOrderByDescending(x => x.CreatedAt);
        }
        public NotificationSpecification(string userId, bool isRead)
        {
            AddCriteria(x => x.ToUserId == userId && x.IsRead == isRead);
            AddOrderByDescending(x => x.CreatedAt);
        }
        public void AddPagination(int pageIndex, int PageSize)
        {
            ApplyPagination(pageIndex, PageSize);
        }
    }
}
