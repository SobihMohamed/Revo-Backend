namespace Revo.API.Requests.ContactRequest
{
    public record CreateContactRequestApiRequest(
            string Name,
            string PhoneNumber,
            string Message,
            Guid? ServiceId
        );
}
