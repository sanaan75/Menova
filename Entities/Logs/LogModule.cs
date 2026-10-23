namespace Entities.Logs;

public enum LogModule
{
    [EnumInfo(Glossary.Settings)]
    Settings = 1,

    [EnumInfo(Glossary.BasicInfo + " - " + Glossary.User)]
    Basics_User = 2,
}