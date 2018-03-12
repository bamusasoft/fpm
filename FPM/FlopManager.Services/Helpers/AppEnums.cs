namespace FlopManager.Services.Helpers
{
    public enum ModelState
    { 
        New,
        Saved,
        Deleted,
    }
    public enum ViewState
    { 
        New,
        Saved,
        Edited,
        HasErrors,
        Deleted,
    }
    public enum LogMessageTypes
    {
        Error,
        Info,
    }
}
