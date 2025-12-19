

using System.ComponentModel;

namespace App.Domain.Enums
{
    public enum Authenticate
    {
    }
    public enum OffenseLevel
    {
        [Description("You are banned for 1 hour.")]
        FirstOffense = 1,

        [Description("You are banned for 24 hours.")]
        SecondOffense = 2,

        [Description("You are banned for 1 week. Please contact the admin.")]
        ThirdOffense = 3,

        [Description("You are permanently banned. Please contact the admin.")]
        FourthOffense = 4
    }
}
