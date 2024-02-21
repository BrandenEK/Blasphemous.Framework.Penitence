using UnityEngine;
using Framework.Managers;
using System.Collections;

namespace Blasphemous.Framework.Penitence;

/// <summary>
/// An abstract representation of a penitence
/// </summary>
public abstract class ModPenitence
{
    /// <summary>
    /// The unique id of this penitence (PE...)
    /// </summary>
    protected internal abstract string Id { get; }

    /// <summary>
    /// The descriptive name of this penitence
    /// </summary>
    protected internal abstract string Name { get; }

    /// <summary>
    /// The full description of this penitence
    /// </summary>
    protected internal abstract string Description { get; }

    /// <summary>
    /// Should perform any necessary actions to activate this penitence's functionality
    /// </summary>
    protected internal abstract void Activate();

    /// <summary>
    /// Should perform any necessary actions to deactivate this penitence's functionality
    /// </summary>
    protected internal abstract void Deactivate();

    /// <summary>
    /// Should perform any necessary actions to complete the penitence.
    /// By default it marks the current penitence as complete
    /// </summary>
    public virtual IEnumerator Complete()
    {
        Core.PenitenceManager.MarkCurrentPenitenceAsCompleted();
        yield break;
    }

    internal Sprite InProgressImage { get; private set; }
    internal Sprite CompletedImage { get; private set; }
    internal Sprite AbandonedImage { get; private set; }
    internal Sprite GameplayImage { get; private set; }
    internal Sprite ChooseSelectedImage { get; private set; }
    internal Sprite ChooseUnselectedImage { get; private set; }

    /// <summary>
    /// Stores the associated images for the penitence - only executed on startup
    /// </summary>
    protected abstract void LoadImages(out Sprite inProgress, out Sprite completed, out Sprite abandoned, out Sprite gameplay, out Sprite chooseSelected, out Sprite chooseUnselected);

    /// <summary>
    /// Creates a new custom penitence
    /// </summary>
    public ModPenitence()
    {
        LoadImages(out Sprite inProgress, out Sprite completed, out Sprite abandoned, out Sprite gameplay, out Sprite chooseSelected, out Sprite chooseUnselected);
        InProgressImage = inProgress;
        CompletedImage = completed;
        AbandonedImage = abandoned;
        GameplayImage = gameplay;
        ChooseSelectedImage = chooseSelected;
        ChooseUnselectedImage = chooseUnselected;
    }
}
