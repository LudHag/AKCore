using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AKCore.Models.Api.V1.Profile;

public class UpdateProfileRequest
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    [EmailAddress]
    public string Email { get; set; } = "";

    public string Phone { get; set; } = "";
    public string Instrument { get; set; } = "";
    public IList<string> OtherInstruments { get; set; } = [];
}
