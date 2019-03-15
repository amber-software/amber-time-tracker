using System.ComponentModel;

public enum Status
{
    [Description("Open")]
    Open, 
    [Description("In Progress")]
    InProgress, 
    [Description("Need Clarification")]
    NeedClarification, 
    [Description("Need Approve")]
    NeedApprove, 
    [Description("In Review")]
    InReview, 
    [Description("In Verification")]
    InVerification, 
    [Description("Closed")]
    Closed
}