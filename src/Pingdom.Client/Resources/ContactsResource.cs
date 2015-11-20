using System.Collections;
using System.Collections.Generic;
using PingdomClient.Contracts;
using PingdomClient.Extensions;

namespace PingdomClient.Resources
{
    using System.Threading.Tasks;

    public sealed class ContactsResource : Resource
    {
        internal ContactsResource() { }

        /// <summary>
        /// Returns a list of all contacts.
        /// </summary>
        /// <returns></returns>
        public Task<GetContactsListResponse> GetContactsList()
        {
            return Client.GetAsync<GetContactsListResponse>("notification_contacts/");
        }

        /// <summary>
        /// Create a new contact.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Task<CreateNewContactResponse> CreateNewContact(object contact)
        {
            return Client.PostAsync<CreateNewContactResponse>("notification_contacts/", contact);
        }

        /// <summary>
        /// Modify a contact.
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Task<PingdomResponse> ModifyContact(int contactId, object contact)
        {
            var apiMethod = string.Format("notification_contacts/{0}", contactId);
            return Client.PutAsync<PingdomResponse>(apiMethod, contact);
        }

        /// <summary>
        /// Modifies a list of contacts.
        /// </summary>
        /// <param name="contactIds">Comma-separated list of identifiers for contacts to be modified.</param>
        /// <param name="paused">Pause contacts</param>
        /// <returns></returns>
        public Task<string> ModifyMultipleContacts(IEnumerable<int> contactIds, bool paused)
        {
            var requestBody = string.Format("contactids={0}&paused={1}", string.Join(",", contactIds), paused);
            return Client.PutAsync<string>("notification_contacts/", requestBody);
        }

        /// <summary>
        /// Deletes a list of contacts.
        /// </summary>
        /// <param name="contactIds"></param>
        /// <returns></returns>
        public Task<PingdomResponse> DeleteMultipleContacts(object contactIds)
        {
            return Client.DeleteAsync<PingdomResponse>("notification_contacts/", contactIds);
        }

        /// <summary>
        /// Deletes a contact.
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public Task<PingdomResponse> DeleteContact(int contactId)
        {
            return Client.DeleteAsync<PingdomResponse>(string.Format("notification_contacts/{0}", contactId));
        }

    }
}