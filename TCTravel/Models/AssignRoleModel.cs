namespace TCTravel.Models;

public class AssignRoleModel
{
    public string UserId { get; set; }
    public string RoleName { get; set; }
    
    // TODO Need to add these Ids to link users so they an update their specific account
    // public int? ClientCompanyId { get; set; }
    //
    // public int? CustomerId { get; set; }
    //
    // public int? DriverId { get; set; }
}