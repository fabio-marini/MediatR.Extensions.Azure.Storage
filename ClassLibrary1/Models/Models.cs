using System;

namespace ClassLibrary1
{
    public class SourceCustomer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class CanonicalCustomer
    {
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    public class TargetCustomer
    {
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
    }
}
