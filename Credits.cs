namespace Dawn.PostProcessing
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///  Crediting this person / org for their code contribution(s) / snippet(s)
    /// </summary>
    /// <param name="To">Person / Organization</param>
    /// <param name="URL">URL for the code snippet (Optional).</param>
    [SuppressMessage("ReSharper", "NotAccessedField.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class Credit : Attribute { internal const string Info = "Crediting this person / org for their code contribution(s) / snippet(s)"; public Credit(string To, string URL = "") { } }

    /// <summary>
    ///  This snipped of code was modified from the original.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class Modified : Attribute { internal const string Info = "This snipped of code was modified from the original."; }
}