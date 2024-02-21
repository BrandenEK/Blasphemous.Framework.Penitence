using UnityEngine;

namespace Blasphemous.Framework.Penitence;

/// <summary>
/// Stores all images relating to a penitence
/// </summary>
public class PenitenceImageCollection
{
    /// <summary> The menu icon for an active penitence </summary>
    public Sprite InProgress { get; set; }

    /// <summary> The menu icon for a completed penitence </summary>
    public Sprite Completed { get; set; }

    /// <summary> The menu icon for an abandoned penitence </summary>
    public Sprite Abandoned { get; set; }

    /// <summary> The in-game icon for the current penitence </summary>
    public Sprite Gameplay { get; set; }

    /// <summary> The selected icon in the penitence selector menu </summary>
    public Sprite ChooseSelected { get; set; }

    /// <summary> The unselected icon in the penitence selector menu </summary>
    public Sprite ChooseUnselected { get; set; }
}
