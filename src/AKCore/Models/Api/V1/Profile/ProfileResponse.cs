using System.Collections.Generic;

namespace AKCore.Models.Api.V1.Profile;

public class ProfileResponse
{
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Instrument { get; set; } = "";

    public IList<string> OtherInstruments { get; set; } = [];
    public IList<string> Roles { get; set; } = [];
    public IList<string> Posts { get; set; } = [];

    public string Medal { get; set; } = "";
    public string GivenMedal { get; set; } = "";
}
