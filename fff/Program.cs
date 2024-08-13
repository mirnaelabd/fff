using System;
using System.Collections.Generic;

public enum LayOffCause
{
    VacationStockNegative,
    AgeAboveSixty,
    SalesTargetNotAchieved,
    Resignation
}

public class EmployeeLayOffEventArgs : EventArgs
{
    public LayOffCause Cause { get; set; }
}

public abstract class Employee
{
    public event EventHandler<EmployeeLayOffEventArgs> EmployeeLayOff;

    protected virtual void OnEmployeeLayOff(EmployeeLayOffEventArgs e)
    {
        EmployeeLayOff?.Invoke(this, e);
    }

    public int EmployeeID { get; set; }
    public DateTime BirthDate { get; set; }
    public int VacationStock { get; set; }

    public bool RequestVacation(DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }

    public void EndOfYearOperation()
    {
        // Check conditions and raise layoff event if needed
        if (VacationStock < 0)
        {
            OnEmployeeLayOff(new EmployeeLayOffEventArgs { Cause = LayOffCause.VacationStockNegative });
        }
        else if (DateTime.Now.Year - BirthDate.Year > 60)
        {
            OnEmployeeLayOff(new EmployeeLayOffEventArgs { Cause = LayOffCause.AgeAboveSixty });
        }
    }
}

public class SalesPerson : Employee
{
    public int AchievedTarget { get; set; }

    public bool CheckTarget(int quota)
    {
        return AchievedTarget >= quota;
    }

    // Overriding EndOfYearOperation to handle sales target specific logic
    public new void EndOfYearOperation()
    {
        // Check sales target and raise layoff event if target is not met
        // Assuming quota is a fixed value for simplicity
        int salesQuota = 100; // Example quota
        if (!CheckTarget(salesQuota))
        {
            OnEmployeeLayOff(new EmployeeLayOffEventArgs { Cause = LayOffCause.SalesTargetNotAchieved });
        }
        else
        {
            base.EndOfYearOperation(); // Call base method for other checks
        }
    }
}

public class BoardMember : Employee
{
    public void Resign()
    {
        OnEmployeeLayOff(new EmployeeLayOffEventArgs { Cause = LayOffCause.Resignation });
    }

    // BoardMember should always be a Club Member
    // No need to override EndOfYearOperation for age as it doesn't apply
}

public class Department
{
    public int DeptID { get; set; }
    public string DeptName { get; set; }

    private List<Employee> Staff = new List<Employee>();

    public void AddStaff(Employee e)
    {
        Staff.Add(e);
        e.EmployeeLayOff += RemoveStaff; // Register for EmployeeLayOff event
    }

    public void RemoveStaff(object sender, EmployeeLayOffEventArgs e)
    {
        var employee = sender as Employee;
        if (employee != null)
        {
            Staff.Remove(employee);
        }
    }
}

public class Club
{
    public int ClubID { get; set; }
    public string ClubName { get; set; }

    private List<Employee> Members = new List<Employee>();

    public void AddMember(Employee e)
    {
        Members.Add(e);
        e.EmployeeLayOff += RemoveMember; // Register for EmployeeLayOff event
    }

    public void RemoveMember(object sender, EmployeeLayOffEventArgs e)
    {
        var employee = sender as Employee;
        if (employee != null)
        {
            if (e.Cause == LayOffCause.VacationStockNegative)
            {
                Members.Remove(employee);
            }
            // For other reasons, the employee remains a member
        }
    }
}
