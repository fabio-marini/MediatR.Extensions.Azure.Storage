using Microsoft.Azure.Cosmos.Table;
using System;

namespace ClassLibrary1
{
    public class CustomerActivityEntity : TableEntity
    {
        public string Email { get; set; }
        public DateTime? CustomerReceivedOn { get; set; }
        public bool? IsValid { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? CustomerPublishedOn { get; set; }
    }
}
